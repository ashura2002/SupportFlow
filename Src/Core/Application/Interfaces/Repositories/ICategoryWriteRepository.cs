
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryWriteRepository
    {
        void Add(Category category);
        void Remove(Category category);
    }
}
