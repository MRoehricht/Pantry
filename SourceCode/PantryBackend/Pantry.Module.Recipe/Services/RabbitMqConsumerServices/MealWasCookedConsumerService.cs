using MassTransit;
using Pantry.Module.Recipe.Configuration;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Shared.Models.MessageModes;

namespace Pantry.Module.Recipe.Services.RabbitMqConsumerServices;

public class MealWasCookedConsumerService(ILoggerFactory loggerFactory, RecipeContext recipeContext, IPublishEndpoint publishEndpoint) : IConsumer<MealWasCookedMessage>
{
    private readonly RecipeContext _recipeContext = recipeContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger<MealWasCookedConsumerService> _logger = loggerFactory.CreateLogger<MealWasCookedConsumerService>();

    public async Task Consume(ConsumeContext<MealWasCookedMessage> context)
    {
        try
        {
            var recipe = await _recipeContext.Recipes.FindAsync(context.Message.RecipeId);

            if (recipe != null)
            {
                var mapper = new RecipeMapper();
                recipe.Details.CookedOn.Add(DateOnly.FromDateTime(DateTime.Now));
                await _recipeContext.SaveChangesAsync();
                foreach (var ingredientEntity in recipe.Ingredients)
                {
                    await _publishEndpoint.Publish(new MinimizeGoodsQuantityMessage { Ingredient = mapper.MapToIngredient(ingredientEntity) });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
