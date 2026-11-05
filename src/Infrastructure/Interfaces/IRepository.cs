using LearnBank.Domain;
namespace LearnBank.Infrastructure;

interface IRepository<T> where T : IEntity
{
    T? GetById(Guid id);
    IReadOnlyCollection<T> GetAll();
    void Add(T entity);
}
