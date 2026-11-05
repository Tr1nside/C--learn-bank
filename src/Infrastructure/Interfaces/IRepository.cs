using LearnBank.Domain;

namespace LearnBank.Infrastructure;

/// <summary>
/// Абстракция хранилища сущностей. Позволяет заменить память на JSON или БД,
/// не меняя Domain и Application.
/// </summary>
/// <typeparam name="T">Тип сущности, у которой есть идентификатор.</typeparam>
public interface IRepository<T>
	where T : IEntity
{
	/// <summary>
	/// Возвращает сущность по идентификатору или <see langword="null"/>, если её нет.
	/// </summary>
	/// <param name="id">Идентификатор сущности.</param>
	/// <returns>Найденная сущность или <see langword="null"/>.</returns>
	T? GetById(Guid id);

	/// <summary>
	/// Возвращает все сохранённые сущности.
	/// </summary>
	/// <returns>Коллекция сущностей только для чтения.</returns>
	IReadOnlyCollection<T> GetAll();

	/// <summary>
	/// Добавляет сущность в хранилище.
	/// </summary>
	/// <param name="entity">Добавляемая сущность.</param>
	/// <exception cref="ArgumentNullException">Сущность равна <see langword="null"/>.</exception>
	/// <exception cref="InvalidOperationException">Сущность с таким идентификатором уже существует.</exception>
	void Add(T entity);
}
