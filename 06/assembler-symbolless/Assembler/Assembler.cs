using System.CommandLine;
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

    var isEligibleInput = inputFile is not null && inputFile.Exists && inputFile.Extension.Equals(".asm", StringComparison.OrdinalIgnoreCase);
    if (!isEligibleInput)
    {
        Console.WriteLine("Invalid input file. Please provide a valid .asm file.");
        return 1;
    }

    ParseAssemblyFile(inputFile!, outputFileName is null ? null : new FileInfo(outputFileName)); // Library will handle missing file argument, so we can safely use the null-forgiving operator here.
    return 0;
}

void ParseAssemblyFile(FileInfo inputFile, FileInfo? outputFile)
{
    var parser = new Parser(File.ReadLines(inputFile.FullName));
    var outputFilePath = outputFile is null || string.IsNullOrWhiteSpace(outputFile.FullName)
        ? Path.ChangeExtension(inputFile.FullName, ".hack")
        : Path.ChangeExtension(outputFile.FullName, ".hack");

    using var writer = new StreamWriter(outputFilePath);
    while (parser.HasMoreCommands())
    {
        parser.Advance();
        //...
        writer.WriteLine($"{parser.CurrentLine}");
    }
}
