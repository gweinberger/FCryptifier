using System.Diagnostics;
using System.Security.Cryptography;

namespace FCryptifier;

/// <summary>
/// Provides AES encryption and decryption functionality for files.
/// Uses AES-256-CFB with PBKDF2-SHA512 key derivation.
/// </summary>
/// <param name="preventConsoleColor">If true, prevents console color output (useful for GUI). Defaults to false.</param>
/// <param name="propagateExceptions">If true, exceptions are re-thrown instead of being handled internally. Defaults to false.</param>
public class Crypto(bool preventConsoleColor = false, bool propagateExceptions = false)
{
    /// <summary>
    /// Event raised during file operations to report progress.
    /// </summary>
    public event Action<long, long>? FileProgress;

    /// <summary>
    /// Raises the FileProgress event with current progress information.
    /// </summary>
    /// <param name="read">Number of bytes read so far.</param>
    /// <param name="total">Total number of bytes to process.</param>
    private void RaiseFileProgress(long read, long total) => FileProgress?.Invoke(read, total);

    /// <summary>
    /// Encrypts a file using AES-256-CFB encryption.
    /// Generates a random salt for each encryption operation.
    /// The salt is prepended to the output file (first 32 bytes).
    /// </summary>
    /// <param name="inputFile">Path to the file to encrypt.</param>
    /// <param name="outputFile">Path where the encrypted file will be saved (will be overwritten if exists).</param>
    /// <param name="password">Password used for encryption.</param>
    /// <returns>True if encryption was successful; otherwise false.</returns>
    /// <exception cref="CryptographicException">Thrown when encryption fails due to cryptographic errors.</exception>
    public bool FileEncrypt(string inputFile, string outputFile, string password)
    {
        bool result = true;
        long totalBytes = new FileInfo(inputFile).Length;
        long processedBytes = 0;

        // generate Salt
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[32];
        rng.GetBytes(salt);

        using FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.PKCS7;

        // Generate key and IV from password using PBKDF2 with SHA512 (50k iterations)
        // Key (32 bytes) + IV (16 bytes) = 48 bytes
        byte[] keyAndIv = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, 50000, HashAlgorithmName.SHA512, 48);
        aes.Key = keyAndIv.AsSpan(0, 32).ToArray();
        aes.IV = keyAndIv.AsSpan(32, 16).ToArray();
        aes.Mode = CipherMode.CFB;

        // write salt to the beginning of the output file, so in this case can be random every time
        fsCrypt.Write(salt, 0, salt.Length);

        using FileStream fsIn = new FileStream(inputFile, FileMode.Open);
        CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);

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

    /// <summary>
    /// Decrypts a file using AES-256-CFB encryption.
    /// Reads the salt from the first 32 bytes of the input file.
    /// </summary>
    /// <param name="inputFile">Path to the encrypted file (.aes).</param>
    /// <param name="outputFile">Path where the decrypted file will be saved (will be overwritten if exists).</param>
    /// <param name="password">Password used for decryption.</param>
    /// <returns>True if decryption was successful; otherwise false.</returns>
    /// <exception cref="CryptographicException">Thrown when decryption fails (e.g., wrong password).</exception>
    public bool FileDecrypt(string inputFile, string outputFile, string password)
    {
        bool result = true;
        long totalBytes = new FileInfo(inputFile).Length;
        long processedBytes = 0;

        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] salt = new byte[32];

        using FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
        fsCrypt.ReadExactly(salt, 0, salt.Length);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CFB;

        // Generate key and IV from password using PBKDF2 with SHA512 (50k iterations)
        // Key (32 bytes) + IV (16 bytes) = 48 bytes
        byte[] keyAndIv = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, 50000, HashAlgorithmName.SHA512, 48);
        aes.Key = keyAndIv.AsSpan(0, 32).ToArray();
        aes.IV = keyAndIv.AsSpan(32, 16).ToArray();

        using FileStream fsOut = new FileStream(outputFile, FileMode.Create);
        CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);

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
        finally
        {
            cs.Close();
        }
        return result;
    }
}
