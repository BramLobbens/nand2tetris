namespace VMtranslator.Modules.CodeWriter.StackArithmetic;

internal static class ArithmeticAssembler
{
    #region Stack Operations
    internal static string PopStackToD() =>
    """
        @SP     // Point to the stack pointer
        AM=M-1  // SP--; A=SP
        D=M     // Store the topmost stack value in D
    """;

    internal static string DecrementA() =>
    """
        A=A-1  // Move A to point to the second-to-topmost value on the stack
    """;

    #endregion

    #region Arithmetic Operations

    internal static string IncrementD() =>
    """
        D=D+1  // Increment the value in D
    """;

    internal static string IncrementM() =>
    """
        M=M+1  // Increment the value in M
    """;

    internal static string AddMToD() =>
    """
        D=D+M  // Add the value in M to D
    """;

    internal static string AddDToM() =>
    """
        M=M+D  // Add the value in D to M
    """;

    internal static string SubDFromM() =>
    """
        M=M-D  // Subtract the value in D from M
    """;

    #endregion
}