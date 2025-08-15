using System.Reflection;

namespace FCryptifier;

internal static class Utils
{
    private const string Author = "Gerald Weinberger";
    private const string AuthorEmail = "g.weinberger@outlook.com";
    private const string AuthorWeb = "https://github.com/gweinberger/FCryptifier";
    private const int DevYear = 2025;

    internal static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        Console.WriteLine(Assembly.GetExecutingAssembly()?.GetName().Name + " " + Assembly.GetEntryAssembly()?.GetName().Version);
        Console.ResetColor();
        Console.WriteLine("-------------------------------------------------------------------------------");
        Console.WriteLine($"(c){(DateTime.Now.Year > DevYear ? DevYear + "-" : "")}{DateTime.Now.Year} {Author} | {AuthorEmail}");
        Console.WriteLine($"{AuthorWeb}");
        Console.WriteLine("-------------------------------------------------------------------------------");
    }

    internal static void PrintUsageInfo()
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
