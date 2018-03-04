namespace Quinntyne.CodeGenerator.Infrastructure.Interfaces
{
    public interface ICodeGeneratorCommandRequest
    {
        string Namespace { get; set; }
        string RootNamespace { get; set; }
        dynamic Settings { get; set; }
    }
}
