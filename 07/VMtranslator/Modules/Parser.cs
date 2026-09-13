namespace VMtranslator.Modules;

internal class Parser : IParser
{
    private readonly IEnumerable<string> _lines;

    internal Parser(IEnumerable<string> lines)
    {
        _lines = lines;
    }

    public void Advance()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public bool HasMoreCommands()
    {
        throw new NotImplementedException();
    }
}
