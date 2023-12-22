using AutoMapper;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Recipe.Api.Services.RabbitMqConsumerServices {
    public class RecipeRabbitMqConsumerService : IRabbitMqConsumerService {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqPublisher _publisher;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipeRabbitMqConsumerService> _logger;

        public RecipeRabbitMqConsumerService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IRabbitMqPublisher publisher, IMapper mapper) {
            _serviceProvider = serviceProvider;
            _publisher = publisher;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<RecipeRabbitMqConsumerService>();

        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.UpdateIngredientName && typeof(Message<Ingredient>).IsAssignableFrom(typeof(T)) && message is Message<Ingredient> ingredientMessage) {
                await ReactOnUpdateIngredientName(ingredientMessage.Content);
            }else if (type == MessageType.MealWasCooked && typeof(Message<Guid>).IsAssignableFrom(typeof(T)) && message is Message<Guid> recipeId) {
                await ReactOnMealWasCooked(recipeId.Content);
            }

        }

        private async Task ReactOnMealWasCooked(Guid recipeId) {
            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<RecipeContext>();
                var recipe = await context.Recipes.FindAsync(recipeId);

                if (recipe != null) {
                    foreach (var ingredientEntity in recipe.Ingredients) {
                        _publisher.SendMessage(_mapper.Map<Ingredient>(ingredientEntity), MessageType.MinimizeGoodsQuantity);
                    }
                }

            } catch (InvalidOperationException ex) {
                _logger.LogError($"{ex.Message}");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }

        private async Task ReactOnUpdateIngredientName(Ingredient ingredient) {
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
                
                foreach (var recipe in context.Recipes) {
                    foreach (var ingredientItem in recipe.Ingredients) {
                        if (ingredientItem.PantryItemId == ingredient.PantryItemId) {
                            ingredientItem.Name = ingredient.Name;
                            ingredientItem.Unit = ingredient.Unit;
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
