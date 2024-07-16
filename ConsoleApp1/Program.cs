namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main()
        {
            // Парсинг (пример)
            // Плохой пример, лучше создавать таск, работа продолжительная (очень)
            Bull[]? result = await Parser.GetBullsAsync(new ConsolePrinter(20));
        }
    }
}
