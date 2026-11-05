namespace LearnBank.Infrastructure;

/// <summary>
/// Часы, возвращающие системное время.
/// </summary>
public class SystemClock : IClock
{
	/// <inheritdoc/>
	public DateTime Now => DateTime.Now;
}
