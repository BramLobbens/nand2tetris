namespace VMtranslator.Modules.CodeWriter;

internal sealed class CodeWriterContext
{
    internal string VmModuleName { get; private set; } = string.Empty;

    internal void SetVmModuleName(string vmFileNameWithoutExtension)
    {
        ArgumentNullException.ThrowIfNull(vmFileNameWithoutExtension);
        VmModuleName = vmFileNameWithoutExtension;
    }
}