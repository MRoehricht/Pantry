using Pantry.Api.Database.Repositories;
using Pantry.Services.RabbitMqServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Api.Services.RabbitMqConsumerServices {
    public class RegisterGoodConsumerService : IRabbitMqConsumerService {
        private readonly IGoodRepository _goodRepository;

        public RegisterGoodConsumerService(IGoodRepository goodRepository) {
            _goodRepository = goodRepository;
        }

        public async Task ProcessMessage<T>(T message, MessageType type) {
            if (type == MessageType.RegisterGood && typeof(Message<Ingredient>).IsAssignableFrom(typeof(T)) && message is Message<Ingredient> ingredientMessage) {
                await _goodRepository.Create(ingredientMessage.Content);
            }
        }
    }
}
