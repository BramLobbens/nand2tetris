namespace Assembler.Modules;
internal sealed class Parser : IParser
{
    public string CurrentLine => _currentLine;

    private readonly IEnumerable<string> _lines;

    private readonly IEnumerator<string> _lineEnumerator;

    private string _currentLine = string.Empty;

    internal Parser(IEnumerable<string> lines)
    {
        _lines = lines;
        _lineEnumerator = _lines.GetEnumerator();
    }
    public void Advance()
    {
        // Our HasMoreCommands method already calls MoveNext, so we don't need to call it again here.
        _currentLine = ReadCurrentLine();
        if (string.IsNullOrWhiteSpace(_currentLine) || _currentLine.StartsWith(Constants.COMMENT_PREFIX))
        {
            // Skip empty lines and comments
            _lineEnumerator.MoveNext();
            _currentLine = ReadCurrentLine();
        }
    }

    public Type CommandType()
    {
        return _currentLine switch
        {
            // @symbol or @number
            { } line when line.StartsWith(Constants.A_COMMAND_PREFIX)
                && line.Substring(Constants.A_COMMAND_PREFIX.Length) is { } symbol
                && (!string.IsNullOrWhiteSpace(symbol) || int.TryParse(symbol, out _)) => Type.A_COMMAND,

            // (symbol)
            { } line when line.StartsWith(Constants.L_COMMAND_PREFIX) && line.EndsWith(Constants.L_COMMAND_SUFFIX)
                && line.Substring(Constants.L_COMMAND_PREFIX.Length,
                    line.Length - Constants.L_COMMAND_PREFIX.Length - Constants.L_COMMAND_SUFFIX.Length) is {} symbol
                && (!string.IsNullOrWhiteSpace(symbol)) => Type.L_COMMAND,

            // dest=comp;jump
            { } line when line.Contains(Constants.C_COMMAND_DEST_SEPARATOR)
                || line.Contains(Constants.C_COMMAND_JUMP_SEPARATOR) => Type.C_COMMAND,

            _ => throw new Assembler.ParseException($"Could not determine command type: {_currentLine}")
        };
    }

    public string Comp()
    {
        var line = _currentLine;
        // The comp part is the part after the '=' and before the ';' if they exist.
        if (line.Contains(Constants.C_COMMAND_DEST_SEPARATOR))
        {
            var parts = line.Split(Constants.C_COMMAND_DEST_SEPARATOR, 2);
            var compPart = parts[1].Trim();
            if (compPart.Contains(Constants.C_COMMAND_JUMP_SEPARATOR))
            {
                var compAndJumpParts = compPart.Split(Constants.C_COMMAND_JUMP_SEPARATOR, 2);
                return compAndJumpParts[0].Trim();
            }
            return compPart;
        }
        else if (line.Contains(Constants.C_COMMAND_JUMP_SEPARATOR))
        {
            var parts = line.Split(Constants.C_COMMAND_JUMP_SEPARATOR, 2);
            return parts[0].Trim();
        }

        // Comp is mandatory for C_COMMAND, so if we reach here, it's an error.
        throw new Assembler.ParseException($"Invalid C_COMMAND format: {line}");
    }

    public string Dest()
    {
        var line = _currentLine;
        if (line.Contains(Constants.C_COMMAND_DEST_SEPARATOR))
        {
            var parts = line.Split(Constants.C_COMMAND_DEST_SEPARATOR, 2);
            return parts[0].Trim();
        }
        return string.Empty;
    }

    public bool HasMoreCommands()
    {
        return _lineEnumerator.MoveNext();
    }

    // Even though we effectively already parsed the jump part in the Comp method,
    // we still need to implement this method to satisfy the IParser interface proposed by the book.
    public string Jump()
    {
        var line = _currentLine;
        if (line.Contains(Constants.C_COMMAND_JUMP_SEPARATOR))
        {
            var parts = line.Split(Constants.C_COMMAND_JUMP_SEPARATOR, 2);
            return parts[1].Trim();
        }
        return string.Empty;
    }

    public string Symbol()
    {
        // The book specifies the Symbol method is called without arguments,
        // so we check for the command type again here to ensure the correct behavior.
        return CommandType() switch
        {
            Type.A_COMMAND => _currentLine.Substring(Constants.A_COMMAND_PREFIX.Length),
            Type.L_COMMAND => _currentLine.Substring(Constants.L_COMMAND_PREFIX.Length,
                _currentLine.Length - Constants.L_COMMAND_PREFIX.Length - Constants.L_COMMAND_SUFFIX.Length),
            _ => throw new Assembler.ParseException($"Symbol is not applicable for command type: {CommandType()}")
        };
    }

    private string ReadCurrentLine()
    {
        if (!string.IsNullOrWhiteSpace(_lineEnumerator.Current))
        {
            return _lineEnumerator.Current.Trim();
        }
        return string.Empty;
    }
}