namespace VMtranslator.Modules.Parser;

internal class Parser : IParser
{
    private readonly IEnumerator<string> _enumerator;
    internal string _currentLine = string.Empty;
    internal string? _nextLine = string.Empty;
    private string[] _currentLineParts = Array.Empty<string>();

    private readonly string[] arithmeticCommands = ["add", "sub", "neg", "eq", "gt", "lt", "and", "or", "not"];
    private readonly string[] memoryAccessSegments = ["argument", "local", "static", "constant", "this", "that", "pointer", "temp"];

    internal Parser(IEnumerable<string> lines)
    {
        _enumerator = lines.GetEnumerator();
        // Provide lookahead for the next command
        _nextLine = ReadNextCommand();
    }

    /// <summary>
    /// Advances the parser to the next command, skipping comments and whitespace.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Advance()
    {
        if (_nextLine is null)
        {
            throw new InvalidOperationException("No more commands to advance to.");
        }

        _currentLine = _nextLine;
        _currentLineParts = _currentLine.Trim().Split();

        _nextLine = ReadNextCommand();
    }

    /// <summary>
    /// Gets the first argument of the current command. For arithmetic commands, this returns the command itself.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetArg1()
    {
        return _currentLineParts switch
        {
            [var command] when arithmeticCommands.Contains(command) => command, // Return the arithmetic command itself as the first argument
            [_, var arg1, ..] => arg1,
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Gets the second argument of the current command. This is only valid for commands that have a second argument.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetArg2()
    {
        return _currentLineParts switch
        {
            [_, _, var arg2] => arg2,
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Determines the type of the current command.
    /// </summary>
    /// <returns>The type of the current command.</returns>
    public CommandType GetCommandType()
    {
        return _currentLineParts switch
        {
            // Arithmetic & logic
            [var command] when arithmeticCommands.Contains(command) => CommandType.C_ARITHMETIC,

            // Memory access
            ["pop", ..] => CommandType.C_POP,
            ["push",..] => CommandType.C_PUSH,

            // Program flow
            ["label", ..] => CommandType.C_LABEL,
            ["goto", ..] => CommandType.C_GOTO,
            ["if-goto", ..] => CommandType.C_IF,

            // Function calling
            ["function", ..] => CommandType.C_FUNCTION,
            ["call", ..] => CommandType.C_CALL,
            ["return"] => CommandType.C_RETURN,

            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Checks if there are more commands to be parsed.
    /// </summary>
    /// <returns>True if there are more commands, false otherwise.</returns>
    public bool HasMoreCommands()
    {
        return _nextLine is not null;
    }

    private string? ReadNextCommand()
    {
        while (_enumerator.MoveNext())
        {
            var line = _enumerator.Current.Trim();
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
            {
                return line;
            }
        }

        return null;
    }
}
