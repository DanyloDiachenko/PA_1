// природне злиття,
// латинська літера (ключ) - рядок довжиною до 45 символів - номер телефону
// зростання
// доступний об'єм - 300мб

using System;

namespace Lab1
{
  public class Program
  {
    private readonly string _inputFilePath = "";
    private readonly string _outputFilePath = "";
    private readonly int _inputFileSizeMb = 10;
    private readonly Generators _generators = new();

    public void Core()
    {
      for (int i = 0; i < 5; i++)
      {
        Console.WriteLine(_generators.RandomFormattedString());
      }
    }

    public static void Main()
    {
      var program = new Program();
      program.Core();
    }
  }
}