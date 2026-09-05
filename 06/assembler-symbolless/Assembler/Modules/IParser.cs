interface IParser
{
    bool HasMoreCommands();
    void Advance();
    CommandType CommandType();
    string Symbol();
    string Dest();
    string Comp();
    string Jump();
}