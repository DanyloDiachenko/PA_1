using System.Diagnostics;

namespace Lab1
{
  readonly struct DataLine : IComparable<DataLine>
  {
    public readonly string Key;
    public readonly string OriginalLine;
    public readonly int ApproximateSize;

    public DataLine(string line)
    {
      OriginalLine = line;
      int separatorIndex = line.IndexOf(':');

      if (separatorIndex != -1)
      {
        Key = line.Substring(0, separatorIndex);
      }
      else
      {
        Key = line;
      }

      ApproximateSize = (OriginalLine.Length * 2) + (Key.Length * 2) + 70;
    }

    public int CompareTo(DataLine other)
    {
      return string.Compare(Key, other.Key);
    }
  }

  file readonly struct MergeNode(DataLine line, int fileIndex) : IComparable<MergeNode>
  {
    public readonly DataLine Line = line;
    public readonly int FileIndex = fileIndex;

    public int CompareTo(MergeNode other)
    {
      return Line.CompareTo(other.Line);
    }
  }

  public class ModifiedNaturalSort
  {
    private readonly string _mainFilePath;
    private readonly string _tempDirectory = Path.Combine(Environment.CurrentDirectory, "sort_temp");
    private readonly long _memoryLimitBytes;
    private readonly long _actualBufferLimitBytes;

    public ModifiedNaturalSort(string mainFilePath, int memoryLimitMb = 300)
    {
      _mainFilePath = mainFilePath;
      _memoryLimitBytes = memoryLimitMb * 1024 * 1024;
      _actualBufferLimitBytes = (long)(_memoryLimitBytes * 0.9);

      if (Directory.Exists(_tempDirectory))
      {
        Directory.Delete(_tempDirectory, true);
      }
      Directory.CreateDirectory(_tempDirectory);
    }

    public void Sort()
    {
      Console.WriteLine("--- 2. Start Modified Natural Sorting ---");
      Stopwatch sw = Stopwatch.StartNew();

      Console.WriteLine($"Phase 1: Creating initial runs (RAM limit: {_actualBufferLimitBytes / 1024 / 1024}MB)...");
      List<string> runFilePaths = CreateInitialRuns();
      Console.WriteLine($"Phase 1: Created {runFilePaths.Count} sorted runs.");

      Console.WriteLine($"Phase 2: Merging {runFilePaths.Count} runs...");
      KWayMerge(runFilePaths);

      sw.Stop();
      Console.WriteLine($"--- 3. Sorting has been completed in {sw.Elapsed.TotalSeconds:F2} sec. ---");
      Cleanup();
    }

    private List<string> CreateInitialRuns()
    {
      var runFilePaths = new List<string>();
      long currentMemoryUsage = 0;
      var memoryBuffer = new List<DataLine>();

      using (var sr = new StreamReader(_mainFilePath))
      {
        string? currentLine;
        while ((currentLine = sr.ReadLine()) != null)
        {
          var dataLine = new DataLine(currentLine);
          memoryBuffer.Add(dataLine);
          currentMemoryUsage += dataLine.ApproximateSize;

          if (currentMemoryUsage >= _actualBufferLimitBytes)
          {
            FlushBuffer(memoryBuffer, runFilePaths);
            currentMemoryUsage = 0;
          }
        }

        if (memoryBuffer.Count > 0)
        {
          FlushBuffer(memoryBuffer, runFilePaths);
        }
      }
      return runFilePaths;
    }

    private void FlushBuffer(List<DataLine> buffer, List<string> filePaths)
    {
      Console.WriteLine($"  ...flushing run {filePaths.Count + 1} ({buffer.Sum(l => l.ApproximateSize) / 1024 / 1024}MB)");

      buffer.Sort();

      string newRunFile = Path.Combine(_tempDirectory, $"run_{filePaths.Count}.txt");
      filePaths.Add(newRunFile);

      using (var sw = new StreamWriter(newRunFile))
      {
        foreach (var line in buffer)
        {
          sw.WriteLine(line.OriginalLine);
        }
      }

      buffer.Clear();
    }

    private void KWayMerge(List<string> runFilePaths)
    {
      var readers = new List<StreamReader>(runFilePaths.Count);
      var priorityQueue = new PriorityQueue<MergeNode, MergeNode>();

      try
      {
        for (int i = 0; i < runFilePaths.Count; i++)
        {
          var reader = new StreamReader(runFilePaths[i]);
          readers.Add(reader);

          if (!reader.EndOfStream)
          {
            var line = new DataLine(reader.ReadLine()!);
            priorityQueue.Enqueue(new MergeNode(line, i), new MergeNode(line, i));
          }
        }

        using var sw = new StreamWriter(_mainFilePath);
        while (priorityQueue.Count > 0)
        {
          MergeNode node = priorityQueue.Dequeue();
          sw.WriteLine(node.Line.OriginalLine);

          int fileIndex = node.FileIndex;
          if (!readers[fileIndex].EndOfStream)
          {
            var nextLine = new DataLine(readers[fileIndex].ReadLine()!);
            priorityQueue.Enqueue(new MergeNode(nextLine, fileIndex), new MergeNode(nextLine, fileIndex));
          }
        }
      }
      finally
      {
        foreach (var reader in readers)
        {
          reader.Close();
        }
      }
    }

    private void Cleanup()
    {
      Console.WriteLine("Cleaning temporary files...");
      if (Directory.Exists(_tempDirectory))
      {
        Directory.Delete(_tempDirectory, true);
      }
    }
  }
}