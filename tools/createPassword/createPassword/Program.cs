using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class Program
{
  public static void Main(string[] args)
  {
    while (true)
    {
      Console.Write("Enter a password: ");
      string password = Console.ReadLine();

      // パスワード未入力時は終了
      if (string.IsNullOrEmpty(password))
      {
        break;
      }

      // generate a 128-bit salt using a secure PRNG
      byte[] salt = new byte[128 / 8];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(salt);
      }
      var saltBase64 = Convert.ToBase64String(salt);
      Console.WriteLine($"Salt: {saltBase64}");

      // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
      string encryptedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));
      Console.WriteLine($"Encrypted Password: {encryptedPassword}");

      Console.WriteLine("");

      // ファイル作成
      Directory.CreateDirectory("output");
      var filePath = $"output/{password}.txt";
      File.WriteAllText(filePath,$"> Encrypted Password:{Environment.NewLine}{encryptedPassword}{Environment.NewLine}> Salt:{Environment.NewLine}{saltBase64}"); 
    }
  }
}

