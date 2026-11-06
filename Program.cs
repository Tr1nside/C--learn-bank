using LearnBank.Domain;
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
		ILogger logger = new ConsoleLogger();
		IRepository<Account> accounts = new InMemoryRepository<Account>();

		logger.Info($"Now: {clock.Now:O}");

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
	// private static ConsoleMenu Compose(ILogger logger)
	// {
	//     IClock clock = new SystemClock();
	//     IRepository<Account> accounts = new InMemoryRepository<Account>();

	//     var service = new BankService(accounts, clock, logger);
	//     return new ConsoleMenu(service, logger);
	// }
}
