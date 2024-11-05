using Medical.Shared.Cart;

namespace Medical.Application.MappingProfıles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItem, CartItemDto>().ReverseMap();
    }
}
