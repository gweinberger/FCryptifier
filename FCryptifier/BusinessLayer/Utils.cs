using System.Reflection;

namespace FCryptifier;

internal static class Utils
{
    private const string AUTHOR = "Gerald Weinberger";
    private const string AUTHOR_EMAIL = "g.weinberger@outlook.com";
    private const string AUTHOR_WEB = "https://github.com/gweinberger/FCryptifier";
    private const int DEV_YEAR = 2025;

    internal static void printHeader()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        Console.WriteLine(Assembly.GetExecutingAssembly()?.GetName().Name + " " + Assembly.GetEntryAssembly()?.GetName().Version);
        Console.ResetColor();
        Console.WriteLine("-------------------------------------------------------------------------------");
        Console.WriteLine($"(c){(DateTime.Now.Year > DEV_YEAR ? DEV_YEAR + "-" : "")}{DateTime.Now.Year} {AUTHOR} | {AUTHOR_EMAIL}");
        Console.WriteLine($"{AUTHOR_WEB}");
        Console.WriteLine("-------------------------------------------------------------------------------");
    }

    internal static void printUsageInfo()
    {
        Console.WriteLine("");
        Console.WriteLine("Usage:");
        Console.WriteLine($"  fcryptifier -e|-d -f inputfile -p password | -pf passwordfile");
        Console.WriteLine("");
        Console.WriteLine($"  Example encryption: fcryptifier -e -f file.txt -p 1234");
        Console.WriteLine($"  Example decryption with passwordfile: fcryptifier -d -f file.txt.aes -pf pass.txt");
        Console.WriteLine("");
    }
}
