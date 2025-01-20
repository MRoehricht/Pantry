using Pantry.Module.Shared.Models.EanModels;

namespace Pantry.Module.Scanner.EanServices;
public interface IEanProductFinderService
{
    Task<EanProductRespose> FindProduct(long eanId);
}