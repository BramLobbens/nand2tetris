using System.CommandLine;

namespace VMtranslator.Modules;

internal class Parser : IParser
{
    private readonly IEnumerable<string> _lines;
    private readonly IEnumerator<string> _enumerator;
    internal string _currentLine = string.Empty;

    private readonly string[] arithmeticCommands = ["add", "sub", "neg", "eq", "gt", "lt", "and", "or", "not"];
    private readonly string[] memoryAccessSegments = ["argument", "local", "static", "constant", "this", "that", "pointer", "temp"];

    internal Parser(IEnumerable<string> lines)
    {
        _lines = lines;
        _enumerator = _lines.GetEnumerator();
    }

    public void Advance()
    {
        _currentLine = _enumerator.Current;
        while (string.IsNullOrWhiteSpace(_currentLine) || _currentLine.StartsWith("//"))
        {
            // Skip empty lines and comments
            _enumerator.MoveNext();
            _currentLine = _enumerator.Current;
        }
    }

    public string GetArg1()
    {
        throw new NotImplementedException();
    }

    public string GetArg2()
    {
        throw new NotImplementedException();
    }

    public CommandType GetCommandType()
    {
        var lineParts = _currentLine.Trim().Split();
        return lineParts switch
        {
            // Arithmetic & logic
            [var head] when arithmeticCommands.Contains(head) => CommandType.C_ARITHMETIC,

            // Memory access
            ["pop", var segment, var index] => CommandType.C_POP,
            ["push", var segment, var index] => CommandType.C_PUSH,

            // Program flow
            ["label", var symbol] => CommandType.C_LABEL,
            ["goto", var symbol] => CommandType.C_GOTO,
            ["if-goto", var symbol] => CommandType.C_IF,

            // Function calling
            ["function", var functionName, var nLocals] => CommandType.C_FUNCTION,
            ["call", var functionName, var nArgs] => CommandType.C_CALL,
            ["return"] => CommandType.C_RETURN,

            _ => throw new InvalidOperationException()
        };
    }

    public bool HasMoreCommands()
    {
        return _enumerator.MoveNext();
    }
}
