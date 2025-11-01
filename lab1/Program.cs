// природне злиття,
// латинська літера (ключ) - рядок довжиною до 45 символів - номер телефону
// зростання
// доступний об'єм - 300мб

namespace Lab1
{
  public class Program
  {
    private readonly int _inputFileSizeMb = 1024;
    private readonly int _maxLimitMb = 300;
    private readonly string _filePath = Path.Combine(Environment.CurrentDirectory, "sort_data.txt");

    public void Core()
    {
      Generators generator = new(_inputFileSizeMb);

      BaseNaturalSort sorter = new(_filePath);
      /*  ModifiedNaturalSort modifiedSorter = new(_filePath, _maxLimitMb); */

      generator.GenerateInputFile(_filePath);
      /*  modifiedSorter.Sort(); */
    }

    public static void Main()
    {
      var program = new Program();
      program.Core();
    }
  }
}