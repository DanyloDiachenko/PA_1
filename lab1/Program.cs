// природне злиття,
// латинська літера (ключ) - рядок довжиною до 45 символів - номер телефону
// зростання
// доступний об'єм - 300мб

namespace Lab1
{
  public class Program
  {
    private readonly int _inputFileSizeMb = 1024;
    private readonly int _memoryLimitMb = 300;
    private readonly string _filePath = Path.Combine(Environment.CurrentDirectory, "sort_data.txt");

    public void Core()
    {
      Generators generator = new(_inputFileSizeMb);
      BaseNaturalSort baseSorter = new(_filePath);
      ModifiedNaturalSort modifiedSorter = new(_filePath, _memoryLimitMb);
      AiNaturalSort aiSorter = new(_filePath);

      generator.GenerateInputFile(_filePath);
      modifiedSorter.Sort();
      
      /* Console.WriteLine("--- Verifying Sort Order ---");
      bool isSorted = true;
      using (var sr = new StreamReader(_filePath))
      {
          string? previousLine = null;
          string? currentLine;
          long lineNum = 0;
          while ((currentLine = sr.ReadLine()) != null)
          {
              lineNum++;
              if (previousLine != null && string.CompareOrdinal(currentLine, previousLine) < 0)
              {
                  Console.WriteLine($"Sort Error at line {lineNum}:");
                  Console.WriteLine($"Previous: {previousLine}");
                  Console.WriteLine($"Current:  {currentLine}");
                  isSorted = false;
                  break;
              }
              previousLine = currentLine;
          }
      }
      
      if (isSorted)
      {
          Console.WriteLine("Verification Passed: File is sorted.");
      }
      else
      {
          Console.WriteLine("Verification Failed.");
      } */
   }

    public static void Main()
    {
      var program = new Program();
      program.Core();
    }
  }
}