interface IParser
{
    bool HasMoreCommands();
    void Advance();
    Type CommandType();
    string Symbol();
    string Dest();
    string Comp();
    string Jump();
}