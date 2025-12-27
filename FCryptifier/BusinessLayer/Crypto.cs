using System.Diagnostics;
using System.Security.Cryptography;

namespace FCryptifier;

public class Crypto(bool preventConsoleColor = false, bool propagateExceptions = false)
{
    public event Action<long, long>? FileProgress;
    private void RaiseFileProgress(long read, long total) => FileProgress?.Invoke(read, total);
    
    public bool FileEncrypt(string inputFile, string outputFile, string password)
    {
        bool result = true;
        long totalBytes = new FileInfo(inputFile).Length;
        long processedBytes = 0;

        // generate Salt
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[32];
        rng.GetBytes(salt);

        FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.PKCS7;

        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA512);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);
        aes.Mode = CipherMode.CFB;

        // write salt to the beginning of the output file, so in this case can be random every time
        fsCrypt.Write(salt, 0, salt.Length);

        CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);
        FileStream fsIn = new FileStream(inputFile, FileMode.Open);

        //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
        byte[] buffer = new byte[1048576];
        try
        {
            int read;
            while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
            {
                cs.Write(buffer, 0, read);
                processedBytes += read;
                RaiseFileProgress(processedBytes, totalBytes);
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
        long totalBytes = new FileInfo(inputFile).Length;
        long processedBytes = 0;

        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] salt = new byte[32];

        FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
        fsCrypt.ReadExactly(salt, 0, salt.Length);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;

        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA512);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CFB;

        CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);
        FileStream fsOut = new FileStream(outputFile, FileMode.Create);

        byte[] buffer = new byte[1048576];

        try
        {
            int read;
            while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
            {
                fsOut.Write(buffer, 0, read);
                processedBytes += read;
                RaiseFileProgress(processedBytes, totalBytes);
            }
        }
        catch (CryptographicException exCryptographicException)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Debug.WriteLine(exCryptographicException.Message);
            Console.WriteLine("");
            Console.WriteLine($"Wrong Password");
            if (!preventConsoleColor) Console.ResetColor();
            result = false;
        }
        catch (Exception ex)
        {
            if (propagateExceptions) throw;
            if (!preventConsoleColor) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("");
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
            Console.WriteLine("");
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
