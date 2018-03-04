using EventSourcing.CodeGenerator.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace EventSourcing.CodeGenerator.Infrastructure.Services
{
    public interface ITemplateRepository
    {
        string[] Get(string filename);
    }

    public class TemplateRepository : ITemplateRepository
    {
        protected readonly INamingConventionConverter _namingConventionConverter;

        public TemplateRepository(INamingConventionConverter namingConventionConverter)
        {
            _namingConventionConverter = namingConventionConverter;
        }

        public string[] Get(string name)
        {
            List<string> lines = new List<string>();

            string templateName = $"EventSourcing.CodeGenerator.Infrastructure.Templates.{name}";
            
            try
            {
                using (System.IO.Stream stream = typeof(TemplateRepository).Assembly.GetManifestResourceStream(templateName))
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                    return lines.ToArray();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error:" + templateName);

                throw exception;

            }
        }
    }
}
