using ConanSharpmake;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public static class Globals
{
    // Namespace under which packages would be generated
    public const string DefaultPackageNamespace = "ConanPackages";
}

internal class ConsoleParser
{
    private Dictionary<string, string> mParsed = new Dictionary<string, string>();
    public IDictionary<string, string> Parsed
    {
        get
        {
            return mParsed;
        }
    }

    public ConsoleParser(string[] args)
    {
        foreach (string arg in args)
        {
            bool isArg = arg.StartsWith("--");
            if (!isArg)
            {
                continue;
            }

            int equals = arg.IndexOf('=');
            if (equals != -1)
            {
                string key = arg.Substring(2, equals - 2);
                string val = arg.Substring(equals + 1, arg.Length - equals - 1);
                mParsed[key] = val;
            }
            else
            {
                string key = arg.Substring(2, equals);
                mParsed[key] = "";
            }
        }
    }

    public string GetOrDefault(string key, string def = "")
    {
        if (Parsed.ContainsKey(key))
        {
            return Parsed[key];
        }
        return def;
    }

    public bool GetOrDefault(string key, bool def)
    {
        if (Parsed.ContainsKey(key))
        {
            string lowerCase = Parsed[key].Trim();
            if (lowerCase == "true")
            {
                return true;
            }

            if (lowerCase == "false")
            {
                return false;
            }

            bool parseSucceded = int.TryParse(lowerCase, out int outInt);
            if (parseSucceded)
            {
                return outInt > 0;
            }
        }
        return def;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var parser = new ConsoleParser(args);

        if (parser.Parsed.Count == 0)
        {
            Console.WriteLine("ConanSharpmake - utility to generate Sharpmake projects from conanbuildinfo.json.");
            Console.WriteLine("Syntax:");
            Console.WriteLine("  ConanSharpmake --inputPath=<Path> --outputPath=<Path> [--namespace=<Namespace>]");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("  inputPath  - directory where conanbuildinfo.json is located.");
            Console.WriteLine("  outputPath - directory where *.sharpmake.cs files would be written.");
            Console.WriteLine("  namespace  - [optional] namespace under which packages would be generated (ConanPackages by default).");
            Environment.Exit(-1);
            return;
        }

        string inputPath = parser.GetOrDefault("inputPath", "");
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new Exception("Parameter inputPath is not set.");
        }
        else
        {
            string absolutePath = Path.GetFullPath(inputPath);
            if (!Directory.Exists(absolutePath))
            {
                throw new Exception("Parameter inputPath does not lead to a directory.");
            }
        }

        string outputPath = parser.GetOrDefault("outputPath", "");
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new Exception("Parameter outputPath is not set.");
        }

        string packageNamespace = parser.GetOrDefault("namespace", Globals.DefaultPackageNamespace);
        if (parser.Parsed.ContainsKey("namespace"))
        {
            Regex namespaceMatcher = new Regex(@"[A-Za-z_][A-Za-z0-9_]*");
            if (!namespaceMatcher.IsMatch(packageNamespace))
            {
                throw new Exception("Parameter namespace is invalid");
            }
        }

        GeneratorParameters parameters = new GeneratorParameters();
        parameters.WorkingDirectory = inputPath;
        parameters.OutputDirectory  = outputPath;
        parameters.PackageNamespace = packageNamespace;

        var generator = new Generator();
        generator.Generate(parameters);
    }
}