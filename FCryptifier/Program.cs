namespace FCryptifier;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            App app = new(args);
            app.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
        }
    }
}
