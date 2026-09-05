using System.CommandLine;
using System.Text;
using Assembler;
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

    var isEligibleInput = inputFile is not null && inputFile.Exists && inputFile.Extension.Equals(Constants.FileExtensions.ASM, StringComparison.OrdinalIgnoreCase);
    if (!isEligibleInput)
    {
        Console.WriteLine($"Invalid input file. Please provide a valid {Constants.FileExtensions.ASM} file.");
        return 1;
    }

    ParseAssemblyFile(inputFile!, outputFileName is null ? null : new FileInfo(outputFileName)); // Library will handle missing file argument, so we can safely use the null-forgiving operator here.
    return 0;
}

void ParseAssemblyFile(FileInfo inputFile, FileInfo? outputFile)
{
    var parser = new Parser(File.ReadLines(inputFile.FullName));
    var outputFilePath = outputFile is null || string.IsNullOrWhiteSpace(outputFile.FullName)
        ? Path.ChangeExtension(inputFile.FullName, Constants.FileExtensions.HACK)
        : Path.ChangeExtension(outputFile.FullName, Constants.FileExtensions.HACK);

    var writer = new Lazy<StreamWriter>(() => new StreamWriter(outputFilePath));
    var sb = new StringBuilder();
    try
    {
        while (parser.HasMoreCommands())
        {
            parser.Advance();
            var parsedCommands = GetCommands(parser);

            if (parser.CommandType() == Type.A_COMMAND)
            {
                sb.Append(Constants.HackLexemes.A_MSB);
            }
            else if (parser.CommandType() == Type.C_COMMAND)
            {
                sb.Append(Constants.HackLexemes.C_MSB);
            }

            var binaryText = ConvertCommandsToBinary(parsedCommands, sb);
            writer.Value.WriteLine($"{binaryText}");
            sb.Clear();
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

IReadOnlyCollection<KeyValuePair<string, string>> GetCommands(Parser parser)
{
    var parsedResults = parser.CommandType() switch
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
    return parsedResults;
}

string ConvertCommandsToBinary(IReadOnlyCollection<KeyValuePair<string, string>> parsedCommands, StringBuilder sb)
{
    // binary: comp-dest-jump
    var binaryCompOrder = new List<string> { "comp", "dest", "jump" };
    foreach (var kvp in parsedCommands.OrderBy(k => binaryCompOrder.IndexOf(k.Key)))
    {
        var binaryTextValue = kvp switch
        {
            { Key: "symbol", Value: var symbol } => symbol, //throw new NotImplementedException(),
            { Key: "dest", Value: var dest } => Code.Dest(dest),
            { Key: "comp", Value: var comp } => Code.Comp(comp),
            { Key: "jump", Value: var jump } => Code.Jump(jump),

            _ => throw new InvalidOperationException($"Unexpected key-value pair: {kvp.Key}={kvp.Value}")
        };

        sb.Append(binaryTextValue);
    }

    return sb.ToString().TrimEnd(',', ' ');
}