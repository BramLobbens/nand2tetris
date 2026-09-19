namespace VMtranslator.Modules;

using System.Text;

internal class CodeWriter : ICodeWriter, IDisposable
{
    private readonly StreamWriter _writer;

    internal CodeWriter(string filePath)
    {
        _writer = new StreamWriter(filePath);
        WriteInit();
    }

    public void Close() => Dispose();

    public void Dispose() => _writer.Dispose();

    public void SetFileName(string fileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the initialization code for the VM translator, setting the stack pointer to 256.
    /// </summary>
    public void WriteInit()
    {
        _writer.WriteLine("@256");
        _writer.WriteLine("D=A"); // D=256
        _writer.WriteLine("@SP");
        _writer.WriteLine("M=D"); // RAM[0]=256
    }

    public void WriteArithmetic(string command)
    {
        var sb = new StringBuilder();
        if (command == "add")
        {
            // We need to pop the last two values from the stack, add them, and push the result back onto the stack
            sb.AppendLine("@SP"); // Point to the stack pointer
            sb.AppendLine("AM=M-1"); // SP--; A=SP
            sb.AppendLine("D=M"); // D now holds the topmost value from the stack
            sb.AppendLine("A=A-1"); // A now points to the second-to-topmost value on the stack, since SP valuewas kept in A
            sb.AppendLine("M=M+D"); // Add the topmost value (in D) to the second-to-topmost value (in M) and store the result back in M
        }
        _writer.WriteLine(sb);
    }

    public void WritePushPop(CommandType commandType, string segment, int index)
    {
        var sb = new StringBuilder();
        switch (commandType)
        {
            // Push the value of segment[index] onto the stack
            case CommandType.C_PUSH:
                if (segment == "constant")
                {
                    sb.AppendLine($"@{index}"); // e.g., @7 for constant 7
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
    }
}
