namespace VMtranslator.Modules.CodeWriter;

internal sealed class MemoryAccess
{
    private readonly CodeWriterContext _context;

    public MemoryAccess(CodeWriterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    internal string PushConstantToStack(int value) =>
    $"""
        @{value}            // Load constant '{value}' value into A
        D=A                 // Store '{value}' in D
        @SP
        A=M                 // Load the top of the stack into A
        M=D                 // Push '{value}' onto the stack
        @SP
        M=M+1               // SP++;
    """;

    internal string PopToD() =>
    """
        @SP                 // Point to the stack pointer
        AM=M-1              // SP--; A=SP
        D=M                 // Store the topmost stack value in D
    """;

    internal static string LoadBinaryOperands() =>
    """
        @SP                 // Point to the stack pointer
        AM=M-1              // SP--; A=SP
        D=M                 // Store the topmost stack value in D
        A=A-1               // Point to the second-to-topmost value on the stack
    """;
}