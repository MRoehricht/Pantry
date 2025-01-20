

using Pantry.Module.Scanner.EanServices;

namespace Pantry.Module.Scanner.Tests.EanServices;

public class EanProductFinderServiceTest
{
    private readonly EanProductFinderService _service = new EanProductFinderService(new HttpClient());


    [Fact]
    public async Task FindProductTest()
    {
        var response = await _service.FindProduct(4104420071667);
        Assert.NotNull(response.Product);
        Assert.Equal(4104420071667, response.Product.EanId);
        Assert.Equal("Alnatura Bio Traubensaft, 330 ml", response.Product.Name);
    }

    [Fact]
    public void MapProductTest()
    {
        var responseContent = """
                              error=0
                              ---
                              asin=
                              name=
                              detailname=Alnatura Bio Traubensaft, 330 ml
                              vendor=Alnatura
                              maincat=
                              subcat=
                              maincatnum=-1
                              subcatnum=
                              contents=
                              pack=
                              origin=Deutschland
                              descr=Dieser Saft ist aus 100% Direktsaft und nicht aus Konzentraten. Er macht sich wegen dem naturtrüben besonders aus. 100% Direktsaft Ideal zum Mitnehmen und unterwegs genießen Dieser Saft ist aus 100% Direktsaft und nicht aus Konzentraten. Er macht sich wegen dem naturtrüben besonders aus.
                              name_en=
                              detailname_en=
                              descr_en=
                              validated=0 %
                              ---
                              """;

        var response = _service.MapEanProduct(GenerateStreamFromString(responseContent), 4104420071667);
        Assert.NotNull(response.Product);
        Assert.Equal(4104420071667, response.Product.EanId);
        Assert.Equal("Alnatura Bio Traubensaft, 330 ml", response.Product.Name);
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}