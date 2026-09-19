namespace VMtranslator.Modules.CodeWriter.StackArithmetic;

using System.Text;
using VMtranslator.Modules.CodeWriter.MemoryAccess;

internal static class ArithmeticCommandBuilder
{
    internal static string Add()
    {
        var sb = new StringBuilder();
        var popCode = MemoryAccessAssembler.PopStackToD();
        var decrementACode = ArithmeticAssembler.DecrementA();
        var addDToMCode = ArithmeticAssembler.AddDToM(Register.M);

        sb.AppendLine(popCode);
        sb.AppendLine(decrementACode);
        sb.AppendLine(addDToMCode);

        return sb.ToString();
    }

    internal static string Sub()
    {
        var sb = new StringBuilder();
        var popCode = MemoryAccessAssembler.PopStackToD();
        var decrementACode = ArithmeticAssembler.DecrementA();
        var subDFromMCode = ArithmeticAssembler.SubDFromM(Register.M);

        sb.AppendLine(popCode);
        sb.AppendLine(decrementACode);
        sb.AppendLine(subDFromMCode);

        return sb.ToString();
    }

    internal static string Neg()
    {
        var sb = new StringBuilder();
        var decrementACode = ArithmeticAssembler.DecrementA();

        sb.AppendLine(decrementACode);
        sb.AppendLine("M=-M");

        return sb.ToString();
    }

    internal static string Eq()
    {
        throw new NotImplementedException();
        // sb.AppendLine("(EQ_TRUE)");
        // sb.AppendLine("");
        // sb.PopStackValue();
        // //sb.AppendLine("A=A-1");

        // sb.AppendLine($"@EQ_TRUE");
        // sb.AppendLine("D;JEQ");
    }

    internal static string Gt()
    {
        throw new NotImplementedException();
    }

    internal static string Lt()
    {
        throw new NotImplementedException();
    }

    internal static string And()
    {
        throw new NotImplementedException();
    }

    internal static string Or()
    {
        throw new NotImplementedException();
    }

    internal static string Not()
    {
        throw new NotImplementedException();
    }
}