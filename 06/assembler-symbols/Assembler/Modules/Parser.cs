namespace Assembler.Modules;
internal sealed class Parser : IParser
{
    private readonly IEnumerable<string> _lines;

    private IEnumerator<string> _lineEnumerator;

    private string _currentLine = string.Empty;

    internal Parser(IEnumerable<string> lines)
    {
        _lines = lines;
        _lineEnumerator = GetLineEnumerator();
    }

    public void Reset()
    {
        _lineEnumerator = GetLineEnumerator();
        _currentLine = string.Empty;
    }

    public void Advance()
    {
        // Our HasMoreCommands method already calls MoveNext, so we don't need to call it again here.
        _currentLine = ReadCurrentLine();
        while (string.IsNullOrWhiteSpace(_currentLine) || _currentLine.StartsWith(Constants.AsmLexemes.COMMENT_PREFIX))
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
            { } line when line.StartsWith(Constants.AsmLexemes.A_COMMAND_PREFIX)
                && line.Substring(Constants.AsmLexemes.A_COMMAND_PREFIX.Length) is { } symbol
                && (!string.IsNullOrWhiteSpace(symbol) || int.TryParse(symbol, out _)) => Type.A_COMMAND,

            // (symbol)
            { } line when line.StartsWith(Constants.AsmLexemes.L_COMMAND_PREFIX) && line.EndsWith(Constants.AsmLexemes.L_COMMAND_SUFFIX)
                && line.Substring(Constants.AsmLexemes.L_COMMAND_PREFIX.Length,
                    line.Length - Constants.AsmLexemes.L_COMMAND_PREFIX.Length - Constants.AsmLexemes.L_COMMAND_SUFFIX.Length) is {} symbol
                && (!string.IsNullOrWhiteSpace(symbol)) => Type.L_COMMAND,

            // dest=comp;jump
            { } line when line.Contains(Constants.AsmLexemes.C_COMMAND_DEST_SEPARATOR)
                || line.Contains(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR) => Type.C_COMMAND,

            _ => throw new Assembler.ParseException($"Could not determine command type: {_currentLine}")
        };
    }

    public string Comp()
    {
        var line = _currentLine;
        // The comp part is the part after the '=' and before the ';' if they exist.
        if (line.Contains(Constants.AsmLexemes.C_COMMAND_DEST_SEPARATOR))
        {
            var parts = line.Split(Constants.AsmLexemes.C_COMMAND_DEST_SEPARATOR, 2);
            var compPart = parts[1].Trim();
            if (compPart.Contains(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR))
            {
                var compAndJumpParts = compPart.Split(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR, 2);
                return NormaliseParsedLine(compAndJumpParts[0]);
            }
            return NormaliseParsedLine(compPart);
        }
        else if (line.Contains(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR))
        {
            var parts = line.Split(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR, 2);
            return NormaliseParsedLine(parts[0]);
        }

        // Comp is mandatory for C_COMMAND, so if we reach here, it's an error.
        throw new Assembler.ParseException($"Invalid C_COMMAND format: {line}");
    }

    public string Dest()
    {
        var line = _currentLine;
        if (line.Contains(Constants.AsmLexemes.C_COMMAND_DEST_SEPARATOR))
        {
            var parts = line.Split(Constants.AsmLexemes.C_COMMAND_DEST_SEPARATOR, 2);
            var destPart = parts[0].Trim();
            return NormaliseParsedLine(destPart);
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
        if (line.Contains(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR))
        {
            var parts = line.Split(Constants.AsmLexemes.C_COMMAND_JUMP_SEPARATOR, 2);
            var jumpPart = parts[1].Trim();
            return NormaliseParsedLine(jumpPart);
        }
        return string.Empty;
    }

    public string Symbol()
    {
        // The book specifies the Symbol method is called without arguments,
        // so we check for the command type again here to ensure the correct behavior.
        var symbolLine = CommandType() switch
        {
            Type.A_COMMAND => _currentLine.Substring(Constants.AsmLexemes.A_COMMAND_PREFIX.Length),
            Type.L_COMMAND => _currentLine.Substring(Constants.AsmLexemes.L_COMMAND_PREFIX.Length,
                _currentLine.Length - Constants.AsmLexemes.L_COMMAND_PREFIX.Length - Constants.AsmLexemes.L_COMMAND_SUFFIX.Length),
            _ => throw new Assembler.ParseException($"Symbol is not applicable for command type: {CommandType()}")
        };

        return NormaliseParsedLine(symbolLine);
    }

    private string ReadCurrentLine()
    {
        if (!string.IsNullOrWhiteSpace(_lineEnumerator.Current))
        {
            return _lineEnumerator.Current.Trim();
        }
        return string.Empty;
    }

    private IEnumerator<string> GetLineEnumerator()
    {
        if (_lineEnumerator != null)
        {
            _lineEnumerator.Dispose();
        }
        return _lines.GetEnumerator();
    }

    private string NormaliseParsedLine(string line)
    {
        // Normalize the symbol by trimming whitespace and splitting on
        // spaces or comments to get the first part.
        var normalizedSymbol = line.Trim().Split(' ')[0]?.Split(Constants.AsmLexemes.COMMENT_PREFIX)[0]?.Trim() ?? string.Empty;
        return normalizedSymbol;
    }
}