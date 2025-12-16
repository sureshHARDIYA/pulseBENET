using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.Categories.DTO;

namespace PulseLMS.Features.Categories;

public sealed class CategoriesController(AppDbContext db, ICurrentUser currentUser) : BaseController
{
    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponse>>> GetAllCategories(CancellationToken ct)
    {
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name ?? string.Empty,
                Description = c.Description,
                ParentId = c.ParentId
            })
            .ToListAsync(ct);

        return Ok(categories);
    }

	/// <summary>
	/// Get a category by id
	/// </summary>
	/// <param name="id">Category id</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>Category</returns>
	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken ct)
	{
			var category = await db.Categories
				.AsNoTracking()
				.Where(c => c.Id == id)
				.Select(c => new CategoryResponse
				{
					Id = c.Id,
					Name = c.Name ?? string.Empty,
					Description = c.Description,
					ParentId = c.ParentId
				})
				.FirstOrDefaultAsync(ct);

			if (category is null)
				return NotFound();

			return Ok(category);
	}

    /// <summary>
    /// Get all categories as tree 
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("tree")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CategoryTreeResponse>>> GetTree(CancellationToken ct)
    {
        var categories = await db.Categories
        .AsNoTracking()
        .Select(c => new
        {
            c.Id,
            c.Name,
            c.Description,
            c.ParentId
        }).ToListAsync(ct);

        var byParent = categories.ToLookup(c => c.ParentId);

        List<CategoryTreeResponse> Build(Guid? parentId)
        {
            return byParent[parentId]
                .Select(c => new CategoryTreeResponse
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Description = c.Description,
                    Children = Build(c.Id)
                })
                .ToList();
        }

        return Ok(Build(null));
    }

    /// <summary>
    /// Create a category (supports nesting via ParentId)
    /// </summary>
    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CategoryRequest request, CancellationToken ct)
    {
        var name = request.Name?.Trim();
        if (request.ParentId is not null)
        {
            var parentId = request.ParentId.Value;

            var parentExists = await db.Categories
                .AnyAsync(c => c.Id == parentId, ct);

            if (!parentExists)
                return BadRequest("ParentId does not exist.");
        }

        var entity = new Category
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            ParentId = request.ParentId
        };

        db.Categories.Add(entity);
        await db.SaveChangesAsync(ct);

        var response = new CategoryResponse
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            Description = entity.Description,
            ParentId = entity.ParentId
        };

			return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
    }

    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> Put(Guid id, [FromBody] CategoryRequest request, CancellationToken ct)
    {
        var userRole = User.FindFirst("user_role")?.Value;
        var userId = currentUser.UserId;

        if (userRole == "subscriber" || string.IsNullOrEmpty(userRole))
            return Forbid();

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return NotFound();

        // Author can only update categories created by them
        if (userRole == "author" && category.CreatedBy != userId)
            return Forbid();

        if (request.ParentId == id)
            return BadRequest("ParentId cannot be the same as the category id.");

        if (request.ParentId is not null)
        {
            var parentId = request.ParentId.Value;

            // Parent must exist
            var parentExists = await db.Categories.AnyAsync(c => c.Id == parentId, ct);
            if (!parentExists)
                return BadRequest("ParentId does not exist.");

            // Prevent cycles: a parent cannot be a descendant of this category
            var wouldCreateCycle = await IsDescendantAsync(parentId, id, ct);
            if (wouldCreateCycle)
                return BadRequest("Invalid ParentId. It would create a cycle.");
        }

        // Update fields
        category.Name = request.Name.Trim();
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        category.ParentId = request.ParentId;

        await db.SaveChangesAsync(ct);

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId
        });
    }

    // Walk up the parent chain from candidate parent to root. If we hit `targetId`, it's a cycle.
    private async Task<bool> IsDescendantAsync(Guid candidateParentId, Guid targetId, CancellationToken ct)
    {
        Guid? current = candidateParentId;

        while (current is not null)
        {
            if (current.Value == targetId)
                return true;

            var current1 = current;
            current = await db.Categories
                .AsNoTracking()
                .Where(c => c.Id == current1.Value)
                .Select(c => c.ParentId)
                .FirstOrDefaultAsync(ct);
        }

        return false;
    }

    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userRole = User.FindFirst("user_role")?.Value;
        var userId = currentUser.UserId;

        if (userRole == "subscriber" || string.IsNullOrEmpty(userRole))
            return Forbid();

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return NotFound();

        if (userRole == "author" && category.CreatedBy != userId)
            return Forbid();

        var hasChildren = await db.Categories.AnyAsync(c => c.ParentId == id, ct);
        if (hasChildren)
            return BadRequest("Cannot delete category because it has child categories. Delete/move children first.");

        var isUsedByQuizzes = await db.QuizCategories.AnyAsync(qc => qc.CategoryId == id, ct);
        if (isUsedByQuizzes)
            return BadRequest("Cannot delete category because it is linked to quizzes. Unlink it first.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }


}