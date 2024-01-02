using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pantry.Plan.Api.Database.Contexts;
using Pantry.Shared.Models.MessageModes;

namespace Pantry.Plan.Api.Services.RabbitMqConsumerServices;

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
