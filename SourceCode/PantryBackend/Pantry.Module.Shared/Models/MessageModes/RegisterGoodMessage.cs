using Pantry.Module.Shared.Models.RecipeModels;

namespace Pantry.Module.Shared.Models.MessageModes {
    public class RegisterGoodMessage {
        public Ingredient Ingredient { get; set; }
        public string Owner { get; set; }
    }
}
