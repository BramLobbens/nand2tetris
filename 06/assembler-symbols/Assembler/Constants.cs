namespace Assembler;

internal static class Constants
{
    public static class FileExtensions
    {
        public const string ASM = ".asm";
        public const string HACK = ".hack";
    }

    public static class AsmLexemes
    {
        public const string A_COMMAND_PREFIX = "@";
        public const string L_COMMAND_PREFIX = "(";
        public const string L_COMMAND_SUFFIX = ")";
        public const string C_COMMAND_DEST_SEPARATOR = "=";
        public const string C_COMMAND_JUMP_SEPARATOR = ";";
        public const string COMMENT_PREFIX = "//";
    }

    public static class HackLexemes
    {
        public const string A_MSB = "0";
        public const string C_MSB = "111";
    }
}