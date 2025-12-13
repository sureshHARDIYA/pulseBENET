using Microsoft.EntityFrameworkCore;
using PulseLMS.Data;

namespace PulseLMS.Features.Categories.Services;

public sealed class CategoryService(AppDbContext dbContext)
{
    public async Task<bool> AllCategoriesExistAsync(IReadOnlyCollection<Guid> categoryIds, CancellationToken ct)
    {
        if (categoryIds.Count == 0) return true;
        
        var count = await dbContext.Categories
            .CountAsync(c => categoryIds.Contains(c.Id), ct);

        return count == categoryIds.Count;
    }
}