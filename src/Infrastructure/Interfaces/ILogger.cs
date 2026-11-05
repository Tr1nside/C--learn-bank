namespace LearnBank.Infrastructure;

/// <summary>
/// Интерфейс логирования. Позволяет заменить вывод в консоль на файл, аудит или тестовую заглушку.
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Записывает информационное сообщение.
	/// </summary>
	/// <param name="message">Текст сообщения.</param>
	void Info(string message);

	/// <summary>
	/// Записывает предупреждение.
	/// </summary>
	/// <param name="message">Текст сообщения.</param>
	void Warn(string message);

	/// <summary>
	/// Записывает ошибку.
	/// </summary>
	/// <param name="message">Описание ошибки.</param>
	/// <param name="exception">Исключение, вызвавшее ошибку.</param>
	void Error(string message, Exception exception);
}
