using AutoMapper;
using MassTransit;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Recipe.Api.Services.RabbitMqConsumerServices;

public class MealWasCookedConsumerService(ILoggerFactory loggerFactory, RecipeContext recipeContext, IPublishEndpoint publishEndpoint, IMapper mapper) : IConsumer<MealWasCookedMessage>
{
    private readonly RecipeContext _recipeContext = recipeContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<MealWasCookedConsumerService> _logger = loggerFactory.CreateLogger<MealWasCookedConsumerService>();

    public async Task Consume(ConsumeContext<MealWasCookedMessage> context)
    {       
        try
        {
            var recipe = await _recipeContext.Recipes.FindAsync(context.Message.RecipeId);

            if (recipe != null)
            {
                foreach (var ingredientEntity in recipe.Ingredients)
                {
                    await _publishEndpoint.Publish(new MinimizeGoodsQuantityMessage { Ingredient = _mapper.Map<Ingredient>(ingredientEntity) });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
