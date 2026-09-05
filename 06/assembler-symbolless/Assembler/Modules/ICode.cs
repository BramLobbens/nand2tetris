namespace Assembler.Modules;
internal interface ICode
{
    string Comp(string mnemonic);
    string Dest(string mnemonic);
    string Jump(string mnemonic);
}