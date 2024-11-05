using Medical.Shared.Product;

namespace Medical.Application.Repositories.Commands;

public interface IProductCommandRepository : ICommandRepository<Product, int>
{
}
