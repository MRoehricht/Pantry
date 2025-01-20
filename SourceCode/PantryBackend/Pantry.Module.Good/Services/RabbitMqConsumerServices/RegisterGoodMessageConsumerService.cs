using MassTransit;
using Pantry.Module.Good.Database.Contexts;
using Pantry.Module.Good.Database.Entities;
using Pantry.Module.Shared.Models.MessageModes;

namespace Pantry.Module.Good.Services.RabbitMqConsumerServices;

public class RegisterGoodMessageConsumerService(PantryContext pantryContext, ILoggerFactory loggerFactory) : IConsumer<RegisterGoodMessage>
{
    private readonly PantryContext _pantryContext = pantryContext;
    private readonly ILogger<RegisterGoodMessageConsumerService> _logger = loggerFactory.CreateLogger<RegisterGoodMessageConsumerService>();

    public async Task Consume(ConsumeContext<RegisterGoodMessage> context)
    {
        if (!context.Message.Ingredient.PantryItemId.HasValue)
        {
            _logger.LogError("PantryItemId is NULL");
            return;
        }


        if (context.Message.Ingredient.PantryItemId == Guid.Empty)
        {
            _logger.LogError("PantryItemId is nich gültig");
            return;
        }

        try
        {
            var entity = new GoodEntity
            {
                Owner = context.Message.Owner,
                Name = context.Message.Ingredient.Name,
                Id = context.Message.Ingredient.PantryItemId.Value,
                UnitOfMeasurement = context.Message.Ingredient.Unit
            };

            _pantryContext.Goods.Add(entity);
            await _pantryContext.SaveChangesAsync();


        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}

