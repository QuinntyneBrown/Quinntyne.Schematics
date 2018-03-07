namespace Quinntyne.Schematics.Infrastructure.Interfaces
{
    public interface ICodeGeneratorCommandRequest
    {
        string Namespace { get; set; }
        string RootNamespace { get; set; }
        dynamic Settings { get; set; }
    }
}
