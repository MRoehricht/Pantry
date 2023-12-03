using Pantry.Shared.Models.EanModels;

namespace Pantry.Services.EanServices;
public class EanProductFinderService : IEanProductFinderService
{
    private const string _detailname = "detailname=";
    private const string _descr = "descr=";
    private const string _vendor = "vendor=";
    private const string _error = "error=";
    private readonly HttpClient _httpClient;

    public EanProductFinderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EanProductRespose> FindProduct(long eanId)
    {
        using var response = await _httpClient.GetAsync($"http://opengtindb.org/?ean={eanId}&cmd=query&queryid=400000000", HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode) return new EanProductRespose { ErrorId = 16, ErrorValue = MapError(16) };
        var output = new EanProductRespose();
        var stream = await response.Content.ReadAsStreamAsync();

        if (stream == null || stream.Length == 0)
        {
            output.ErrorId = 15;
            output.ErrorValue = MapError(15);
            return output;
        }

        return MapEanProduct(stream, eanId);
    }

    public EanProductRespose MapEanProduct(Stream stream, long eanId)
    {
        var output = new EanProductRespose();
        var product = new EanProduct { EanId = eanId };

        using (StreamReader reader = new(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(_error))
                {
                    if (int.TryParse(line.Substring(_error.Length).Trim(), out var errorId))
                    {
                        output.ErrorId = errorId;
                        output.ErrorValue = MapError(errorId);
                    }
                }
                else if (line.StartsWith(_detailname))
                {
                    product.Name = line.Substring(_detailname.Length).Trim();
                }
                else if (line.StartsWith(_descr))
                {
                    product.Description = line.Substring(_descr.Length).Trim();
                }
                else if (line.StartsWith(_vendor))
                {
                    product.Vendor = line.Substring(_vendor.Length).Trim();
                }
            }
        }

        output.Product = product;

        return output;
    }

    public static string MapError(int errorCode)
    {
        return errorCode switch
        {
            0 => "OK - Operation war erfolgreich",
            1 => "not found - die EAN konnte nicht gefunden werden ",
            2 => "checksum - die EAN war fehlerhaft (Checksummenfehler) ",
            3 => "EAN-format - die EAN war fehlerhaft (ungültiges Format / fehlerhafte Ziffernanzahl) ",
            4 => "not a global, unique EAN - es wurde eine für interne Anwendungen reservierte EAN eingegeben (In-Store, Coupon etc.) ",
            5 => "access limit exceeded - Zugriffslimit auf die Datenbank wurde überschritten",
            6 => "no product name - es wurde kein Produktname angegeben ",
            7 => "product name too long - der Produktname ist zu lang (max. 20 Zeichen) ",
            8 => "no or wrong main category id - die Nummer für die Hauptkategorie fehlt oder liegt außerhalb des erlaubten Bereiches ",
            9 => "no or wrong sub category id - die Nummer für die zugehörige Unterkategorie fehlt oder liegt außerhalb des erlaubten Bereiches ",
            10 => "illegal data in vendor field - unerlaubte Daten im Herstellerfeld ",
            11 => "illegal data in description field - unerlaubte Daten im Beschreibungsfeld ",
            12 => "data already submitted - Daten wurden bereits übertragen",
            13 => "queryid missing or wrong - die UserID/queryid fehlt in der Abfrage oder ist für diese Funktion nicht freigeschaltet ",
            14 => "unknown command - es wurde mit dem Parameter \"cmd\" ein unbekanntes Kommando übergeben",
            15 => "ResponseBody war leer",
            16 => "Http StatusCode war nicht erfolgreich.",
            _ => "Fehler unbekannt",
        };
    }
}
