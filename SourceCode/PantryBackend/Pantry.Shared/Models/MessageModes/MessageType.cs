using Pantry.Shared.Attribute.MessageAttributes;
using System.ComponentModel;
using Pantry.Shared.Extensions;

namespace Pantry.Shared.Models.MessageModes;

public enum MessageType : int
{
    [DestinationQueue(MessageConstants.PANTRY_QUEUE_NAME)]
    None = 0,
    [DestinationQueue(MessageConstants.PANTRY_API_QUEUE_NAME)]
    RegisterGood = 1,
    [DestinationQueue(MessageConstants.PANTRY_RECIPE_QUEUE_NAME)]
    UpdateIngredientName = 2,
    [DestinationQueue(MessageConstants.PANTRY_PLAN_QUEUE_NAME)]
    RecipeIsDeleted = 3,
    [DestinationQueue(MessageConstants.PANTRY_RECIPE_QUEUE_NAME)]
    MealWasCooked = 4,
    [DestinationQueue(MessageConstants.PANTRY_API_QUEUE_NAME)]
    MinimizeGoodsQuantity = 5,
}

public static class MessageTypeExtension {
    public static string GetDestinationQueue(this MessageType messageType) {

        var attribute = messageType.GetAttribute<DestinationQueueAttribute>();
        return attribute == null ? throw new ArgumentException(nameof(messageType)+ "hat keine DestinationQueue") : attribute.DestinationQueue;
    }
}