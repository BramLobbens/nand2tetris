using System.Resources;

namespace Assembler.Modules;
internal sealed class Parser : IParser
{
    private readonly StreamReader _reader;
    internal Parser(StreamReader reader)
    {
        _reader = reader;
    }
    public void Advance()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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