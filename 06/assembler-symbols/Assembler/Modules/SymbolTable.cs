namespace Assembler.Modules;

internal sealed class SymbolTable : ISymbolTable
{
    private static readonly Lazy<SymbolTable> _instance = new Lazy<SymbolTable>(() => new SymbolTable());

    internal static SymbolTable Instance => _instance.Value;

    private readonly Dictionary<string, int> _table = new Dictionary<string, int>();

    private bool _isInitialized = false;

    private SymbolTable()
    {
        if (!_isInitialized)
        {
            InitializePredefinedSymbols();
            _isInitialized = true;
        }
    }

    private void InitializePredefinedSymbols()
    {
        foreach (var kvp in PredefinedSymbols.Symbols)
        {
            _table[kvp.Key] = kvp.Value;
        }
    }

    public void AddEntry(string symbol, int address)
    {
        _table[symbol] = address;
    }

    public bool Contains(string symbol)
    {
        return _table.ContainsKey(symbol);
    }

    public int GetAddress(string symbol)
    {
        return _table[symbol];
    }

    public int GetNextAvailableAddress()
    {
        // The next available RAM address starts from 16, as addresses 0-15 are reserved for predefined symbols.
        int nextAvailableAddress = 16;

        // Find the next available address that is not already in use in the symbol table.
        while (_table.ContainsValue(nextAvailableAddress))
        {
            nextAvailableAddress++;
        }

        return nextAvailableAddress;
    }
}