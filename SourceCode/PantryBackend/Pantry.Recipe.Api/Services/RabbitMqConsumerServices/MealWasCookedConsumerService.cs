using MassTransit;
using Pantry.Recipe.Api.Configuration;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Shared.Models.MessageModes;

namespace Pantry.Recipe.Api.Services.RabbitMqConsumerServices;

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
