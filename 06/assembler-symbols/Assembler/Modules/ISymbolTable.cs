namespace Assembler.Modules;

internal interface ISymbolTable
{
    void AddEntry(string symbol, int address);
    bool Contains(string symbol);
    int GetAddress(string symbol);
    int GetNextAvailableAddress();
}