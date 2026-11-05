namespace LearnBank.Infrastructure;

/// <summary>
/// Абстракция генератора идентификаторов. Позволяет управлять идентификаторами в тестах.
/// </summary>
public interface IIdGenerator
{
	/// <summary>
	/// Создаёт новый уникальный идентификатор.
	/// </summary>
	/// <returns>Новый идентификатор.</returns>
	Guid NewId();
}
