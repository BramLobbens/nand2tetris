namespace VMtranslator.Modules;

internal interface IParser
{
    public bool HasMoreCommands();

    public void Advance();

    public CommandType GetCommandType();

    public string GetArg1();

    public string GetArg2();
}
