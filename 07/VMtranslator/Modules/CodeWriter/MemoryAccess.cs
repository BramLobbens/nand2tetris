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

    internal string PushVirtualSegmentToStack(string segment, int index) =>
    $"""
        @{segment}           // Load base address of segment '{segment}' into A, e.g., LCL for local, ARG for argument
        D=A                  // Store the base address of segment '{segment}' in D
        @{index}             // Load the index '{index}' into A
        A=D+A                // Compute the effective address of segment '{segment}' at index '{index}'

        D=M                  // Store the value at the effective address in D

        @SP
        A=M                  // A=SP; Load the address of the top of the stack into A
        M=D                  // Push the value onto the stack

        @SP
        M=M+1               // SP++;
    """;

    internal string PopStackToVirtualSegment(string segment, int index) =>
    $"""
        @{segment}
        D=A
        @{index}
        D=D+A               // Compute the effective address of segment '{segment}' at index '{index}'

        @R13
        M=D                 // Store the effective address in R13 (temporary register)

        @SP
        AM=M-1              // SP--; A=SP
        D=M                 // Store the top of the stack in D

        @R13
        A=M
        M=D
    """;
}