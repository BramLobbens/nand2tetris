namespace VMtranslator.Modules.CodeWriter;

internal sealed class StackArithmetic
{
    private readonly CodeWriterContext _context;

    public StackArithmetic(CodeWriterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    internal string Add() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M+D
    """;

    internal string Sub() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M-D
    """;

    internal string Neg() =>
    """
        @SP
        AM=M-1
        M=-M
    """;

    internal string Eq()
    {
        return
        $"""
            @SP
            AM=M-1
            D=M
            A=A-1
            D=M-D
            @EQ_TRUE_{_context.LabelCounter}
            D;JEQ

            @SP
            A=M-1
            M=0         // False
            @EQ_END_{_context.LabelCounter}
            0;JMP

        (EQ_TRUE_{_context.LabelCounter})
            @SP
            A=M-1
            M=-1        // True

        (EQ_END_{_context.LabelCounter})
        """;
    }

    internal string Gt()
    {
        return
        $"""
            @SP
            AM=M-1
            D=M
            A=A-1
            D=M-D
            @GT_TRUE_{_context.LabelCounter}
            D;JGT

            @SP
            A=M-1
            M=0         // False
            @GT_END_{_context.LabelCounter}
            0;JMP

        (GT_TRUE_{_context.LabelCounter})
            @SP
            A=M-1
            M=-1        // True

        (GT_END_{_context.LabelCounter})
        """;
    }

    internal string Lt()
    {
        return
        $"""
            @SP
            AM=M-1
            D=M
            A=A-1
            D=M-D
            @LT_TRUE_{_context.LabelCounter}
            D;JLT

            @SP
            A=M-1
            M=0         // False
            @LT_END_{_context.LabelCounter}
            0;JMP

        (LT_TRUE_{_context.LabelCounter})
            @SP
            A=M-1
            M=-1        // True

        (LT_END_{_context.LabelCounter})
        """;
    }

    internal string And() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M&D
    """;

    internal string Or() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M|D
    """;

    internal string Not() =>
    """
        @SP
        AM=M-1
        A=A-1
        M=!M
    """;
}