namespace LearnBank.Domain;

/// <summary>
/// Запись о выполненной операции. Сумма всегда положительная,
/// направление задают <see cref="FromAccountId"/> и <see cref="ToAccountId"/>.
/// </summary>
public sealed class Transaction : IEntity
{
	/// <summary>
	/// Уникальный идентификатор транзакции.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Дата и время выполнения операции.
	/// </summary>
	public DateTime Date { get; }

	/// <summary>
	/// Тип операции.
	/// </summary>
	public TransactionType Type { get; }

	/// <summary>
	/// Сумма операции. Всегда положительная.
	/// </summary>
	public decimal Amount { get; }

	/// <summary>
	/// Идентификатор счёта-источника. <see langword="null"/> для пополнения и начисления процентов.
	/// </summary>
	public Guid? FromAccountId { get; }

	/// <summary>
	/// Идентификатор счёта-получателя. <see langword="null"/> для снятия.
	/// </summary>
	public Guid? ToAccountId { get; }

	public Transaction(
		DateTime date,
		TransactionType type,
		decimal amount,
		Guid? fromAccountId,
		Guid? toAccountId
	)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
		ValidateSides(type, fromAccountId, toAccountId);

		Id = Guid.NewGuid();
		Date = date;
		Type = type;
		Amount = amount;
		FromAccountId = fromAccountId;
		ToAccountId = toAccountId;
	}

	private static void ValidateSides(TransactionType type, Guid? fromAccountId, Guid? toAccountId)
	{
		switch (type)
		{
			case TransactionType.Deposit:
			case TransactionType.Interest:
				if (fromAccountId is not null || toAccountId is null)
				{
					throw new ArgumentException(
						$"Операция {type} требует только счёт-получатель.",
						nameof(toAccountId)
					);
				}
				break;

			case TransactionType.Withdraw:
				if (fromAccountId is null || toAccountId is not null)
				{
					throw new ArgumentException(
						$"Операция {type} требует только счёт-источник.",
						nameof(fromAccountId)
					);
				}
				break;

			case TransactionType.Transfer:
				if (fromAccountId is null || toAccountId is null)
				{
					throw new ArgumentException(
						"Перевод требует и счёт-источник, и счёт-получатель.",
						nameof(toAccountId)
					);
				}

				if (fromAccountId == toAccountId)
				{
					throw new ArgumentException(
						"Счёт-источник и счёт-получатель перевода должны различаться.",
						nameof(toAccountId)
					);
				}
				break;

			default:
				throw new ArgumentOutOfRangeException(
					nameof(type),
					type,
					"Неизвестный тип транзакции."
				);
		}
	}
}

/// <summary>
/// Тип транзакции.
/// </summary>
public enum TransactionType
{
	/// <summary>
	/// Пополнение счёта.
	/// </summary>
	Deposit,

	/// <summary>
	/// Снятие средств со счёта.
	/// </summary>
	Withdraw,

	/// <summary>
	/// Перевод между счетами.
	/// </summary>
	Transfer,

	/// <summary>
	/// Начисление процентов.
	/// </summary>
	Interest,
}
