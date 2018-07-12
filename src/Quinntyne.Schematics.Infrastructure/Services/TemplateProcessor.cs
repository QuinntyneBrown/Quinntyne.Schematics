using System;
using System.Collections.Generic;
using System.Text;

namespace Quinntyne.Schematics.Infrastructure.Services
{
    public interface ITemplateProcessor
    {
        string[] ProcessTemplate(string[] template, IDictionary<string,string> tokens);
    }

    public class TemplateProcessor: ITemplateProcessor
    {
        private INamingConventionConverter _namingConventionConverter;
        public TemplateProcessor(INamingConventionConverter namingConventionConverter)
        {
            _namingConventionConverter = namingConventionConverter;
        }

        public string[] ProcessTemplate(string[] template, IDictionary<string, string> tokens)
        {
            var lines = new List<string>();

            foreach (var line in template)
            {
                var newLine = line;

                foreach (var token in tokens)
                {
                    newLine = newLine.Replace(token.Key, token.Value);                
                }
                
                lines.Add(newLine);
            }
            return lines.ToArray();
        }
    }
}
