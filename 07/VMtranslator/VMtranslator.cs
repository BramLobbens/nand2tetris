using System.Collections.ObjectModel;
using System.CommandLine;
using VMtranslator;
using VMtranslator.Modules;

var pathArgument = new Argument<string>("path");
var outputFileOption = new Option<string>(name: "--output", aliases: ["-o"]);
var rootCommand = new RootCommand("VMtranslator for Hack assembly language.") { pathArgument, outputFileOption };

rootCommand.SetAction(ParseResultHandler);
var parseResult = rootCommand.Parse(args);
parseResult.Invoke();

int ParseResultHandler(ParseResult result)
{
    var outputFileName = result.GetValue(outputFileOption);
    var outputFile = string.IsNullOrWhiteSpace(outputFileName) ? null : new FileInfo(outputFileName);
    var inputFiles = Enumerable.Empty<FileInfo>();

    var path = result.GetValue(pathArgument);
    if (File.Exists(path))
    {
        var inputFile = new FileInfo(path);
        var isEligibleInput = inputFile is not null && inputFile.Extension.Equals(Constants.FileExtensions.VM, StringComparison.OrdinalIgnoreCase);
        if (!isEligibleInput)
        {
            Console.WriteLine($"Invalid input file. Please provide a valid {Constants.FileExtensions.VM} file.");
            return 1;
        }

        inputFiles = [ inputFile! ];
    }
    else if (Directory.Exists(path))
    {
        var directory = new DirectoryInfo(path);
        var retrievedFiles = directory.EnumerateFiles("*.*")
            .Where(file => file.Extension.Equals(Constants.FileExtensions.VM, StringComparison.OrdinalIgnoreCase));

        if (!retrievedFiles.Any())
        {
            Console.WriteLine("No valid input files found.");
            return 1;
        }

        inputFiles = retrievedFiles;
    }
    else
    {
        Console.WriteLine($"Invalid argument {path}");
        return 1;
    }

    ParseVMFiles(inputFiles.ToList().AsReadOnly(), outputFile);

    return 0;
}

void ParseVMFiles(ReadOnlyCollection<FileInfo> inputFiles, FileInfo? outputFile)
{
    var codeWriter = new CodeWriter();
    foreach (var file in inputFiles)
    {
        var parser = new Parser();
        ParseVMFile(parser, codeWriter, file, outputFile);
    }
}

void ParseVMFile(IParser parser, ICodeWriter codeWriter, FileInfo inputFile, FileInfo? outputFile)
{
    throw new NotImplementedException();
}
