using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Api.Services.RabbitMqConsumerServices {
    public class PantryRabbitMqConsumerService : IRabbitMqConsumerService {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PantryRabbitMqConsumerService> _logger;

        public PantryRabbitMqConsumerService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<PantryRabbitMqConsumerService>();
        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.RegisterGood && typeof(Message<RegisterGoodMessage>).IsAssignableFrom(typeof(T)) && message is Message<RegisterGoodMessage> registerGoodMessage) {
                await ReactOnRegisterGood(registerGoodMessage.Content);
            }
            else if (type == MessageType.MinimizeGoodsQuantity && typeof(Message<Ingredient>).IsAssignableFrom(typeof(T)) && message is Message<Ingredient> minimizeGoodsQuantityMessage) {
                await ReactOnMinimizeGoodsQuantity(minimizeGoodsQuantityMessage.Content);
            }
        }

        private async Task ReactOnMinimizeGoodsQuantity(Ingredient ingredient) {
            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<PantryContext>();

                var goodEntity = await context.Goods.FindAsync(ingredient.PantryItemId);
                if (goodEntity != null) {
                    goodEntity.Amount -= ingredient.CountOff;
                    await context.SaveChangesAsync();
                }

            } catch (InvalidOperationException ex) {
                _logger.LogError($"{ex.Message}");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }



        private async Task ReactOnRegisterGood(RegisterGoodMessage message) {
            if (!message.Ingredient.PantryItemId.HasValue) {
                _logger.LogError("PantryItemId is NULL");
                return;
            }


            if (message.Ingredient.PantryItemId == Guid.Empty) {
                _logger.LogError("PantryItemId is nich gültig");
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            try {
                var context = scope.ServiceProvider.GetRequiredService<PantryContext>();

                var entity = new GoodEntity {
                    Owner = message.Owner,
                    Name = message.Ingredient.Name,
                    Id = message.Ingredient.PantryItemId.Value,
                    UnitOfMeasurement = message.Ingredient.Unit
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
