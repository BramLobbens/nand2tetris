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
        @{value}        // Load the constant value into A
        D=A             // Store the constant value in D
        @SP
        A=M             // Point to the top of the stack
        M=D             // Push the constant value onto the stack
        @SP
        M=M+1           // Increment the stack pointer
    """;

    internal string PopToD() =>
    """
        @SP             // Point to the stack pointer
        AM=M-1          // SP--; A=SP
        D=M             // Store the topmost stack value in D
    """;

    internal static string LoadBinaryOperands() =>
    """
        @SP             // Point to the stack pointer
        AM=M-1          // SP--; A=SP
        D=M             // Store the topmost stack value in D
        A=A-1           // Point to the second-to-topmost value on the stack
    """;
}