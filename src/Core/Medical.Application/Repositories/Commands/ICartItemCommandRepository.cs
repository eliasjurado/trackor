using Medical.Shared.Cart;

namespace Medical.Application.Repositories.Commands;

public interface ICartItemCommandRepository : ICommandRepository<CartItem, int>
{
}
