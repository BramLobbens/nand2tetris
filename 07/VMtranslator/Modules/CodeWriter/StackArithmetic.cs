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
        @END
        0;JMP
    """;

    internal string Sub() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M-D
        @END
        0;JMP
    """;

    internal string Neg() =>
    """
        @SP
        AM=M-1
        M=-M
        @END
        0;JMP
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
            @EQ_{_context.VmFileName}
            D;JEQ

            M=0         // Set false if not equal
            @END
            0;JMP

        (EQ_{_context.VmFileName})
            M=-1        // Set true if equal
        """;
        // sb.AppendLine("(EQ_TRUE)");
        // sb.AppendLine("");
        // sb.PopStackValue();
        // //sb.AppendLine("A=A-1");

        // sb.AppendLine($"@EQ_TRUE");
        // sb.AppendLine("D;JEQ");
    }

    internal string Gt()
    {
        throw new NotImplementedException();
    }

    internal string Lt()
    {
        throw new NotImplementedException();
    }

    internal string And() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M&D
        @END
        0;JMP
    """;

    internal string Or() =>
    """
        @SP
        AM=M-1
        D=M
        A=A-1
        M=M|D
        @END
        0;JMP
    """;

    internal string Not() =>
    """
        @SP
        AM=M-1
        A=A-1
        M=!M
        @END
        0;JMP
    """;
}