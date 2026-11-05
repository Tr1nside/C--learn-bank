namespace LearnBank.Infrastructure;

/// <summary>
/// Генератор идентификаторов на основе <see cref="Guid.NewGuid"/>.
/// </summary>
public class GuidGenerator : IIdGenerator
{
	/// <inheritdoc/>
	public Guid NewId()
	{
		return Guid.NewGuid();
	}
}
