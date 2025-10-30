namespace Lab1
{
  public class Generators
  {
    private readonly string _latinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly string _allChars;
    private readonly int _maxKeyLength = 45;
    private readonly int _phoneNumberLength = 10;
    private readonly Random _random = new();

    public Generators()
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

    public string RandomFormattedString()
    {
      return $"{RandomLatinLetter()}-{RandomString()}-{RandomPhoneNumber()}";
    }
  }
}