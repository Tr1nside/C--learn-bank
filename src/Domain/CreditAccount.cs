namespace LearnBank.Domain;

/// <summary>
/// Кредитный счёт. Операции в пределах кредитного лимита.
/// Баланс не может опускаться ниже <c>-Limit</c>.
/// </summary>
public class CreditAccount : Account
{
	/// <summary>
	/// Кредитный лимит.
	/// </summary>
	public decimal Limit { get; }

	public CreditAccount(string number, decimal limit, decimal initialBalance = 0m)
		: base(number, initialBalance)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(number);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);
		ArgumentOutOfRangeException.ThrowIfNegative(limit + initialBalance);
		Limit = limit;
	}

	/// <inheritdoc/>
	/// <exception cref="ArgumentOutOfRangeException">Сумма снятия строго больше нуля.</exception>
	/// <exception cref="InvalidOperationException">Выход за лимит.</exception>
	public override void Withdraw(decimal amount)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

		if (Balance - amount < -Limit)
		{
			throw new InvalidOperationException(
				$"Выход за лимит при попытке снятия средств на счёте {Number}: доступно {Balance}, лимит {Limit}, запрошено {amount}."
			);
		}

		Balance -= amount;
	}
}
