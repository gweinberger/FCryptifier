namespace FCryptifier;

/// <summary>
/// Parses command-line arguments for the FCryptifier application.
/// Supports encryption, decryption, password input, and help options.
/// </summary>
/// <param name="args">Command-line arguments passed to the application.</param>
internal class CommandlineParser(string[] args)
{
    internal const int MinPasswordLength = 8;

    /// <summary>
    /// Contains parsed command-line options for the application.
    /// </summary>
    internal class AppOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to encrypt files.
        /// </summary>
        internal bool Encrypt { get; set; }

        /// <summary>
        /// Gets or sets the path of the input file to encrypt/decrypt.
        /// </summary>
        internal string InputFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the output file.
        /// Automatically set based on input file and operation type.
        /// </summary>
        internal string OutputFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for encryption/decryption.
        /// </summary>
        internal string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to suppress progress output.
        /// </summary>
        internal bool Silent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether help was requested.
        /// </summary>
        internal bool Help { get; set; }
    }

    /// <summary>
    /// Parses the command-line arguments and returns the parsed options.
    /// </summary>
    /// <returns>An AppOptions object containing the parsed arguments.</returns>
    /// <exception cref="ArgumentException">Thrown when required arguments are missing or invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified input file does not exist.</exception>
    internal AppOptions ParseArguments()
    {
        // Parse command-line arguments into a dictionary
        AppOptions options = new AppOptions();
        Dictionary<string, string> argDict = new Dictionary<string, string>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("-"))
            {
                // Check if next argument is a value (not another flag)
                string value = (i + 1 < args.Length && !args[i + 1].StartsWith("-")) ? args[++i] : "true";
                argDict[arg] = value;
            }
        }

        // Extract individual options from parsed arguments
        options.Encrypt = argDict.ContainsKey("-e");
        options.Silent = argDict.ContainsKey("-s");
        options.Help = argDict.ContainsKey("-h") || argDict.ContainsKey("--help");
        options.InputFile = argDict.GetValueOrDefault("-f", "");
        options.Password = argDict.GetValueOrDefault("-p", "");

        // Read password from file if specified
        string passwordFile = argDict.GetValueOrDefault("-pf", "");
        if (passwordFile != "" && new FileInfo(passwordFile).Length > 0)
            options.Password = File.ReadAllLines(passwordFile)[0]; //only first line without line break

        // Show help if requested (bypass other validations)
        if (options.Help)
        {
            return options;
        }

        // Validate required arguments
        if (options.InputFile == "" || options.Password == "")
            throw new ArgumentException("Input file and password are required.");

        // Validate password length
        if (options.Password.Length < MinPasswordLength)
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long.");

        // Validate input file exists
        if (!File.Exists(options.InputFile))
            throw new FileNotFoundException("File not found: " + options.InputFile);

        // Determine output file path based on operation type
        if (options.Encrypt)
        {
            options.OutputFile = options.InputFile + ".aes";
        }
        else
        {
            // For decryption, remove .aes extension or add _decrypted suffix
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
