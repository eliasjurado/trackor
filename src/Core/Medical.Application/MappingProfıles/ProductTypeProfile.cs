using Medical.Shared.ProductType;

namespace Medical.Application.MappingProfıles;

public class ProductTypeProfile : Profile
{
    public ProductTypeProfile()
    {
        CreateMap<ProductType, ProductTypeDto>().ReverseMap();
    }
}
