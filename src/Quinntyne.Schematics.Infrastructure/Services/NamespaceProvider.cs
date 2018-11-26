using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.String;
using static System.Xml.Linq.XDocument;
using static System.IO.Directory;
using static System.IO.Path;
using System.IO;
using Newtonsoft.Json;

namespace Quinntyne.Schematics.Infrastructure.Services
{
    public class Namespace {
        public Namespace(string root)
        {
            Root = root;
            Value = root;
        }

        public Namespace(string root, string value)
        {
            Root = root;
            Value = value;
        }

        public string Value { get; set; }
        public string Root { get; set; }
    }

    public interface INamespaceProvider
    {
        Namespace GetNamespace(string path);
        string GetSolutionPath(string path, int depth = 0);
    }

    public class NamespaceProvider : INamespaceProvider
    {
        private INamingConventionConverter _namingConventionConverter;
        public NamespaceProvider(INamingConventionConverter namingConventionConverter)
            => _namingConventionConverter = namingConventionConverter;

        public Namespace GetNamespace(string path)
        {
            var rootNamespace = GetRootNamespace(path);

            var projectPath = GetProjectPath(path);

            if(IsNullOrEmpty(projectPath))
                return new Namespace(rootNamespace, null);

            var subNamespaces = GetSubNamespaces(path, projectPath);
            
            return new Namespace(rootNamespace, $"{Join(".", subNamespaces)}");
        }

        public string GetRootNamespace(string path)
        {
            if(File.Exists($"{path}\\codeGeneratorSettings.json"))
            {
                using(var settings = new StreamReader($"{path}\\codeGeneratorSettings.json"))
                {
                    string json = settings.ReadToEnd();
                    return JsonConvert.DeserializeObject<dynamic>(json).RootNamespace;
                }
            } else if(!IsNullOrEmpty(GetProjectPath(path)))
            {
                using (var settings = new StreamReader($"{System.IO.Path.GetDirectoryName(GetProjectPath(path))}\\codeGeneratorSettings.json"))
                {
                    string json = settings.ReadToEnd();
                    return JsonConvert.DeserializeObject<dynamic>(json).RootNamespace;
                }
            }
            
            return Path.GetFileName(path);            
        }

        public string GetProjectPath(string path, int depth = 0)
        {
            var directories = path.Split(DirectorySeparatorChar);

            if (directories.Length <= depth)
                return null;

            var newDirectories = directories.Take(directories.Length - depth);
            var computedPath = Join(DirectorySeparatorChar.ToString(), newDirectories);
            var projectFiles = GetFiles(computedPath, "*.csproj");
            depth = depth + 1;
            return (projectFiles.FirstOrDefault() != null) ? projectFiles.First() : GetProjectPath(path, depth);
        }

        public string GetWebpackConfigPath(string path, int depth = 0)
        {
            var directories = path.Split(DirectorySeparatorChar);

            if (directories.Length <= depth)
                return null;

            var newDirectories = directories.Take(directories.Length - depth);
            var computedPath = Join(DirectorySeparatorChar.ToString(), newDirectories);
            var projectFiles = GetFiles(computedPath, "*.config.js");
            depth = depth + 1;
            return (projectFiles.FirstOrDefault() != null) ? projectFiles.First() : GetProjectPath(path, depth);
        }

        public string GetSolutionPath(string path, int depth = 0)
        {
            var directories = path.Split(DirectorySeparatorChar);

            if (directories.Length <= depth)
                return null;

            var newDirectories = directories.Take(directories.Length - depth);
            var computedPath = Join(DirectorySeparatorChar.ToString(), newDirectories);
            var solutionFile = GetFiles(computedPath, "*.sln");
            depth = depth + 1;
            return (solutionFile.FirstOrDefault() != null) ? solutionFile.First() : GetSolutionPath(path, depth);
        }

        public List<string> GetSubNamespaces(string path, string projectPath)
        {
            var pathDirectories = path.Split(DirectorySeparatorChar);
            var skip = GetProjectPath(path).Split(DirectorySeparatorChar).Length - 2;
            var subNamespaces = pathDirectories.Skip(skip).Take(pathDirectories.Length).ToList();
            List<string> subNamespacesPascalCase = new List<string>();
            foreach (var subNamespace in subNamespaces)
            {
                subNamespacesPascalCase.Add(_namingConventionConverter.Convert(NamingConvention.PascalCase, subNamespace));
            }
            return subNamespacesPascalCase;
        }

        public bool IsDirectory(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory);
    }
}
