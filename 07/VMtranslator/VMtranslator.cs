using System.Collections.ObjectModel;
using System.CommandLine;
using VMtranslator.Modules.Parser;
using VMtranslator.Modules.CodeWriter;
using VMtranslator;
using VMtranslator.Modules.Interfaces;

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

    ProcessInputFiles(inputFiles.ToList().AsReadOnly(), outputFile);

    return 0;
}

void ProcessInputFiles(ReadOnlyCollection<FileInfo> inputFiles, FileInfo? outputFile)
{
    var outputFilePath = outputFile is null || string.IsNullOrWhiteSpace(outputFile.FullName)
        ? Path.ChangeExtension("out", Constants.FileExtensions.ASM)
        : Path.ChangeExtension(outputFile.FullName, Constants.FileExtensions.ASM);

    using var codeWriter = new CodeWriter(outputFilePath);

    foreach (var file in inputFiles)
    {
        codeWriter.SetFileName(file.Name);
        // Instantiate a different parser for each separate VM file.
        var parser = new Parser(File.ReadLines(file.FullName));
        ParseVMFile(parser, codeWriter, file, outputFile);
    }

    codeWriter.Close();
}

void ParseVMFile(IParser parser, ICodeWriter codeWriter, FileInfo inputFile, FileInfo? outputFile)
{
    while (parser.HasMoreCommands())
    {
        parser.Advance();

        var commandType = parser.GetCommandType();

        switch (commandType)
        {
            case CommandType.C_ARITHMETIC:
                codeWriter.WriteArithmetic(parser.GetArg1());
                break;
            case CommandType.C_PUSH:
            case CommandType.C_POP:
                var segment = parser.GetArg1();
                var success = int.TryParse(parser.GetArg2(), out var index);
                if (!success) throw new InvalidOperationException($"Failed to parse index for segment ['{segment}']");
                codeWriter.WritePushPop(commandType, segment, index);
                break;
            case CommandType.C_LABEL:
            case CommandType.C_GOTO:
            case CommandType.C_IF:
            case CommandType.C_FUNCTION:
            case CommandType.C_RETURN:
            case CommandType.C_CALL:
            default:
                break;
        }
    }
}
