namespace Pantry.Shared.Models.GoodModels {
    public class GoodsOverview {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public int? Rating { get; set; }
    }
}
