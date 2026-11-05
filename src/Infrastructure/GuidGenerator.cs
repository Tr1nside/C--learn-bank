namespace LearnBank.Infrastructure;

class GuidGenerator : IIdGenerator
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
