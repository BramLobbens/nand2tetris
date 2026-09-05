using System.Resources;

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
        //_lineEnumerator.MoveNext();
        // Our HasMoreCommands method already calls MoveNext, so we don't need to call it again here.
        _currentLine = _lineEnumerator.Current ?? string.Empty;
        Console.WriteLine($"Current line: {_currentLine}");
    }

    public CommandType CommandType()
    {
        throw new NotImplementedException();
    }

    public string Comp()
    {
        throw new NotImplementedException();
    }

    public string Dest()
    {
        throw new NotImplementedException();
    }

    public bool HasMoreCommands()
    {
        return _lineEnumerator.MoveNext();
    }

    public string Jump()
    {
        throw new NotImplementedException();
    }

    public string Symbol()
    {
        throw new NotImplementedException();
    }
}