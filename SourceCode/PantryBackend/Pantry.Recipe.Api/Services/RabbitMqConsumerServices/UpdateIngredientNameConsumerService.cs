﻿using MassTransit;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Shared.Models.MessageModes;

namespace Pantry.Recipe.Api.Services.RabbitMqConsumerServices;

public class UpdateIngredientNameConsumerService(ILoggerFactory loggerFactory, RecipeContext recipeContext) : IConsumer<UpdateIngredientNameMessage>
{
    private readonly RecipeContext _recipeContext = recipeContext;
    private readonly ILogger<UpdateIngredientNameConsumerService> _logger = loggerFactory.CreateLogger<UpdateIngredientNameConsumerService>();

    public async Task Consume(ConsumeContext<UpdateIngredientNameMessage> context)
    {
        if (!context.Message.Ingredient.PantryItemId.HasValue)
        {
            _logger.LogError("PantryItemId is NULL");
            return;
        }


        if (context.Message.Ingredient.PantryItemId == Guid.Empty || string.IsNullOrWhiteSpace(context.Message.Ingredient.Name))
        {
            _logger.LogError("PantryItemId is nich gültig");
            return;
        }

        if (string.IsNullOrWhiteSpace(context.Message.Ingredient.Name))
        {
            _logger.LogError("Name is nich gültig");
            return;
        }
        try
        {

            foreach (var recipe in _recipeContext.Recipes.Where(e => e.Owner == context.Message.Owner))
            {
                foreach (var ingredientItem in recipe.Ingredients)
                {
                    if (ingredientItem.PantryItemId == context.Message.Ingredient.PantryItemId)
                    {
                        ingredientItem.Name = context.Message.Ingredient.Name;
                        ingredientItem.Unit = context.Message.Ingredient.Unit;
                    }
                }
            }

            await _recipeContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
