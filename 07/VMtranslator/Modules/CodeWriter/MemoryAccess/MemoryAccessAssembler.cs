namespace VMtranslator.Modules.CodeWriter.MemoryAccess;

internal static class MemoryAccessAssembler
{
    internal static string PushConstantToStack(int value) =>
    $"""
        @{value}        // Load the constant value into A
        D=A             // Store the constant value in D
        @SP
        A=M             // Point to the top of the stack
        M=D             // Push the constant value onto the stack
        @SP
        M=M+1           // Increment the stack pointer
    """;

    internal static string PopStackToD() =>
    """
        @SP             // Point to the stack pointer
        AM=M-1          // SP--; A=SP
        D=M             // Store the topmost stack value in D
    """;
}