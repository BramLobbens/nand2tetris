namespace VMtranslator.Modules.CodeWriter.StackArithmetic;

internal static class ArithmeticAssembler
{
    internal static string AddDToM(Register register) =>
    $"""
        {register}=M+D  // Add the value in D to M
    """;

    internal static string SubDFromM(Register register) =>
    $"""
        {register}=M-D  // Subtract the value in D from M
    """;

    internal static string DecrementA() =>
    """
        A=A-1  // Move A to point to the second-to-topmost value on the stack
    """;

    internal static string SetZero(Register register) =>
    $"""
        {register}=0  // Set the value in {register} to zero
    """;

    internal static string SetOne(Register register) =>
    $"""
        {register}=1  // Set the value in {register} to one
    """;

    internal static string SetNegativeOne(Register register) =>
    $"""
        {register}=-1  // Set the value in {register} to minus one
    """;
}