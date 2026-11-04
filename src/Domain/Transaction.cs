namespace LearnBank.Domain;

/// <summary>
/// Запись о выполненной операции. Сумма всегда положительная,
/// направление задают <see cref="FromAccountId"/> и <see cref="ToAccountId"/>.
/// </summary>
public sealed class Transaction
{
	public Guid Id { get; }
	public DateTime Date { get; }
	public TransactionType Type { get; }
	public decimal Amount { get; }
	public Guid? FromAccountId { get; }
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

public enum TransactionType
{
	Deposit,
	Withdraw,
	Transfer,
	Interest,
}
