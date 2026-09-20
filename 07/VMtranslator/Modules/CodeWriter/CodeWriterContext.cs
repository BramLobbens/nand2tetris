namespace VMtranslator.Modules.CodeWriter;

internal sealed class CodeWriterContext
{
    internal string VmModuleName { get; private set; } = string.Empty;

    internal int LabelCounter { get; private set; } = 0;

    internal void SetVmModuleName(string vmFileNameWithoutExtension)
    {
        ArgumentNullException.ThrowIfNull(vmFileNameWithoutExtension);
        VmModuleName = vmFileNameWithoutExtension;
    }

    internal void IncrementLabelCounter()
    {
        LabelCounter++;
    }
}