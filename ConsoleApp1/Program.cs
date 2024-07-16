namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main()
        {
            // Парсинг (пример)
            Bull[]? result = await Parser.GetBullsAsync(new ConsolePrinter(20));
        }
    }
}
