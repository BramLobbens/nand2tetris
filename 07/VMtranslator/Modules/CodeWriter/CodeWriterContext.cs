namespace VMtranslator.Modules.CodeWriter;

internal sealed class CodeWriterContext
{
    internal string VmFileName { get; private set; } = string.Empty;

    internal void SetVmFileName(string vmFileName)
    {
        ArgumentNullException.ThrowIfNull(vmFileName);
        VmFileName = vmFileName;
    }
}