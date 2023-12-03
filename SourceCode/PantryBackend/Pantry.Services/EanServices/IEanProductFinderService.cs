using Pantry.Shared.Models.EanModels;

namespace Pantry.Services.EanServices;
public interface IEanProductFinderService
{
    Task<EanProductRespose> FindProduct(long eanId);
}