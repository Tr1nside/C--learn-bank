namespace LearnBank.Infrastructure;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
