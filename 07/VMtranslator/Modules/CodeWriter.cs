namespace VMtranslator.Modules;

using System.Text;

internal class CodeWriter : ICodeWriter, IDisposable
{
    private readonly StreamWriter _writer;

    internal CodeWriter(string filePath)
    {
        _writer = new StreamWriter(filePath);
    }

    public void Close() => Dispose();

    public void Dispose() => _writer.Dispose();

    public void SetFileName(string fileName)
    {
        throw new NotImplementedException();
    }

    public void WriteInit()
    {
        _writer.WriteLine("@256");
        _writer.WriteLine("D=A"); // D=256
        _writer.WriteLine("@SP");
        _writer.WriteLine("M=D"); // RAM[0]=256
    }

    public void WriteArithmetic(string command)
    {
        _writer.WriteLine(command);
    }

    public void WritePushPop(CommandType commandType, string segment, int index)
    {
        var sb = new StringBuilder();
        switch (commandType)
        {
            // Push the value of segment[index] onto the stack
            // in hack asm, this is a 2-step operation

            // 1. Obtain constant
            // 2. Find SP
            // 3. RAM[SP] = 7
            // 4. SP++
            case CommandType.C_PUSH:
                if (segment == "constant")
                {
                    sb.AppendLine($"@{index}");
                    sb.AppendLine("D=A"); // 1. D=7

                    sb.AppendLine("@SP");
                    sb.AppendLine("A=M"); // 2. A=SP

                    sb.AppendLine("M=D"); // 3. RAM[SP]=7

                    sb.AppendLine("@SP");
                    sb.AppendLine("M=M+1"); // 4. SP++
                }
                _writer.Write(sb);
                break;
            // Pop the top stack value and store it in segment[index]
            case CommandType.C_POP:
                break;
        }

        _writer.WriteLine("Wrote WritePushPop test");
    }
}
