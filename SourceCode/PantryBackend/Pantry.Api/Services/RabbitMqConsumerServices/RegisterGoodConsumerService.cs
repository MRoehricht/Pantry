using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Api.Services.RabbitMqConsumerServices {
    public class RegisterGoodConsumerService : IRabbitMqConsumerService {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterGoodConsumerService> _logger;

        public RegisterGoodConsumerService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<RegisterGoodConsumerService>();
        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.RegisterGood && typeof(Message<Ingredient>).IsAssignableFrom(typeof(T)) && message is Message<Ingredient> ingredientMessage) {
                await Create(ingredientMessage.Content);
            }
        }

        private async Task Create(Ingredient ingredient) {
            if (!ingredient.PantryItemId.HasValue) {
                _logger.LogError("PantryItemId is NULL");
                return;
            }


            if (ingredient.PantryItemId == Guid.Empty) {
                _logger.LogError("PantryItemId is nich gültig");
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<PantryContext>();

                var entity = new GoodEntity {
                    Name = ingredient.Name,
                    Id = ingredient.PantryItemId.Value
                };

                context.Goods.Add(entity);
                await context.SaveChangesAsync();


            } catch (InvalidOperationException ex) {
                _logger.LogError($"{ex.Message}");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }
    }
}
