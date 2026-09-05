using System.CommandLine;
using System.Globalization;
using Assembler.Modules;

var fileArgument = new Argument<FileInfo>(name: "file");
var outputFileOption = new Option<string>(
    name: "--output",
    aliases: ["-o"]);

var rootCommand = new RootCommand("Assembler for Hack assembly language.")
{
    fileArgument,
    outputFileOption
};

rootCommand.SetAction(ParseResultHandler);
var parseResult = rootCommand.Parse(args);
parseResult.Invoke();

int ParseResultHandler(ParseResult result)
{
    var inputFile = result.GetValue(fileArgument);
    var outputFileName = result.GetValue(outputFileOption);

    var isEligibleInput = inputFile is not null && inputFile.Exists && inputFile.Extension.Equals(Constants.ASM, StringComparison.OrdinalIgnoreCase);
    if (!isEligibleInput)
    {
        Console.WriteLine($"Invalid input file. Please provide a valid {Constants.ASM} file.");
        return 1;
    }

    ParseAssemblyFile(inputFile!, outputFileName is null ? null : new FileInfo(outputFileName)); // Library will handle missing file argument, so we can safely use the null-forgiving operator here.
    return 0;
}

void ParseAssemblyFile(FileInfo inputFile, FileInfo? outputFile)
{
    var parser = new Parser(File.ReadLines(inputFile.FullName));
    var outputFilePath = outputFile is null || string.IsNullOrWhiteSpace(outputFile.FullName)
        ? Path.ChangeExtension(inputFile.FullName, Constants.HACK)
        : Path.ChangeExtension(outputFile.FullName, Constants.HACK);

    var writer = new Lazy<StreamWriter>(() => new StreamWriter(outputFilePath));
    try
    {
        while (parser.HasMoreCommands())
        {
            parser.Advance();

            var parsedCommands = parser.CommandType() switch
            {
                Type.A_COMMAND or Type.L_COMMAND => new[]
                {
                    new KeyValuePair<string, string>("symbol", parser.Symbol())
                },
                Type.C_COMMAND => new[]
                {
                    new KeyValuePair<string, string>("dest", parser.Dest()),
                    new KeyValuePair<string, string>("comp", parser.Comp()),
                    new KeyValuePair<string, string>("jump", parser.Jump())
                },
                _ => throw new InvalidOperationException($"Unsupported command type: {parser.CommandType()}")
            };

            // var parsedLine = parser.CurrentLine;
            // var isValidLine = int.TryParse(parsedLine, NumberStyles.BinaryNumber, CultureInfo.InvariantCulture, out _);
            // if (!isValidLine)
            // {
            //     throw new Assembler.ParseException($"Invalid line format: {parsedLine}");
            // }
            writer.Value.WriteLine($"{parsedCommands.Aggregate(string.Empty, (acc, kvp) => $"{acc}{kvp.Key}: {kvp.Value}, ")}");
        }
    }
    finally
    {
        if (writer.IsValueCreated)
        {
            writer.Value.Dispose();
        }
    }
}
