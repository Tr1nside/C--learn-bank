using LearnBank.Infrastructure;

namespace LearnBank;

/// <summary>
/// Точка входа приложения. Связывает реализации с интерфейсами.
/// </summary>
public static class Program
{
	/// <summary>
	/// Запускает демонстрационный сценарий.
	/// </summary>
	public static void Main()
	{
		IClock clock = new SystemClock();
		IIdGenerator ids = new GuidGenerator();
		ILogger logger = new ConsoleLogger();

		logger.Info($"Now: {clock.Now:O}");
		logger.Info($"Id:  {ids.NewId()}");

		try
		{
			throw new InvalidOperationException("Проверка логирования ошибок.");
		}
		catch (Exception ex)
		{
			logger.Warn("Демонстрация уровня предупреждения");
			logger.Error("Демонстрация уровня ошибки", ex);
		}
	}
}
