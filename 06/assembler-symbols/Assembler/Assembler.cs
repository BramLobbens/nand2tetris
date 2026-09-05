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
    var symbolTable = SymbolTable.Instance;

    var outputFilePath = outputFile is null || string.IsNullOrWhiteSpace(outputFile.FullName)
        ? Path.ChangeExtension(inputFile.FullName, Constants.FileExtensions.HACK)
        : Path.ChangeExtension(outputFile.FullName, Constants.FileExtensions.HACK);

    var writer = new Lazy<StreamWriter>(() => new StreamWriter(outputFilePath));
    var sb = new StringBuilder();
    try
    {
        BuildSymbolTable(parser, symbolTable);
        parser.Reset();

        // SECOND PASS: Handle A_COMMANDs and C_COMMANDs, converting them to binary and writing to the output file.
        while (parser.HasMoreCommands())
        {
            parser.Advance();
            var (type, parsedCommands) = GetParsedCommands(parser);

            switch (type)
            {
                case Type.A_COMMAND:
                    // Update our parsedCommands to replace the symbol with its corresponding address or constant value.
                    ResolveSymbolValues(symbolTable, parsedCommands);
                    sb.Append(Constants.HackLexemes.A_MSB);
                    break;

                case Type.C_COMMAND:
                    sb.Append(Constants.HackLexemes.C_MSB);
                    break;

                case Type.L_COMMAND:
                    // L_COMMANDs are labels and do not produce binary output, so we skip writing them to the output file.
                    continue;

                default:
                    throw new InvalidOperationException($"Unsupported command type: {type}");
            }

            var binaryText = ConvertCommandsToBinary(symbolTable, parsedCommands, sb);
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

/// <summary>
/// Builds the symbol table by performing a first pass over the assembly code to handle L_COMMANDs (i.e., (Xxx) labels)
/// and populate the symbol table with label addresses at the corresponding ROM addresses.
/// </summary>
void BuildSymbolTable(Parser parser, ISymbolTable symbolTable)
{
    int romAddress = 0;
    // FIRST PASS: Handle L_COMMANDs and populate the symbol table with label addresses.
    while (parser.HasMoreCommands())
    {
        parser.Advance();
        if (parser.CommandType() == Type.L_COMMAND)
        {
            // Store the address of the next instruction (ROM address) in the symbol table for the label.
            symbolTable.AddEntry(parser.Symbol(), romAddress);
        }
        else
        {
            // Increment the ROM address for each command that produces binary output (A_COMMAND and C_COMMAND).
            romAddress++;
        }
    }
}

/// <summary>
/// Resolves the symbol values in the parsed commands, updating them to either a constant or an
/// address based on the symbol table and the nature of the symbol (numeric or symbolic).
/// e.g., @LOOP -> existing label: resolve from symbol table that stored '(LOOP)' at ROM address 4,
///       @sum -> new user-assigned variable: assign next available RAM address if new symbol,
///       @100 -> use numeric value directly.
/// </summary>
void ResolveSymbolValues(ISymbolTable symbolTable, ICollection<KeyValuePair<string, string>> parsedCommands)
{
        var entry = parsedCommands.First(kvp => kvp.Key == "symbol");
        var symbol = entry.Value;
        var isNumeric = int.TryParse(symbol, out var constant);
        var isPresent = symbolTable.Contains(symbol);

        if (isNumeric)
        {
            // If the symbol is numeric, we can directly use it as the address.
            parsedCommands.Remove(entry);
            parsedCommands.Add(new KeyValuePair<string, string>("constant", constant.ToString()));
        }
        else if (!isPresent)
        {
            // If the symbol is not present in the symbol table, we need to add it with the next available RAM address.
            var nextAvailableAddress = symbolTable.GetNextAvailableAddress();
            symbolTable.AddEntry(symbol, nextAvailableAddress);
            parsedCommands.Remove(entry);
            parsedCommands.Add(new KeyValuePair<string, string>("address", nextAvailableAddress.ToString()));
        }
        else
        {
            var address = symbolTable.GetAddress(symbol);
            parsedCommands.Remove(entry);
            parsedCommands.Add(new KeyValuePair<string, string>("address", address.ToString()));
        }
}

/// <summary>
/// Parses the current command in the parser and returns its type along with a collection of key-value
/// pairs representing the parsed components of the command.
/// For A_COMMAND and L_COMMAND, the collection will contain a single entry with the key "symbol" and the corresponding symbol value.
/// For C_COMMAND, the collection will contain entries for "dest", "comp", and "jump" with their respective values.
/// </summary>
(Type type, ICollection<KeyValuePair<string, string>>) GetParsedCommands(Parser parser)
{
    var commandType = parser.CommandType();
    var parsedResults = commandType switch
    {
        Type.A_COMMAND or Type.L_COMMAND => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("symbol", parser.Symbol())
        },
        Type.C_COMMAND => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("dest", parser.Dest()),
            new KeyValuePair<string, string>("comp", parser.Comp()),
            new KeyValuePair<string, string>("jump", parser.Jump())
        },
        _ => throw new InvalidOperationException($"Unsupported command type: {parser.CommandType()}")
    };

    return (commandType, parsedResults);
}

string ConvertCommandsToBinary(ISymbolTable symbolTable, ICollection<KeyValuePair<string, string>> parsedCommands, StringBuilder sb)
{
    // binary: comp-dest-jump
    var binaryCompOrder = new List<string> { "comp", "dest", "jump" };
    foreach (var kvp in parsedCommands.OrderBy(k => binaryCompOrder.IndexOf(k.Key)))
    {
        var binaryTextValue = kvp switch
        {
            { Key: "constant", Value: var constant } => Code.ToBinary(int.Parse(constant)),
            { Key: "address", Value: var address } => Code.ToBinary(int.Parse(address)),

            { Key: "dest", Value: var dest } => Code.Dest(dest),
            { Key: "comp", Value: var comp } => Code.Comp(comp),
            { Key: "jump", Value: var jump } => Code.Jump(jump),

            _ => throw new InvalidOperationException($"Unexpected key-value pair: {kvp.Key}={kvp.Value}")
        };

        sb.Append(binaryTextValue);
    }

    return sb.ToString().TrimEnd(',', ' ');
}
