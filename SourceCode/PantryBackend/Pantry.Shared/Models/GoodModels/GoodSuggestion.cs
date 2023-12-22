namespace Pantry.Shared.Models.GoodModels; 

public class GoodSuggestion {
    public Guid  Id{ get; set; }
    public string Name { get; set; }
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
}