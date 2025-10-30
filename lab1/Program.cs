// природне злиття,
// латинська літера (ключ) - рядок довжиною до 45 символів - номер телефону
// зростання
// доступний об'єм - 300мб

using System;

namespace Lab1
{
  public class Program
  {
    private readonly string _latinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly string _allChars;
    private readonly int _maxKeyLength = 45;
    private readonly int _phoneNumberLength = 10;
    private readonly string _inputFilePath = "";
    private readonly string _outputFilePath = "";
    private readonly int _inputFileSizeMb = 10;
    private readonly Random _random = new();

    public Program()
    {
      _allChars = _latinLetters + "0123456789!@#$%^&*()-_=+[]{}|;:',.<>?/`~";
    }

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

    public void Core()
    {
      for (int i = 0; i < 5; i++)
      {
        Console.WriteLine($"{RandomLatinLetter()}-{RandomString()}-{RandomPhoneNumber()}");
      }
    }

    public static void Main()
    {
      var program = new Program();
      program.Core();
    }

    // Add a static entry point expected by the runtime.
    // This keeps the existing instance Main logic and calls into it.
    /* public static int Main(string[] args)
    {
      var program = new Program();
      program.Main();
      return 0;
    } */
  }
}