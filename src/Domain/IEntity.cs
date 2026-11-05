namespace LearnBank.Domain;

/// <summary>
/// Сущность, у которой есть уникальный идентификатор.
/// </summary>
public interface IEntity
{
	/// <summary>
	/// Уникальный идентификатор сущности.
	/// </summary>
	Guid Id { get; }
}
