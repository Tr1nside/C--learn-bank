namespace LearnBank.Domain;

/// <summary>
/// Клиент. Хранит свои счета, может добавлять и удалять их.
/// </summary>
public class Customer
{
	/// <summary>
	/// Уникальный идентификатор клиента.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Имя клиента.
	/// </summary>
	public string Name { get; }

	private readonly List<Account> _accounts = [];

	/// <summary>
	/// Список счетов только для чтения.
	/// </summary>
	public IReadOnlyList<Account> Accounts { get; }

	public Customer(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		Id = Guid.NewGuid();
		Name = name;
		Accounts = _accounts.AsReadOnly();
	}

	/// <summary>
	/// Удалить счёт клиента.
	/// </summary>
	/// <param name="accountId">Идентификатор счёта.</param>
	/// <exception cref="InvalidOperationException">Счёт у клиента не найден.</exception>
	public void RemoveAccount(Guid accountId)
	{
		Account account =
			_accounts.FirstOrDefault(a => a.Id == accountId)
			?? throw new InvalidOperationException(
				$"Счёт {accountId} не найден у клиента {Name} ({Id})."
			);
		_accounts.Remove(account);
	}

	/// <summary>
	/// Добавить счёт клиенту.
	/// </summary>
	/// <param name="account">Объект счёта.</param>
	/// <exception cref="ArgumentNullException">Счёт равен null.</exception>
	/// <exception cref="InvalidOperationException">Счёт уже принадлежит клиенту.</exception>
	public void AddAccount(Account account)
	{
		ArgumentNullException.ThrowIfNull(account);
		if (_accounts.Any(a => a.Id == account.Id))
		{
			throw new InvalidOperationException(
				$"Счёт {account.Number} ({account.Id}) уже найден у клиента {Name} ({Id}). Добавление невозможно."
			);
		}
		_accounts.Add(account);
	}

	/// <summary>
	/// Общий баланс счетов клиента.
	/// </summary>
	/// <returns>Сумма балансов всех счетов клиента.</returns>
	public decimal TotalBalance()
	{
		return _accounts.Sum(a => a.Balance);
	}
}
