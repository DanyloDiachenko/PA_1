using System.Text;

namespace Lab1
{
  public class Generators(
   int fileSizeMb
  )
  {
    private static readonly string _latinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static readonly string _allChars = _latinLetters + "0123456789!@#$%^&*()-_=+[]{}|;:',.<>?/`~";
    private readonly int _maxKeyLength = 45;
    private readonly int _phoneNumberLength = 10;
    private readonly int _fileSizeMb = fileSizeMb;
    private readonly Random _random = new();

    private char RandomLatinLetter()
    {
      int index = _random.Next(_latinLetters.Length);

      return _latinLetters[index];
    }

    private char RandomChar()
    {
      int index = _random.Next(_allChars.Length);

      return _allChars[index];
    }

    private string RandomString()
    {
      char[] chars = new char[_maxKeyLength];
      for (int i = 0; i < _maxKeyLength; i++)
      {
        chars[i] = RandomChar();
      }

      return new string(chars);
    }

    private int RandomNumber()
    {
      return _random.Next(10);
    }

    private string RandomPhoneNumber()
    {
      char[] chars = new char[_phoneNumberLength];
      for (int i = 0; i < _phoneNumberLength; i++)
      {
        if (i == 0)
        {
          chars[i] = '+';
          continue;
        }

        chars[i] = (char)('0' + RandomNumber());
      }

      return new string(chars);
    }

    private string RandomFormattedString()
    {
      return $"{RandomLatinLetter()}-{RandomString()}-{RandomPhoneNumber()}";
    }

    public void GenerateInputFile(string filePath)
    {
      {
        Console.WriteLine($"--- 1. Generation File {_fileSizeMb} ({_fileSizeMb} Mb) ---");
        long targetBytes = _fileSizeMb * 1024 * 1024;

        int stringLength = RandomFormattedString().Length;
        int bytesPerLine = stringLength + Environment.NewLine.Length;
        long lineCount = targetBytes / bytesPerLine;

        using (StreamWriter sw = new(filePath, false, Encoding.UTF8))
        {
          for (long i = 0; i < lineCount; i++)
          {
            sw.WriteLine(RandomFormattedString());
          }
        }
        Console.WriteLine($"File Generated, {lineCount:N0} lines.");
      }

    }
  }
}