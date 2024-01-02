using MassTransit;
using Pantry.Api.Database.Contexts;
using Pantry.Shared.Models.MessageModes;

namespace Pantry.Api.Services.RabbitMqConsumerServices;

public class MinimizeGoodsQuantityMessageConsumerService(PantryContext pantryContext, ILoggerFactory loggerFactory) : IConsumer<MinimizeGoodsQuantityMessage>
{
    private readonly PantryContext _pantryContext = pantryContext;
    private readonly ILogger<MinimizeGoodsQuantityMessageConsumerService> _logger = loggerFactory.CreateLogger<MinimizeGoodsQuantityMessageConsumerService>();

    public async Task Consume(ConsumeContext<MinimizeGoodsQuantityMessage> context)
    {
        try
        {
            var goodEntity = await _pantryContext.Goods.FindAsync(context.Message.Ingredient.PantryItemId);
            if (goodEntity != null)
            {
                goodEntity.Amount -= context.Message.Ingredient.CountOff;
                await _pantryContext.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
