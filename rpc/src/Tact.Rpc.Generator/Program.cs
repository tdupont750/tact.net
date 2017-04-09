using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tact.Rpc.Generator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var config = new GeneratorConfig();

            new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build()
                .Bind(config);
            
            config
                .GetType()
                .GetTypeInfo()
                .GetProperties()
                .ToList()
                .ForEach(p =>
                {
                    var value = p.GetValue(config);
                    Console.WriteLine("{0}: {1}", p.Name, value);
                });

            if (!Directory.Exists(config.ServicePath))
            {
                Console.WriteLine("Service Directory Not Found");
                return -1;
            }

            if (!Directory.Exists(config.ClientPath) && config.CreateDirectories)
            {
                Console.WriteLine("Creating Client Directory");
                Directory.CreateDirectory(config.ClientPath);
            }

            foreach(var file in Directory.EnumerateFiles(config.ServicePath, config.ServiceSearchFilter, SearchOption.AllDirectories))
            {
                Console.WriteLine("Reading: {0}", file);
                var text = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(text);
                var template = ClientClassTemplate.Create(tree, config);
                if (template == null)
                    continue;
                
                var fileName = template.ClassName + config.FileExtension;
                var filePath = Path.Combine(config.ClientPath, fileName);
                Console.WriteLine("Writing: {0}", filePath);
                File.WriteAllText(filePath, template.ToString());
            }

            return 0;
        }
    }
}