using Medical.Domain.Common;

namespace Medical.Domain.Entities;

public class ProductType : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;

}
