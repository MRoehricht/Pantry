using Microsoft.EntityFrameworkCore;
using Pantry.Plan.Api.Database.Contexts;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Plan.Api.Services.RabbitMqConsumerServices {
    public class PlanRabbitMqConsumerService: IRabbitMqConsumerService {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PlanRabbitMqConsumerService> _logger;

        public PlanRabbitMqConsumerService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<PlanRabbitMqConsumerService>();

        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.RecipeIsDeleted && typeof(Message<Guid>).IsAssignableFrom(typeof(T)) && message is Message<Guid> deleteMessage) {
                await UpdateIngredientName(deleteMessage.Content);
            }
        }

        private async Task UpdateIngredientName(Guid recipeId) {
            try {
                using IServiceScope scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PlanContext>();

                var meals = await context.Meals.Where(_ => _.RecipeId == recipeId).ToListAsync();

                meals.ForEach(meal => context.Meals.Remove(meal)); 
                await context.SaveChangesAsync();

            }  catch (Exception ex) {
                _logger.LogError(ex.Message);
            }
        }
    }
}
