using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace FCryptifier;

internal class App(string[] args)
{
    [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    private static extern bool ZeroMemory(IntPtr destination, int length);

    public void Run()
    {
        try
        {
            Utils.PrintHeader();

            var appOption = new CommandlineParser.AppOptions();
            CommandlineParser parser = new(args);
            try
            {
                appOption = parser.ParseArguments();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }

            if (appOption.Help)
            {
                Utils.PrintUsageInfo();
                Environment.Exit(0);
            }

            DateTime now = DateTime.Now;
            GCHandle gch = GCHandle.Alloc(appOption.Password, GCHandleType.Pinned);  // pinning secret is more secure. see: https://stackoverflow.com/questions/20012534/in-c-why-is-pinning-a-secret-key-in-memory-more-secure
            try
            {
                Crypto cr = new Crypto();
                if (!appOption.Silent) cr.FileProgress += (read, total) => { Utils.ShowProgress(Convert.ToInt32(read * 100 / total) + 1); };
                bool isSuccessful = appOption.Encrypt ? cr.FileEncrypt(appOption.InputFile, appOption.OutputFile, appOption.Password) : cr.FileDecrypt(appOption.InputFile, appOption.OutputFile, appOption.Password);

                Console.WriteLine("");

                if (isSuccessful)
                {
                    Console.WriteLine($"{(appOption.Encrypt ? "Encrypted" : "Decrypted")}" +
                                      $" in {DateTime.Now.Subtract(now).Milliseconds}ms to: {appOption.OutputFile}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success");
                }
                else
                {
                    // Use cryptographic random for delay to prevent timing attacks
                    byte[] delayBytes = new byte[4];
                    RandomNumberGenerator.Fill(delayBytes);
                    int delayMs = Math.Abs(BitConverter.ToInt32(delayBytes, 0) % 2000) + 2000;  // 2-4 seconds random delay
                    Thread.Sleep(delayMs);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed");
                }
                Console.ResetColor();
                Environment.Exit(isSuccessful ? 0 : 1);
            }
            finally
            {
                ZeroMemory(gch.AddrOfPinnedObject(), appOption.Password.Length * 2);
                gch.Free();
                GC.Collect();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
        }
    }
}
