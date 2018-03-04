//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace EventSourcing.CodeGenerator.Infrastructure.Services
//{
//    public interface INamespaceProvider
//    {
//        FileNamespace GetNamespace(string path);
//    }

//    public class NamespaceProvider : INamespaceProvider
//    {
//        protected readonly XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
//        protected readonly INamingConventionConverter _namingConventionConverter;
//        protected readonly ILeoraJSONFileManager _leoraJSONFileManager;
//        public NamespaceProvider(INamingConventionConverter namingConventionConverter, ILeoraJSONFileManager leoraJSONFileManager)
//        {
//            _namingConventionConverter = namingConventionConverter;
//            _leoraJSONFileManager = leoraJSONFileManager;
//        }

//        public NamespaceProvider()
//        {
//            _namingConventionConverter = new NamingConventionConverter();
//            _leoraJSONFileManager = new LeoraJSONFileManager();
//        }

//        public FileNamespace GetNamespace(string path)
//        {
//            var rootNamespace = _leoraJSONFileManager.GetLeoraJSONFile(path, -1).RootNamespace;

//            var projectPath = GetProjectPath(path);

//            var subNamespaces = GetSubNamespaces(path, projectPath);

//            if (subNamespaces.Count() < 1)
//                return new FileNamespace() { Namespace = rootNamespace, RootNamespace = rootNamespace };

//            return new FileNamespace() { Namespace = $"{rootNamespace}.{Join(".", subNamespaces)}", RootNamespace = rootNamespace };
//        }

//        public string GetProjectPath(string path, int depth = 0)
//        {
//            var directories = path.Split(DirectorySeparatorChar);

//            if (directories.Length <= depth)
//                return null;

//            var newDirectories = directories.Take(directories.Length - depth);
//            var computedPath = Join(DirectorySeparatorChar.ToString(), newDirectories);
//            var projectFiles = GetFiles(computedPath, "*.csproj");
//            depth = depth + 1;
//            return (projectFiles.FirstOrDefault() != null) ? projectFiles.First() : GetProjectPath(path, depth);
//        }

//        public List<string> GetSubNamespaces(string path, string projectPath)
//        {
//            var pathDirectories = path.Split(DirectorySeparatorChar);
//            var skip = GetDirectoryName(projectPath).Split(DirectorySeparatorChar).Count();
//            var subNamespaces = pathDirectories.Skip(skip).Take(pathDirectories.Length - skip).ToList();
//            List<string> subNamespacesPascalCase = new List<string>();
//            foreach (var subNamespace in subNamespaces)
//            {
//                subNamespacesPascalCase.Add(_namingConventionConverter.Convert(NamingConvention.PascalCase, subNamespace));
//            }
//            return subNamespacesPascalCase;
//        }

//        public bool IsDirectory(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory);
//    }
//}
