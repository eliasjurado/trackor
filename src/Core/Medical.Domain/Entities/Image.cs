using Medical.Domain.Common;

namespace Medical.Domain.Entities;

public class Image : BaseEntity<int>
{
    public string Data { get; set; } = string.Empty;
}
