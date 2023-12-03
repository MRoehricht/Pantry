namespace Pantry.Shared.Models.EanModels;
public class EanProduct
{
    public long EanId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Vendor { get; set; }
}
