// Тестовый консольный принтер, не несет никакой смысловой нагрузки, кроме как для примера
// исполнение соответствующее =)
public class ConsolePrinter : IPrintable
{
    int _width = 1;
    public ConsolePrinter(int width = 1)
    {
        _width = width;
    }
    public void Print(Bull bull)
    {
        Console.Write($"{bull.Name}");
        int temp_width = _width - bull.Name.Length;
        temp_width = (temp_width < 0) ? 1 : temp_width;
        for (int i = 0; i < temp_width; i++)
        {
            Console.Write(" ");
        }
        Console.Write($"{bull.Number}");
        temp_width = _width - bull.Number.Length;
        temp_width = (temp_width < 0) ? 1 : temp_width;
        for (int i = 0; i < temp_width; i++)
        {
            Console.Write(" ");
        }
        Console.WriteLine(bull.Add);
    }
}
