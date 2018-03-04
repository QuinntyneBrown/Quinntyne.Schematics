namespace EventSourcing.CodeGenerator.Infrastructure.Services
{
    public interface IFileWriter
    {
        void WriteAllLines(string path, string[] lines = default(string[]));
    }

    public class FileWriter : IFileWriter
    {
        public void WriteAllLines(string path, string[] lines = default(string[]))
            => System.IO.File.WriteAllLines(path, lines);
    }
}
