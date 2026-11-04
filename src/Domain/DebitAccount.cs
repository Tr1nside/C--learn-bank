namespace LearnBank.Domain;

/// <summary>
/// Дебетовый счёт. Операции в пределах собственных средств.
/// Баланс не может быть ниже нуля.
/// </summary>
public class DebitAccount : Account
{
    public DebitAccount(string number, decimal initialBalance = 0m)
        : base(number, initialBalance)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentOutOfRangeException.ThrowIfNegative(initialBalance);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">Сумма снятия строго больше нуля.</exception>
    /// <exception cref="InvalidOperationException">Недостаточно средств на счёте.</exception>
    public override void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (amount > Balance)
        {
            throw new InvalidOperationException(
                $"Недостаточно средств на счёте {Number}: доступно {Balance}, запрошено {amount}."
            );
        }

        Balance -= amount;
    }
}
