using System.Reflection;

namespace FCryptifier;

/// <summary>
/// Provides utility methods for the FCryptifier application.
/// Includes header printing, usage information, and progress display.
/// </summary>
internal static class Utils
{
    private const string Author = "Gerald Weinberger";
    private const string AuthorEmail = "g.weinberger@outlook.com";
    private const string AuthorWeb = "https://github.com/gweinberger/FCryptifier";
    private const int DevYear = 2025;

    /// <summary>
    /// Prints the application header with version, author, and website information.
    /// The header includes a magenta-colored title and copyright notice.
    /// </summary>
    internal static void PrintHeader()
    {
        // Print header in magenta color
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("");
        // Get version from assembly metadata
        string version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
        Console.WriteLine($"FCryptifier");
        Console.ResetColor();
        Console.WriteLine("-------------------------------------------------------------------------------");
        Console.WriteLine($"(c){(DateTime.Now.Year > DevYear ? DevYear + "-" : "")}{DateTime.Now.Year} {Author} | {AuthorEmail}");
        Console.WriteLine($"Version: {version}");
        Console.WriteLine($"{AuthorWeb}");
        Console.WriteLine("-------------------------------------------------------------------------------");
    }

    /// <summary>
    /// Prints the application usage information with available options and examples.
    /// This method is called when the user requests help or provides invalid arguments.
    /// </summary>
    internal static void PrintUsageInfo()
    {
        Console.WriteLine("");
        Console.WriteLine("Usage:");
        Console.WriteLine("  fcryptifier [options]");
        Console.WriteLine("");
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help        Show this help message and exit.");
        Console.WriteLine("  -e                Encrypt the specified file.");
        Console.WriteLine("  -d                Decrypt the specified file.");
        Console.WriteLine("  -f <file>         Input file to encrypt/decrypt.");
        Console.WriteLine("  -p <password>     Password for encryption/decryption.");
        Console.WriteLine("  -pf <file>        File containing the password (first line only).");
        Console.WriteLine("  -s                Silent mode (no progress bar).");
        Console.WriteLine("");
        Console.WriteLine("Examples:");
        Console.WriteLine("  fcryptifier -e -f file.txt -p 1234");
        Console.WriteLine("  fcryptifier -d -f file.txt.aes -p mypassword");
        Console.WriteLine("  fcryptifier -e -f file.txt -pf password.txt");
        Console.WriteLine("");
        Console.WriteLine("For more information, visit:");
        Console.WriteLine("  https://github.com/gweinberger/FCryptifier");
        Console.WriteLine("");
    }

    /// <summary>
    /// Displays a progress bar in the console for file operations.
    /// Updates in-place using carriage return to show real-time progress.
    /// </summary>
    /// <param name="percent">Percentage of completion (0-100). Values outside this range are clamped.</param>
    internal static void ShowProgress(int percent)
    {
        percent = percent > 100 ? 100 : percent;
        percent = percent < 0 ? 0 : percent;

        int width = 40;
        int filled = percent * width / 100;

        Console.Write("\r[");
        Console.Write(new string('#', filled));
        Console.Write(new string(' ', width - filled));
        Console.Write($"] {percent}%");
    }
}
