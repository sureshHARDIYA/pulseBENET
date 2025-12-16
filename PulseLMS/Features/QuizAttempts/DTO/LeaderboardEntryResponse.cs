namespace PulseLMS.Features.QuizAttempt.DTO;

public class LeaderboardEntryResponse
{
	public required Guid UserId { get; init; }
	public int TotalScore { get; init; }
	public int Attempts { get; init; }
	public double AverageScore { get; init; }
}


