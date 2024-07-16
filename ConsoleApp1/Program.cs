using System.Collections;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class Bull
    {
        public string Name { get; set; }
        public string Add { get; set; }
        public string Number { get; set; }
        public Bull(string html_info)
        {
            var firsthalf = html_info.Substring(0, html_info.IndexOf("<br>")).Trim();
            Number = firsthalf.Split(" ")[0];
            Name = firsthalf.Split(" ")[1];
            Add = html_info
                .Substring(html_info.IndexOf("<small>"), html_info.IndexOf("</small>")- html_info.IndexOf("<small>"))
                .Replace("&nbsp;", " ")
                .Replace("<small>", "");

            Name = String.Join(" ", Name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            Number = String.Join(" ", Number.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            Add = String.Join(" ", Add.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        public override string ToString()
        {
            return $"{Name} Инв: {Number} Доп: {Add}";
        }
    }
    internal class Program
    {
        static HttpClient httpClient = new HttpClient();
        static List<Bull> bulls = new List<Bull>();
        static async Task Main()
        {
            // определяем данные запроса
            string token = "";
            string result = "";

            using (var request = new HttpRequestMessage(HttpMethod.Put, "https://xn--90aof1e.xn--p1ai/api/filter/1"))
            {
                request.Content = new StringContent(
                "[{\"value\":0,\"im\":\"Общая база быков\",\"field\":\"\",\"method\":\"data_base\",\"group\":\"\",\"ready\":true},{\"method\":\"page\",\"value\":1,\"ready\":true},{\"value\":\"1\",\"field\":\"typeSearch\",\"method\":\"radio\",\"ready\":true},{\"value\":true,\"field\":\"bull\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":false,\"field\":\"sperm\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":true,\"field\":\"parent\",\"method\":\"checkbox\",\"group\":\"prizn\",\"ready\":true},{\"value\":[[\"CVM\",\"CV\",\"TV\"],[\"BLAD\",\"BT\",\"TL\"],[\"Brachyspina\",\"BY\",\"TY\"],[\"DUMPS\",\"DP\",\"TD\"],[\"Mulefoot\",\"MF\",\"TM\"],[\"FXID\",\"FXIDC\",\"FXIDF\"],[\"Citrullinemia\",\"CNC\",\"CNF\"],[\"PT\",\"PTC\",\"PTF\"],[\"DF\",\"DFC\",\"DFF\"],[\"D2\",\"D2C\",\"D2F\"],[\"IS\",\"ISC\",\"ISF\"],[\"BD\",\"BDC\",\"BDF\"],[\"FH2\",\"FH2C\",\"FH2F\"],[\"Weaver\",\"WC\",\"WFF\"],[\"SMA\",\"SMAC\",\"SMAF\"],[\"SAA\",\"SAAC\",\"SAAF\"],[\"SDM\",\"SDMC\",\"SDMF\"],[\"DW\",\"DWC\",\"DWF\"],[\"OS\",\"OSC\",\"OSF\"],[\"AM\",\"AMC\",\"AMF\"],[\"DM\",\"DMC\",\"DMF\"],[\"NH\",\"NHC\",\"NHF\"],[\"aMAN\",\"aMANC\",\"aMANF\"],[\"bMAN\",\"bMANC\",\"bMANF\"],[\"CM1\",\"CM1C\",\"CM1F\"],[\"CM2\",\"CM2C\",\"CM2F\"],[\"CTS\",\"CTSC\",\"CTSF\"],[\"[HAM\",\"HAMC\",\"HAMF\"],[\"AP\",\"APC\",\"APF\"],[\"CA\",\"CAC\",\"CAF\"],[\"IE\",\"IEC\",\"IEF\"],[\"HDZ\",\"HDZC\",\"HDZF\"],[\"PK\",\"PKC\",\"PKF\"],[\"HHT\",\"HHTC\",\"HHTF\"],[\"HI\",\"HIC\",\"HIF\"],[\"DD\",\"DDC\",\"DDF\"],[\"CC\",\"CCC\",\"CCF\"],[\"HY\",\"HYC\",\"HYF\"],[\"TH\",\"THC\",\"THF\"],[\"CP\",\"CPC\",\"CPF\"],[\"PHA\",\"PHAC\",\"PHAF\"],[\"NS\",\"NSC\",\"NSF\"],[\"ICM\",\"ICMC\",\"ICMF\"],[\"OH\",\"OHC\",\"OHF\"],[\"OD\",\"ODC\",\"ODF\"],[\"GC\",\"GCC\",\"GCF\"],[\"MSUD\",\"MSUDC\",\"MSUDF\"],[\"HP\",\"HPC\",\"HPF\"],[\"NCL\",\"NCLC\",\"NCLF\"],[\"NPD\",\"NPDC\",\"NPDF\"],[\"TP\",\"TPC\",\"TPF\"],[\"A\",\"A\",\"A*\"],[\"BMS\",\"BMSC\",\"BMSF\"],[\"HG\",\"HGC\",\"HGF\"],[\"PP\",\"POC\",\"POF\"],[\"Pp\",\"POS\",\"POF\"],[\"Черн. окрас\",\"BC\",\"BF\"],[\"Красн. окрас\",\"RC\",\"RF\"],[\"POR\",\"POR\"],[\"RTF\",\"RTF\"]],\"method\":\"anomaly\",\"ready\":true},{\"method\":\"order\",\"data\":{}},{\"method\":\"token\",\"data\":\"\"},{\"method\":\"radio\",\"value\":1,\"field\":\"typeSearch\",\"ready\":true}]"
                , Encoding.UTF8, "application/json");
                // отправляем запрос
                using (var response = await httpClient.SendAsync(request))
                {
                    // получаем ответ
                    token = await response.Content.ReadAsStringAsync();
                    token = Regex.Unescape(token);

                    token = token
                        .Remove(0, token.IndexOf("\"token\""))
                        .Split(",")[0]
                        .Replace("\"", "")
                        .Split(":")[1];
                }
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://xn--90aof1e.xn--p1ai/api/filter/{token}"))
            using (var response = await httpClient.SendAsync(request))
            {
                result = await response.Content.ReadAsStringAsync();
                result = Regex.Unescape(result);
                JObject jobj = JObject.Parse(result);
                Console.WriteLine($"Найдено {jobj.SelectToken("pages.count_bulls")} элементов\n");
                foreach (var item in jobj.SelectToken("idArray").Values())
                {
                    using (var bull_request = new HttpRequestMessage(HttpMethod.Get, $"https://xn--90aof1e.xn--p1ai/bulls/bull/{item}"))
                    using (var bull_response = await httpClient.SendAsync(bull_request))
                    {
                        var bull_result = await bull_response.Content.ReadAsStringAsync();
                        try
                        {
                            Regex.Unescape(bull_result);
                        }
                        finally { }
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(bull_result);
                        HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='klichka fl_l']");
                        Bull bull = new Bull(node.InnerHtml);
                        bulls.Add(bull);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(bull.Name);
                        Console.ResetColor();
                        Console.Write(" НОМ: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(bull.Number);
                        Console.ResetColor();
                        Console.Write(" ДОП: ");
                        Console.WriteLine(bull.Add);
                    }
                  
                }
            }

            //Console.WriteLine(result);
        }
    }
}
