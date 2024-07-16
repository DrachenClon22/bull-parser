using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class Parser
{
    private static HttpClient _httpClient = new HttpClient();
    public static async Task<Bull[]?> GetBullsAsync(IPrintable? printer = null)
    {
        Bull[]? array_result = null;

        using (var request = new HttpRequestMessage(HttpMethod.Put, "https://xn--90aof1e.xn--p1ai/api/filter/1"))
        {
            request.Content = new StringContent(
            "[{\"value\":0,\"im\":\"Общая база быков\",\"field\":\"\",\"method\":\"data_base\",\"group\":\"\",\"ready\":true},{\"method\":\"page\",\"value\":1,\"ready\":true},{\"value\":\"1\",\"field\":\"typeSearch\",\"method\":\"radio\",\"ready\":true},{\"value\":true,\"field\":\"bull\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":false,\"field\":\"sperm\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":true,\"field\":\"parent\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":[[\"CVM\",\"CV\",\"TV\"],[\"BLAD\",\"BT\",\"TL\"],[\"Brachyspina\",\"BY\",\"TY\"],[\"DUMPS\",\"DP\",\"TD\"],[\"Mulefoot\",\"MF\",\"TM\"],[\"FXID\",\"FXIDC\",\"FXIDF\"],[\"Citrullinemia\",\"CNC\",\"CNF\"],[\"PT\",\"PTC\",\"PTF\"],[\"DF\",\"DFC\",\"DFF\"],[\"D2\",\"D2C\",\"D2F\"],[\"IS\",\"ISC\",\"ISF\"],[\"BD\",\"BDC\",\"BDF\"],[\"FH2\",\"FH2C\",\"FH2F\"],[\"Weaver\",\"WC\",\"WFF\"],[\"SMA\",\"SMAC\",\"SMAF\"],[\"SAA\",\"SAAC\",\"SAAF\"],[\"SDM\",\"SDMC\",\"SDMF\"],[\"DW\",\"DWC\",\"DWF\"],[\"OS\",\"OSC\",\"OSF\"],[\"AM\",\"AMC\",\"AMF\"],[\"DM\",\"DMC\",\"DMF\"],[\"NH\",\"NHC\",\"NHF\"],[\"aMAN\",\"aMANC\",\"aMANF\"],[\"bMAN\",\"bMANC\",\"bMANF\"],[\"CM1\",\"CM1C\",\"CM1F\"],[\"CM2\",\"CM2C\",\"CM2F\"],[\"CTS\",\"CTSC\",\"CTSF\"],[\"[HAM\",\"HAMC\",\"HAMF\"],[\"AP\",\"APC\",\"APF\"],[\"CA\",\"CAC\",\"CAF\"],[\"IE\",\"IEC\",\"IEF\"],[\"HDZ\",\"HDZC\",\"HDZF\"],[\"PK\",\"PKC\",\"PKF\"],[\"HHT\",\"HHTC\",\"HHTF\"],[\"HI\",\"HIC\",\"HIF\"],[\"DD\",\"DDC\",\"DDF\"],[\"CC\",\"CCC\",\"CCF\"],[\"HY\",\"HYC\",\"HYF\"],[\"TH\",\"THC\",\"THF\"],[\"CP\",\"CPC\",\"CPF\"],[\"PHA\",\"PHAC\",\"PHAF\"],[\"NS\",\"NSC\",\"NSF\"],[\"ICM\",\"ICMC\",\"ICMF\"],[\"OH\",\"OHC\",\"OHF\"],[\"OD\",\"ODC\",\"ODF\"],[\"GC\",\"GCC\",\"GCF\"],[\"MSUD\",\"MSUDC\",\"MSUDF\"],[\"HP\",\"HPC\",\"HPF\"],[\"NCL\",\"NCLC\",\"NCLF\"],[\"NPD\",\"NPDC\",\"NPDF\"],[\"TP\",\"TPC\",\"TPF\"],[\"A\",\"A\",\"A*\"],[\"BMS\",\"BMSC\",\"BMSF\"],[\"HG\",\"HGC\",\"HGF\"],[\"PP\",\"POC\",\"POF\"],[\"Pp\",\"POS\",\"POF\"],[\"Черн. окрас\",\"BC\",\"BF\"],[\"Красн. окрас\",\"RC\",\"RF\"],[\"POR\",\"POR\"],[\"RTF\",\"RTF\"]],\"method\":\"anomaly\",\"ready\":true},{\"method\":\"order\",\"data\":{}},{\"method\":\"token\",\"data\":\"\"},{\"method\":\"radio\",\"value\":1,\"field\":\"typeSearch\",\"ready\":true}]"
            , Encoding.UTF8, "application/json");

            using (var response = await _httpClient.SendAsync(request))
            {
                string result = await response.Content.ReadAsStringAsync();
                // TODO Писать результат в файл чтобы при обрыве чего-либо продолжать работу по парсингу
                JObject jobj = JObject.Parse(result);

                // TODO Что если все таки придет null?
                int amount = int.Parse(jobj?.SelectToken("pages.count_bulls")?.ToString()!);
                array_result = new Bull[amount];
                Console.WriteLine($"Найдено {amount} элементов\n");

                HtmlDocument doc = new HtmlDocument();
                HtmlNode node;

                string bull_result = string.Empty;
                JToken[]? tokens = jobj?.SelectToken("idArray")?.Values()?.ToArray();
                if (tokens is not null)
                {
                    for (int i = 0; i < tokens.Count(); i++)
                    {
                        // Получение полной информации о быке из карточки, для быстрого поиска можно использовать
                        // ответ на общий запрос с количеством из списка, получится в разы быстрее
                        using (var bull_request = new HttpRequestMessage(HttpMethod.Get, $"https://xn--90aof1e.xn--p1ai/bulls/bull/{tokens[i]}"))
                        using (var bull_response = await _httpClient.SendAsync(bull_request))
                        {
                            bull_result = await bull_response.Content.ReadAsStringAsync();

                            doc.LoadHtml(bull_result);
                            node = doc.DocumentNode.SelectSingleNode("//div[@class='klichka fl_l']");

                            array_result[i] = new Bull(Regex.Unescape(node.InnerHtml));

                            if (printer is not null)
                            {
                                printer.Print(array_result[i]);
                            }
                        }
                    }
                }
            }
        }
        return array_result;
    }
}
