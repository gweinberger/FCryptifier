using System.Diagnostics;
using System.Security.Cryptography;

namespace FCryptifier;

public class Crypto(bool preventConsoleColor = false, bool propagateExceptions = false)
{
    public bool FileEncrypt(string inputFile, string outputFile, string password)
    {
        bool result = true;

        // generate Salt
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[32];
        rng.GetBytes(salt);

        FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        Aes AES = Aes.Create();
        AES.KeySize = 256;
        AES.BlockSize = 128;
        AES.Padding = PaddingMode.PKCS7;

        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA512);
        AES.Key = key.GetBytes(AES.KeySize / 8);
        AES.IV = key.GetBytes(AES.BlockSize / 8);
        AES.Mode = CipherMode.CFB;

        // write salt to the begining of the output file, so in this case can be random every time
        fsCrypt.Write(salt, 0, salt.Length);

        CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
        FileStream fsIn = new FileStream(inputFile, FileMode.Open);

        //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
        byte[] buffer = new byte[1048576];
        int read;
        try
        {
            while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
            {
                cs.Write(buffer, 0, read);
            }
            fsIn.Close();
        }
        catch (Exception ex)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            if (!preventConsoleColor) Console.ResetColor();
            result = false;
        }
        finally
        {
            cs.Close();
            fsCrypt.Close();
        }
        return result;
    }

    public bool FileDecrypt(string inputFile, string outputFile, string password)
    {
        bool result = true;
        
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] salt = new byte[32];

        FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
        fsCrypt.Read(salt, 0, salt.Length);

        Aes AES = Aes.Create();
        AES.KeySize = 256;
        AES.BlockSize = 128;

        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA512);
        AES.Key = key.GetBytes(AES.KeySize / 8);
        AES.IV = key.GetBytes(AES.BlockSize / 8);
        AES.Padding = PaddingMode.PKCS7;
        AES.Mode = CipherMode.CFB;

        CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
        FileStream fsOut = new FileStream(outputFile, FileMode.Create);

        int read;
        byte[] buffer = new byte[1048576];

        try
        {
            while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
            {
                fsOut.Write(buffer, 0, read);
            }
        }
        catch (CryptographicException ex_CryptographicException)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Debug.WriteLine(ex_CryptographicException.Message);
            Console.WriteLine($"Wrong Password");
            if (!preventConsoleColor) Console.ResetColor();
            result = false;
        }
        catch (Exception ex)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            if (!preventConsoleColor) Console.ResetColor();
            result = false;
        }
        try
        {
            cs.Close();
        }
        catch (Exception ex)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            if (!preventConsoleColor) Console.ResetColor();
            result = false;
        }
        finally
        {
            fsOut.Close();
            fsCrypt.Close();
        }
        return result;
    }
}
