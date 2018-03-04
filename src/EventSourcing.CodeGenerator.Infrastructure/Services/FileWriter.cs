namespace EventSourcing.CodeGenerator.Infrastructure.Services
{
    public interface IFileWriter
    {
        void WriteAllLines(string path, string[] lines);
    }

    public class FileWriter : IFileWriter
    {
        public void WriteAllLines(string path, string[] lines)
            => System.IO.File.WriteAllLines(path, lines);
    }
}
