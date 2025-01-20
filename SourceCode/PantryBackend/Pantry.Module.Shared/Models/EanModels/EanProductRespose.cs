namespace Pantry.Module.Shared.Models.EanModels;
public record EanProductRespose
{
    public int ErrorId { get; set; }

    public string? ErrorValue { get; set; }

    public EanProduct? Product { get; set; }
}
