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

    public static class Hack
    {
        public const int WORD_SIZE = 16;
        public const int MAX_ADDRESS = 32767; // 2^15 - 1
    }
}