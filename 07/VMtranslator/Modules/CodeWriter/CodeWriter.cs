namespace VMtranslator.Modules.CodeWriter;

using VMtranslator.Modules.Interfaces;

internal enum Register
{
    A,
    D,
    M
}

internal sealed class CodeWriter : ICodeWriter, IDisposable
{
    private readonly StreamWriter _writer;

    private readonly CodeWriterContext _context;

    private readonly StackArithmetic _stackArithmetic;

    private readonly MemoryAccess _memoryAccess;


    internal CodeWriter(string filePath)
    {
        _context = new CodeWriterContext();
        _stackArithmetic = new StackArithmetic(_context);
        _memoryAccess = new MemoryAccess(_context);
        _writer = new StreamWriter(filePath);
        WriteInit();
    }

    public void Close()
    {
        _writer.WriteLine(FinalizationCode());
        Dispose();
    }

    public void Dispose() => _writer.Dispose();

    /// <summary>
    /// Informs the code writer that a new VM file is being translated.
    /// </summary>
    /// <param name="fileName">The name of the VM file being translated.</param>
    public void SetFileName(string fileName)
    {
        _context.SetVmModuleName(Path.GetFileNameWithoutExtension(fileName));
        _writer.WriteLine($"// File: {fileName}");
    }

    /// <summary>
    /// Writes the initialization code for the VM translator, setting the stack pointer to 256.
    /// </summary>
    private void WriteInit()
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

    private static string FinalizationCode() =>
    """
        @END
        0;JMP
    (END)
        @END
        0;JMP      // Infinite loop
    // End of translation
    """;

    public void WriteArithmetic(string command)
    {
        string assembly = command switch
        {
            "add" => _stackArithmetic.Add(),
            "sub" => _stackArithmetic.Sub(),
            "neg" => _stackArithmetic.Neg(),
            "eq" => _stackArithmetic.Eq(),
            "gt" => _stackArithmetic.Gt(),
            "lt" => _stackArithmetic.Lt(),
            "and" => _stackArithmetic.And(),
            "or" => _stackArithmetic.Or(),
            "not" => _stackArithmetic.Not(),

            _ => throw new InvalidOperationException($"Unknown command: {command}")
        };

        _writer.WriteLine(assembly);
    }

    public void WritePushPop(CommandType commandType, string segment, int index)
    {
        var assembly = commandType switch
        {
            CommandType.C_PUSH
                when segment == "constant" => _memoryAccess.PushConstantToStack(index),
            CommandType.C_POP
                when segment == "constant" => throw new NotImplementedException(),

            _ => throw new InvalidOperationException($"Unknown command type or segment: {commandType}, {segment}")
        };

        _writer.WriteLine(assembly);
    }

}
