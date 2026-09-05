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
    var output = result.GetValue(outputFileOption);

    ParseAssemblyFile(inputFile!); // Library will handle missing file argument, so we can safely use the null-forgiving operator here.
    return 0;
}

void ParseAssemblyFile(FileInfo fileInfo)
{
    using var reader = new StreamReader(fileInfo.FullName);
    var parser = new Parser(reader);
}
