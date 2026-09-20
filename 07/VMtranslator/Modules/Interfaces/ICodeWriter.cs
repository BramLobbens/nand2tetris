namespace VMtranslator.Modules.Interfaces;

internal interface ICodeWriter
{
    public void SetFileName(string fileName);

    public void WriteArithmetic(string command);

    public void WritePushPop(CommandType commandType, string segment, int index);

    public void Close();
}
