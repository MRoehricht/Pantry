using Pantry.Api.Database.Entities;
using Pantry.Shared.Models.GoodModels;
using Riok.Mapperly.Abstractions;

namespace Pantry.Api.Configuration;

[Mapper]
public partial class PantryMapper
{
    public partial Good MapToGood(GoodEntity goodEntity);
    public partial GoodEntity MapToGoodEntity(Good good);
    public partial IEnumerable<Good> MapToGoods(List<GoodEntity> goodEntities);

    public IEnumerable<GoodsOverview> MapToGoodsOverviews(List<GoodEntity> source)
    {
        return source.Select(x => MapToGoodsOverview(x));
    }

    public GoodsOverview MapToGoodsOverview(GoodEntity source)
    {
        return new GoodsOverview
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Tags = source.Details.Tags,
            Rating = source.Details.Ratings.Any() ? source.Details.Ratings.Average() : null
        };
    }

    public GoodRating MapToGoodRating(GoodEntity source)
    {
        return new GoodRating
        {
            GoodId = source.Id,
            Ratings = source.Details.Ratings
        };
    }


    public IEnumerable<GoodSuggestion> MapToGoodSuggestions(List<GoodEntity> source)
    {
        return source.Select(x => MapToGoodSuggestion(x));
    }

    public GoodSuggestion MapToGoodSuggestion(GoodEntity source)
    {
        return new GoodSuggestion
        {
            Id = source.Id,
            Name = source.Name,
            UnitOfMeasurement = source.UnitOfMeasurement
        };
    }
}
