namespace LearnBank.Infrastructure;

/// <summary>
/// Реализация <see cref="ILogger"/>, выводящая сообщения в консоль с указанием уровня.
/// </summary>
public class ConsoleLogger : ILogger
{
	/// <inheritdoc/>
	public void Info(string message)
	{
		Console.WriteLine($"[INFO]  {message}");
	}

	/// <inheritdoc/>
	public void Warn(string message)
	{
		Console.WriteLine($"[WARN]  {message}");
	}

	/// <inheritdoc/>
	public void Error(string message, Exception exception)
	{
		Console.WriteLine($"[ERROR] {message}: {exception.GetType().Name}: {exception.Message}");
	}
}
