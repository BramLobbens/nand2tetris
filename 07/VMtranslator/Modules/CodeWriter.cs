namespace VMtranslator.Modules;

internal class CodeWriter : ICodeWriter, IDisposable
{
    private readonly StreamWriter _writer;

    internal CodeWriter(string filePath)
    {
        _writer = new StreamWriter(filePath);
    }

    public void Dispose()
    {
        _writer.Dispose();
    }

    public void SetFileName(string fileName)
    {
        throw new NotImplementedException();
    }

    public void WriteArithmetic(string command)
    {
        _writer.WriteLine(command);
    }

    public void WritePushPop(CommandType commandType, string segment, int index)
    {
        _writer.WriteLine("Wrote WritePushPop test");
    }
}
