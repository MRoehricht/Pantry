using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Recipe.Api.Services.RabbitMqConsumerServices {
    public class UpdateIngredientNameConsumerService : IRabbitMqConsumerService {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UpdateIngredientNameConsumerService> _logger;

        public UpdateIngredientNameConsumerService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<UpdateIngredientNameConsumerService>();

        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.UpdateIngredientName && typeof(Message<Ingredient>).IsAssignableFrom(typeof(T)) && message is Message<Ingredient> ingredientMessage) {
                await UpdateIngredientName(ingredientMessage.Content);
            }
        }

        private async Task UpdateIngredientName(Ingredient ingredient) {
            if (!ingredient.PantryItemId.HasValue) {
                _logger.LogError("PantryItemId is NULL");
                return;
            }


            if (ingredient.PantryItemId == Guid.Empty || string.IsNullOrWhiteSpace(ingredient.Name)) {
                _logger.LogError("PantryItemId is nich gültig");
                return;
            }

            if (string.IsNullOrWhiteSpace(ingredient.Name)) {
                _logger.LogError("Name is nich gültig");
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<RecipeContext>();

                //var recipes = context.Recipes.Where(r => r.Ingredients.Any(i => i.PantryItemId == ingredient.PantryItemId)).ToList();

                foreach (var recipe in context.Recipes) {
                    foreach (var ingredientItem in recipe.Ingredients) {
                        if (ingredientItem.PantryItemId == ingredient.PantryItemId) {
                            ingredientItem.Name = ingredient.Name;
                        }
                    }

                }

                await context.SaveChangesAsync();

            }
            catch (InvalidOperationException ex) {
                _logger.LogError($"{ex.Message}");
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }
    }
}
