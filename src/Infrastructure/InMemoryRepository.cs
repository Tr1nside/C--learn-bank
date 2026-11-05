using LearnBank.Domain;

namespace LearnBank.Infrastructure.Persistence;

/// <summary>
/// Реализация <see cref="IRepository{T}"/> в памяти. Подходит для тестов и первых сценариев.
/// </summary>
/// <typeparam name="T">Тип сущности, у которой есть идентификатор.</typeparam>
public class InMemoryRepository<T> : IRepository<T>
	where T : IEntity
{
	private readonly Dictionary<Guid, T> _items = [];

	/// <inheritdoc/>
	public T? GetById(Guid id)
	{
		_items.TryGetValue(id, out T? entity);
		return entity;
	}

	/// <inheritdoc/>
	public IReadOnlyCollection<T> GetAll()
	{
		return _items.Values.ToList();
	}

	/// <inheritdoc/>
	public void Add(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		if (!_items.TryAdd(entity.Id, entity))
		{
			throw new InvalidOperationException(
				$"Сущность с идентификатором {entity.Id} уже существует."
			);
		}
	}
}
