namespace Pantry.Shared.Models {
    public abstract class OverviewBase {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public double? Rating { get; set; }
    }
}
