using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lab1
{
    public class AiNaturalSort(string filePath)
    {
        private readonly string _filePath = filePath;
        private readonly string _tempFileA = Path.Combine(Environment.CurrentDirectory, "ai_sort_a.txt");
        private readonly string _tempFileB = Path.Combine(Environment.CurrentDirectory, "ai_sort_b.txt");
        private readonly string _tempFileC = Path.Combine(Environment.CurrentDirectory, "ai_sort_c.txt");
        
        private const int ChunkSize = 10 * 1024 * 1024; 

        public void Sort()
        {
            try
            {
                Console.WriteLine("--- AI Natural Sort (3-Way Merge) ---");
                var sw = Stopwatch.StartNew();

                Console.WriteLine("Phase 1: Generating Initial Runs (3-Way Distribution)...");
                DistributeInitialRuns();

                int pass = 1;
                while (true)
                {
                    Console.WriteLine($"Phase 2 (Pass {pass}): Merging 3 Tapes...");
                    int runCount = MergeRuns();
                    
                    Console.WriteLine($"Pass {pass} completed. Runs created in merge: {runCount}");

                    if (runCount <= 1)
                    {
                        break;
                    }

                    Console.WriteLine($"Phase 2 (Pass {pass}): Splitting to 3 Tapes...");
                    SplitRuns();
                    pass++;
                }

                sw.Stop();
                Console.WriteLine($"--- AI Sort Completed in {sw.Elapsed.TotalSeconds:F2}s ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR in AiNaturalSort: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Cleanup();
            }
        }

        private void DistributeInitialRuns()
        {
            using var reader = new StreamReader(_filePath);
            using var writerA = new StreamWriter(_tempFileA);
            using var writerB = new StreamWriter(_tempFileB);
            using var writerC = new StreamWriter(_tempFileC);

            var writers = new[] { writerA, writerB, writerC };
            int writerIdx = 0;

            List<string> lines = new List<string>();
            long currentChunkSize = 0;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                currentChunkSize += line.Length * 2 + 50;

                if (currentChunkSize >= ChunkSize)
                {
                    WriteSortedChunk(lines, writers[writerIdx]);
                    lines.Clear();
                    currentChunkSize = 0;
                    writerIdx = (writerIdx + 1) % 3;
                    GC.Collect();
                }
            }

            if (lines.Count > 0)
            {
                WriteSortedChunk(lines, writers[writerIdx]);
            }
        }

        private void WriteSortedChunk(List<string> lines, StreamWriter writer)
        {
            lines.Sort(StringComparer.Ordinal);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        private int MergeRuns()
        {
            using var writer = new StreamWriter(_filePath);
            using var readerA = new StreamReader(_tempFileA);
            using var readerB = new StreamReader(_tempFileB);
            using var readerC = new StreamReader(_tempFileC);

            string? lineA = readerA.ReadLine();
            string? lineB = readerB.ReadLine();
            string? lineC = readerC.ReadLine();

            string? prevA = null;
            string? prevB = null;
            string? prevC = null;

            int runCount = 0;
            bool insideRun = false;

            while (lineA != null || lineB != null || lineC != null)
            {
                bool activeA = lineA != null && (prevA == null || string.CompareOrdinal(lineA, prevA) >= 0);
                bool activeB = lineB != null && (prevB == null || string.CompareOrdinal(lineB, prevB) >= 0);
                bool activeC = lineC != null && (prevC == null || string.CompareOrdinal(lineC, prevC) >= 0);

                if (!activeA && !activeB && !activeC)
                {
                    prevA = null; prevB = null; prevC = null;
                    activeA = lineA != null;
                    activeB = lineB != null;
                    activeC = lineC != null;
                    
                    if (insideRun) 
                    {
                        runCount++;
                        insideRun = false;
                    }
                }

                if (!activeA && !activeB && !activeC)
                {
                    break;
                }

                insideRun = true;

                string? minLine = null;
                int source = -1; 

                if (activeA)
                {
                    minLine = lineA;
                    source = 0;
                }

                if (activeB)
                {
                    if (minLine == null || string.CompareOrdinal(lineB, minLine) < 0)
                    {
                        minLine = lineB;
                        source = 1;
                    }
                }

                if (activeC)
                {
                    if (minLine == null || string.CompareOrdinal(lineC, minLine) < 0)
                    {
                        minLine = lineC;
                        source = 2;
                    }
                }

                if (source != -1 && minLine != null)
                {
                    writer.WriteLine(minLine);

                    if (source == 0) { prevA = lineA; lineA = readerA.ReadLine(); }
                    else if (source == 1) { prevB = lineB; lineB = readerB.ReadLine(); }
                    else { prevC = lineC; lineC = readerC.ReadLine(); }
                }
            }
            
            if (insideRun)
            {
                runCount++;
            }

            return runCount;
        }

        private void SplitRuns()
        {
            using var reader = new StreamReader(_filePath);
            using var writerA = new StreamWriter(_tempFileA);
            using var writerB = new StreamWriter(_tempFileB);
            using var writerC = new StreamWriter(_tempFileC);

            var writers = new[] { writerA, writerB, writerC };
            int writerIdx = 0;

            string? prev = null;
            string? curr;

            while ((curr = reader.ReadLine()) != null)
            {
                if (prev != null && string.CompareOrdinal(curr, prev) < 0)
                {
                    writerIdx = (writerIdx + 1) % 3;
                }
                writers[writerIdx].WriteLine(curr);
                prev = curr;
            }
        }

        private void Cleanup()
        {
            try { if (File.Exists(_tempFileA)) File.Delete(_tempFileA); } catch { }
            try { if (File.Exists(_tempFileB)) File.Delete(_tempFileB); } catch { }
            try { if (File.Exists(_tempFileC)) File.Delete(_tempFileC); } catch { }
        }
    }
}
