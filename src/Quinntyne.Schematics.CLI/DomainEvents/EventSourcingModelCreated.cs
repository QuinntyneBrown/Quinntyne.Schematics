namespace Quinntyne.Schematics.CLI.DomainEvents
{
    public class EventSourcingModelCreated: DomainEvent
    {
        public string ClassName { get; set; }
        public string Entity { get; set; }
        public string SolutionDirectory { get; set; }
        public string Namespace { get; set; }
        public string RootNamespace { get; set; }

    }
}
