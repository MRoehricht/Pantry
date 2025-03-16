using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Module.Plan.Database.Contexts;
using Pantry.Module.Shared.Models.MessageModes;

namespace Pantry.Module.Plan.Services.RabbitMqConsumerServices;

public class RecipeIsDeletedConsumerSerivce(ILoggerFactory loggerFactory, PlanContext planContext) : IConsumer<RecipeIsDeletedMessage>
{
    private readonly PlanContext _planContext = planContext;
    private readonly ILogger<RecipeIsDeletedConsumerSerivce> _logger = loggerFactory.CreateLogger<RecipeIsDeletedConsumerSerivce>();

    public async Task Consume(ConsumeContext<RecipeIsDeletedMessage> context)
    {
        try
        {
            var meals = await _planContext.Meals.Where(_ => _.RecipeId == context.Message.RecipeId).ToListAsync();

            meals.ForEach(meal => _planContext.Meals.Remove(meal));
            await _planContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
