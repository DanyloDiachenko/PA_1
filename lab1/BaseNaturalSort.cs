using System.Diagnostics;

namespace Lab1
{
  public class BaseNaturalSort(string mainFilePath)
  {
    private readonly string _mainFilePath = mainFilePath;
    private readonly string _tempFilePath1 = Path.Combine(Environment.CurrentDirectory, "temp_sort_1.txt");
    private readonly string _tempFilePath2 = Path.Combine(Environment.CurrentDirectory, "temp_sort_2.txt");

    public void Sort()
    {
      Console.WriteLine("--- 2. Start Sorting ---");
      Stopwatch sw = Stopwatch.StartNew();

      int passNumber = 1;
      while (true)
      {
        int runs = SplitRuns();
        Console.WriteLine($"Pass {passNumber}: Splitting has end, found runs: {runs}");

        if (runs <= 1)
        {
          break;
        }

        Console.WriteLine($"Pass {passNumber}: Merging runs...");
        MergeRuns();

        passNumber++;
      }

      sw.Stop();
      Console.WriteLine($"--- 3. Sorting has been completed in {sw.Elapsed.TotalSeconds:F2} sec. ---");
      Cleanup();
    }

    private int SplitRuns()
    {
      int runCount = 0;

      using (var sr = new StreamReader(_mainFilePath))
      using (var sw1 = new StreamWriter(_tempFilePath1))
      using (var sw2 = new StreamWriter(_tempFilePath2))
      {
        StreamWriter currentWriter = sw1;
        string? previousLine = null;
        string? currentLine;

        while ((currentLine = sr.ReadLine()) != null)
        {
          if (previousLine == null)
          {
            runCount = 1;
          }
          else if (string.Compare(currentLine, previousLine) < 0)
          {
            currentWriter = (currentWriter == sw1) ? sw2 : sw1;
            runCount++;
          }

          currentWriter.WriteLine(currentLine);
          previousLine = currentLine;
        }
      }
      return runCount;
    }

    private void MergeRuns()
    {
      using var sw = new StreamWriter(_mainFilePath);
      using var sr1 = new StreamReader(_tempFilePath1);
      using var sr2 = new StreamReader(_tempFilePath2);

      string? line1 = sr1.ReadLine();
      string? line2 = sr2.ReadLine();

      string? prev1 = null;
      string? prev2 = null;

      while (line1 != null || line2 != null)
      {
        bool run1Active = line1 != null && (prev1 == null || string.Compare(line1, prev1) >= 0);
        bool run2Active = line2 != null && (prev2 == null || string.Compare(line2, prev2) >= 0);

        if (!run1Active && !run2Active)
        {
          prev1 = null;
          prev2 = null;
        }

        run1Active = line1 != null && (prev1 == null || string.Compare(line1, prev1) >= 0);
        run2Active = line2 != null && (prev2 == null || string.Compare(line2, prev2) >= 0);

        if (run1Active && run2Active)
        {
          if (string.Compare(line1, line2) <= 0)
          {
            sw.WriteLine(line1);
            prev1 = line1;
            line1 = sr1.ReadLine();
          }
          else
          {
            sw.WriteLine(line2);
            prev2 = line2;
            line2 = sr2.ReadLine();
          }
        }
        else if (run1Active)
        {
          sw.WriteLine(line1);
          prev1 = line1;
          line1 = sr1.ReadLine();
        }
        else if (run2Active)
        {
          sw.WriteLine(line2);
          prev2 = line2;
          line2 = sr2.ReadLine();
        }
        else
        {
          prev1 = null;
          prev2 = null;
        }
      }
    }

    private void Cleanup()
    {
      Console.WriteLine("Cleaning temporary files...");
      File.Delete(_tempFilePath1);
      File.Delete(_tempFilePath2);
    }
  }
}