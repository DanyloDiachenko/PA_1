using System.Diagnostics;

namespace Lab1
{
  public class ModifiedNaturalSort(string mainFilePath)
  {
    private readonly string _mainFilePath = mainFilePath;
    private readonly string _tempFilePath1 = Path.Combine(Environment.CurrentDirectory, "temp_sort_1.txt");
    private readonly string _tempFilePath2 = Path.Combine(Environment.CurrentDirectory, "temp_sort_2.txt");

    public void Sort()
    {

    }
  }
}