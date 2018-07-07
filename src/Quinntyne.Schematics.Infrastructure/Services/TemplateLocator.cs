using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Quinntyne.Schematics.Infrastructure.Services
{
    public interface ITemplateLocator
    {
        string[] Get(string filename);
    }

    public static class StringListExtensions {

        public static string SingleOrDefaultResourceName(this string[] collection,string name)
        {
            try
            {
                string result = null;

                if (collection.Length == 0) return null;

                result = collection.SingleOrDefault(x => x.EndsWith(name));

                if (result != null)
                    return result;

                return collection.SingleOrDefault(x => x.EndsWith($".{name}.txt"));

            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }

    public class TemplateLocator : ITemplateLocator
    {        
        public string[] Get(string name)
        {
            var lines = new List<string>();
            var fullName = default(string);
            var assembly = default(Assembly);
            var embededResourceNames = new List<string>();

            foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                foreach(Assembly _assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var item in _assembly.GetManifestResourceNames()) embededResourceNames.Add(item);
                        
                        if(!string.IsNullOrEmpty(_assembly.GetManifestResourceNames().SingleOrDefaultResourceName(name)))
                        {
                            fullName = _assembly.GetManifestResourceNames().SingleOrDefaultResourceName(name);                            
                            assembly = _assembly;
                        }
                    }
                    catch (System.NotSupportedException notSupportedException)
                    {
                        //swallow
                    }
                }                
            }



            if (fullName == default(string) && assembly == default(Assembly))
                return null;
            
            try
            {
                using (var stream = assembly.GetManifestResourceStream(fullName))
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
                throw exception;
            }
        }

        
    }
}
