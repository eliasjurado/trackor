using Medical.Shared.Address;

namespace Medical.Application.MappingProfıles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressDto>().ReverseMap();
    }
}
