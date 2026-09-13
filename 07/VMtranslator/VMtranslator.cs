using System.CommandLine;
using System.Text;
using VMtranslator;

var pathArgument = new Argument<string>("path");
var outputFileOption = new Option<string>(name: "--output", aliases: ["-o"]);

var rootCommand = new RootCommand("VMtranslator for Hack assembly language.")
{
    pathArgument,
    outputFileOption
};

rootCommand.SetAction(ParseResultHandler);
var parseResult = rootCommand.Parse(args);
parseResult.Invoke();

int ParseResultHandler(ParseResult result)
{
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
    }
    else if (Directory.Exists(path))
    {
        var directory = new DirectoryInfo(path);
        // to-do parse VM files from directory
    }
    else
    {
        Console.WriteLine($"Invalid argument {path}");
        return 1;
    }

    var outputFileName = result.GetValue(outputFileOption);

    return 0;
}