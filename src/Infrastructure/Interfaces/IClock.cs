namespace LearnBank.Infrastructure;

/// <summary>
/// Абстракция текущего времени. Позволяет подставлять фиксированное время в тестах.
/// </summary>
public interface IClock
{
	/// <summary>
	/// Текущее время.
	/// </summary>
	DateTime Now { get; }
}
