namespace LearnBank.Domain;

/// <summary>
/// Абстрактный класс счёта. Хранит идентификатор, баланс и номер.
/// Правила снятия определяет конкретный класс счёта.
/// </summary>
public abstract class Account
{
	/// <summary>
	/// Уникальный идентификатор счёта.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Баланс счёта. Изменяется только через <see cref="Deposit"/> и <see cref="Withdraw"/>.
	/// Допустимость отрицательного баланса определяется типом счёта.
	/// </summary>
	public decimal Balance { get; protected set; }

	/// <summary>
	/// Номер счёта.
	/// </summary>
	public string Number { get; }

	protected Account(string number, decimal initialBalance = 0m)
	{
		Id = Guid.NewGuid();
		Balance = initialBalance;
		Number = number;
	}

	/// <summary>
	/// Вносит сумму на баланс счёта.
	/// </summary>
	/// <param name="amount">Сумма пополнения, строго больше нуля.</param>
	/// <exception cref="ArgumentOutOfRangeException">Сумма пополнения не больше нуля.</exception>
	public virtual void Deposit(decimal amount)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

		Balance += amount;
	}

	/// <summary>
	/// Снимает деньги со счёта. Логику работы определяет реализация конкретного счёта.
	/// </summary>
	/// <param name="amount">Сумма снятия, строго больше нуля.</param>
	public abstract void Withdraw(decimal amount);
}
