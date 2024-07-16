public class Bull
{
    public string Name { get; set; }
    public string Add { get; set; }
    public string Number { get; set; }
    public Bull(string html_info)
    {
        // TODO Можно сделать лучше
        var firsthalf = html_info.Substring(0, html_info.IndexOf("<br>")).Trim();
        Number = firsthalf.Split(" ")[0];
        Name = firsthalf.Split(" ")[1];

        // TODO Демо доп инфы
        Add = html_info
            .Substring(html_info.IndexOf("<small>"), html_info.IndexOf("</small>") - html_info.IndexOf("<small>"))
            .Replace("&nbsp;", " ")
            .Replace("<small>", "");

        Name = Name.Trim();
        Number = Number.Trim();
        Add = Add.Trim();
    }
}
