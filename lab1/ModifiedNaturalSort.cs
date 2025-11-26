using System.Diagnostics;

namespace Lab1
{
    public class ModifiedNaturalSort(string mainFilePath, int memoryLimitMb)
    {
        private readonly string _mainFilePath = mainFilePath;
        private readonly int _memoryLimitMb = memoryLimitMb;
        private readonly string _tempFilePath1 = Path.Combine(Environment.CurrentDirectory, "temp_sort_1.txt");
        private readonly string _tempFilePath2 = Path.Combine(Environment.CurrentDirectory, "temp_sort_2.txt");
        private readonly string _initialRunPath = Path.Combine(Environment.CurrentDirectory, "initial_runs.txt");

        public void Sort()
        {
            Console.WriteLine("--- Modified Natural Sort Started ---");
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine("Phase 1: Generating Initial Sorted Runs...");
            CreateInitialRuns();

            Console.WriteLine("Phase 2: Merging Runs...");
            int passNumber = 1;
            while (true)
            {
                int runs = SplitRuns();
                Console.WriteLine($"Pass {passNumber}: Split into {runs} runs.");

                if (runs <= 1)
                {
                    break;
                }

                MergeRuns();
                passNumber++;
            }

            sw.Stop();
            Console.WriteLine($"--- Sorting completed in {sw.Elapsed.TotalSeconds:F2} sec. ---");
            Cleanup();
        }

        private void CreateInitialRuns()
        {
            const int chunkSize = 50 * 1024 * 1024; 
            
            using (var sr = new StreamReader(_mainFilePath))
            using (var sw = new StreamWriter(_initialRunPath))
            {
                List<string> lines = new List<string>();
                long currentChunkSize = 0;
                string? line;

                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                    currentChunkSize += line.Length * 2 + 50; 

                    if (currentChunkSize >= chunkSize) 
                    {
                        WriteSortedChunk(lines, sw);
                        lines.Clear();
                        currentChunkSize = 0;
                        GC.Collect(); 
                    }
                }

                if (lines.Count > 0)
                {
                    WriteSortedChunk(lines, sw);
                }
            }

            File.Move(_initialRunPath, _mainFilePath, true);
        }

        private void WriteSortedChunk(List<string> lines, StreamWriter sw)
        {
            lines.Sort(StringComparer.Ordinal);
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
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
                    else if (string.CompareOrdinal(currentLine, previousLine) < 0)
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
                bool run1Active = line1 != null && (prev1 == null || string.CompareOrdinal(line1, prev1) >= 0);
                bool run2Active = line2 != null && (prev2 == null || string.CompareOrdinal(line2, prev2) >= 0);

                if (!run1Active && !run2Active)
                {
                    prev1 = null;
                    prev2 = null;
                    run1Active = line1 != null;
                    run2Active = line2 != null;
                }

                if (run1Active && run2Active)
                {
                    if (string.CompareOrdinal(line1, line2) <= 0)
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
            }
        }

        private void Cleanup()
        {
            if (File.Exists(_tempFilePath1)) File.Delete(_tempFilePath1);
            if (File.Exists(_tempFilePath2)) File.Delete(_tempFilePath2);
            if (File.Exists(_initialRunPath)) File.Delete(_initialRunPath);
        }
    }
}