using System.Runtime.InteropServices;

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
            catch
            {
                Utils.PrintUsageInfo();
                Environment.Exit(1);
            }

            DateTime now = DateTime.Now;
            GCHandle gch = GCHandle.Alloc(appOption.Password, GCHandleType.Pinned);  // pinning secret is more secure. see: https://stackoverflow.com/questions/20012534/in-c-why-is-pinning-a-secret-key-in-memory-more-secure
            Crypto cr = new Crypto();
            bool isSuccessful = appOption.Encrypt ? cr.FileEncrypt(appOption.InputFile, appOption.OutputFile, appOption.Password) : cr.FileDecrypt(appOption.InputFile, appOption.OutputFile, appOption.Password);
            ZeroMemory(gch.AddrOfPinnedObject(), appOption.Password.Length * 2);
            gch.Free();
            GC.Collect();

            if (isSuccessful)
            {
                Console.WriteLine($"{(appOption.Encrypt ? "Encrypted" : "Decrypted")} in {DateTime.Now.Subtract(now).Milliseconds}ms to: {appOption.OutputFile}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success");
                Environment.Exit(0);
            }
            else
            {
                Thread.Sleep(new Random().Next(2, 4) * 1000);  // prevent fast Brute Force
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed");
                Environment.Exit(1);
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
        }
    }
}
