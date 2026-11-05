using LearnBank.Infrastructure;

namespace LearnBank;

public static class Program
{
    public static void Main()
    {
        IClock clock = new SystemClock();
        IIdGenerator ids = new GuidGenerator();

        Console.WriteLine($"Now: {clock.Now:O}");
        Console.WriteLine($"Id:  {ids.NewId()}");
    }
}
