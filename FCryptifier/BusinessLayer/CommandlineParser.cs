namespace FCryptifier;

internal class CommandlineParser(string[] args)
{
    internal class AppOptions
    {
        internal bool Encrypt { get; set; }
        internal string InputFile { get; set; } = string.Empty;
        internal string OutputFile { get; set; } = string.Empty;
        internal string Password { get; set; } = string.Empty;
    }

    internal AppOptions ParseArguments()
    {
        AppOptions options = new AppOptions();
        Dictionary<string, string> argDict = new Dictionary<string, string>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("-"))
            {
                string value = (i + 1 < args.Length && !args[i + 1].StartsWith("-")) ? args[++i] : "true";
                argDict[arg] = value;
            }
        }

        options.Encrypt = argDict.ContainsKey("-e");
        options.InputFile = argDict.GetValueOrDefault("-f", "");
        options.Password = argDict.GetValueOrDefault("-p", "");

        string passwordFile = argDict.GetValueOrDefault("-pf", "");
        if (passwordFile != "") options.Password = File.ReadAllText(passwordFile);

        if (options.InputFile == "" || options.Password == "")
            throw new ArgumentException();

        if (!File.Exists(options.InputFile))
            throw new FileNotFoundException("File not found: " + options.InputFile);

        if (options.Encrypt)
        { 
            options.OutputFile = options.InputFile + ".aes";
        }
        else
        {
            if (Path.GetExtension(options.InputFile).ToLower() == ".aes")
            {
                options.OutputFile = options.InputFile.Substring(0, options.InputFile.LastIndexOf("."));
            }
            else
            {
                options.OutputFile = options.InputFile + "_decrypted";
            }
        }

        return options;
    }
}
