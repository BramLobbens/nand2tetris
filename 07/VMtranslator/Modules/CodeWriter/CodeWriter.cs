namespace VMtranslator.Modules.CodeWriter;

using Modules.CodeWriter.StackArithmetic;
using Modules.CodeWriter.MemoryAccess;

internal enum Register
{
    A,
    D,
    M
}

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

    /// <summary>
    /// Informs the code writer that a new VM file is being translated.
    /// </summary>
    /// <param name="fileName">The name of the VM file being translated.</param>
    public void SetFileName(string fileName)
    {
        _writer.WriteLine($"// File: {fileName}");
    }

    /// <summary>
    /// Writes the initialization code for the VM translator, setting the stack pointer to 256.
    /// </summary>
    public void WriteInit()
    {
        _writer.WriteLine(InitializationCode());
    }

    private static string InitializationCode() =>
    """
    // Initialize stack pointer to 256
        @256
        D=A
        @SP
        M=D
    // End of initialization
    """;

    public void WriteArithmetic(string command)
    {
        string assembly = command switch
        {
            "add" => ArithmeticCommandBuilder.Add(),
            "sub" => ArithmeticCommandBuilder.Sub(),
            "neg" => ArithmeticCommandBuilder.Neg(),
            "eq" => ArithmeticCommandBuilder.Eq(),
            "gt" => ArithmeticCommandBuilder.Gt(),
            "lt" => ArithmeticCommandBuilder.Lt(),
            "and" => ArithmeticCommandBuilder.And(),
            "or" => ArithmeticCommandBuilder.Or(),
            "not" => ArithmeticCommandBuilder.Not(),

            _ => throw new InvalidOperationException($"Unknown command: {command}")
        };

        _writer.WriteLine(assembly);
    }

    public void WritePushPop(CommandType commandType, string segment, int index)
    {
        var assembly = commandType switch
        {
            CommandType.C_PUSH
                when segment == "constant" => MemoryAccessAssembler.PushConstantToStack(index),
            CommandType.C_POP
                when segment == "constant" => throw new NotImplementedException(),

            _ => throw new InvalidOperationException($"Unknown command type or segment: {commandType}, {segment}")
        };

        _writer.WriteLine(assembly);
    }

}
