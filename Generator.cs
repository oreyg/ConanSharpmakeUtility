using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ConanSharpmake
{
    public static class Globals
    {
    }

    public class CMakeFilenames
    {
        [JsonPropertyName("cmake_find_package")]
        public string FindPackageName { get; set; }

        [JsonPropertyName("cmake_find_package_multi")]
        public string FindPackageMultiName { get; set; }
        
        [JsonPropertyName("pkg_config")]
        public string PKGConfig { get; set; }

        public CMakeFilenames()
        {
            FindPackageName      = "";
            FindPackageMultiName = "";
            PKGConfig            = "";
        }
    }

    public class ConanSettings
    {
        [JsonPropertyName("arch")]
        public string ConanArch { get; set; }

        [JsonPropertyName("arch_build")]
        public string ConanArchBuild { get; set; }

        [JsonPropertyName("build_type")]
        public string ConanBuildType { get; set; }

        [JsonPropertyName("compiler")]
        public string ConanCompiler { get; set; }

        [JsonPropertyName("compiler.runtime")]
        public string ConanCompilerRuntime { get; set; }

        [JsonPropertyName("compiler.version")]
        public string ConanCompilerVersion { get; set; }

        [JsonPropertyName("os")]
        public string ConanOS { get; set; }

        [JsonPropertyName("os_build")]
        public string ConanOSBuild { get; set; }

        [JsonIgnore]
        public string DevEnv
        {
            get
            {
                // TODO: test and expand
                if (ConanCompiler == "Visual Studio")
                {
                    switch (ConanCompilerVersion)
                    {
                        case "15":
                            return "DevEnv.vs2015";
                        case "17":
                            return "DevEnv.vs2017";
                        case "19":
                            return "DevEnv.vs2019";
                        case "22":
                            return "DevEnv.vs2019";
                    }
                }
                return "";
            }
        }

        [JsonIgnore]
        public string Platform
        {
            get
            {
                // TODO: test and expand
                // To experiment: try building package for exotic platforms
                if (ConanOS == "Windows")
                {
                    switch (ConanArch)
                    {
                        case "x86_64":
                            return "Platform.win64";
                        case "x86_86": // ?
                            return "Platform.win32";
                    }
                }
                return "";
            }
        }

        [JsonIgnore]
        public string Optimization
        {
            get
            {
                // TODO: make it proper
                return "Optimization.Debug | Optimization.Release";
            }
        }

        public ConanSettings()
        {
            ConanArch            = "";
            ConanArchBuild       = "";
            ConanBuildType       = "";
            ConanCompiler        = "";
            ConanCompilerRuntime = "";
            ConanCompilerVersion = "";
            ConanOS              = "";
            ConanOSBuild         = "";
        }
    }

    public class ConanDependency
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("rootpath")]
        public string RootPath { get; set; }

        [JsonPropertyName("sysroot")]
        public string SysRoot { get; set; }

        [JsonPropertyName("include_paths")]
        public IEnumerable<string> IncludePaths { get; set; }

        [JsonPropertyName("lib_paths")]
        public IEnumerable<string> LibPaths { get; set; }

        [JsonPropertyName("bin_paths")]
        public IEnumerable<string> BinPaths { get; set; }

        [JsonPropertyName("build_paths")]
        public IEnumerable<string> BuildPaths { get; set; }

        [JsonPropertyName("res_paths")]
        public IEnumerable<string> ResPaths { get; set; }

        [JsonPropertyName("libs")]
        public IEnumerable<string> Libs { get; set; }

        [JsonPropertyName("system_libs")]
        public IEnumerable<string> SystemLibs { get; set; }

        [JsonPropertyName("defines")]
        public IEnumerable<string> Defines { get; set; }

        [JsonPropertyName("cflags")]
        public IEnumerable<string> CFlags { get; set; }

        [JsonPropertyName("cxxflags")]
        public IEnumerable<string> CXXFlags { get; set; }

        [JsonPropertyName("sharedlinkflags")]
        public IEnumerable<string> SharedLinkFlags { get; set; }

        [JsonPropertyName("exelinkflags")]
        public IEnumerable<string> EXELinkFlags { get; set; }

        [JsonPropertyName("frameworks")]
        public IEnumerable<string> Frameworks { get; set; }

        [JsonPropertyName("framework_paths")]
        public IEnumerable<string> FrameworkPaths { get; set; }

        [JsonPropertyName("names")]
        public CMakeFilenames? Names { get; set; }

        [JsonPropertyName("filenames")]
        public CMakeFilenames? Filenames { get; set; }

        [JsonPropertyName("build_modules")]
        public object? BuildModules { get; set; }

        [JsonPropertyName("build_modules_paths")]
        public object? BuildModulesPaths { get; set; }

        [JsonPropertyName("cppflags")]
        public IEnumerable<string> CPPFlags { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string PreferredName
        { 
            get
            {
                string result = Name;
                if (Names != null && !string.IsNullOrWhiteSpace(Names.FindPackageName))
                {
                    // Prefer names that can be used as a classname
                    Regex matcher = new Regex(@"[A-Za-z_][A-Za-z0-9_]*");
                    if (matcher.IsMatch(Names.FindPackageName))
                    {
                        result = Names.FindPackageName;
                    }
                }

                if (result.Length > 0)
                {
                    // Remove all '-' and capitalize next char
                    while (result.Contains('-'))
                    {
                        int index = result.IndexOf('-');
                        if (index + 1 < result.Length)
                        {
                            char toReplace = char.ToUpper(result[index + 1]);
                            result = result.Remove(index, 2);
                            result = result.Insert(index, toReplace.ToString());
                        }
                        else
                        {
                            result = result.Remove(index, 1);
                        }
                    }

                    // Capitalize first char
                    if (!char.IsUpper(result[0]))
                    {
                        char toReplace = char.ToUpper(result[0]);
                        result = result.Remove(0, 1);
                        result = result.Insert(0, toReplace.ToString());
                    }
                }

                return result;
            }
        }

        public ConanDependency()
        {
            Version         = "";
            Description     = "";
            RootPath        = "";
            SysRoot         = "";
            IncludePaths    = Array.Empty<string>();
            LibPaths        = Array.Empty<string>();
            BinPaths        = Array.Empty<string>();
            BuildPaths      = Array.Empty<string>();
            ResPaths        = Array.Empty<string>();
            Libs            = Array.Empty<string>();
            SystemLibs      = Array.Empty<string>();
            Defines         = Array.Empty<string>();
            CFlags          = Array.Empty<string>();
            CXXFlags        = Array.Empty<string>();
            SharedLinkFlags = Array.Empty<string>();
            EXELinkFlags    = Array.Empty<string>();
            Frameworks      = Array.Empty<string>();
            FrameworkPaths  = Array.Empty<string>();
            CPPFlags        = Array.Empty<string>();
            Name            = "";
        }

    }

    public class ConanBuildInfo
    {
        [JsonPropertyName("dependencies")]
        public IEnumerable<ConanDependency>? Dependencies { get; set; }

        [JsonPropertyName("settings")]
        public ConanSettings? Settings { get; set; }
    }

    public class GeneratorParameters
    {
        public string WorkingDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string PackageNamespace { get; set; }

        public GeneratorParameters()
        {
            WorkingDirectory = "";
            OutputDirectory  = "";
            PackageNamespace = "";
        }
    }

    public class Generator
    {

        public void GenerateOne(ConanDependency dependency, ConanSettings? settings, string[] libIgnoreList, string outDirectory, string packageNamespace)
        {
            using StreamWriter file = new($"{outDirectory}/{dependency.Name}.sharpmake.cs", false);

            file.WriteLine(@"/* ---       Conan Sharpmake        --- */");
            file.WriteLine(@"/* --- This file is Autogenerated!  --- */");
            file.WriteLine();

            file.WriteLine(@"using Sharpmake;");

            file.WriteLine();

            file.WriteLine($"namespace {packageNamespace}");
            file.WriteLine(@"{");
            file.WriteLine();

            string prefferedName = dependency.PreferredName;
            string projectName = $"{prefferedName}Project";
            file.WriteLine(@"  [Export]");
            file.WriteLine($"  public class {projectName} : Project");
            file.WriteLine(@"  {");
            file.WriteLine();
            file.WriteLine($"    public {projectName}()"); // Default constructor
            file.WriteLine(@"    {");
            file.WriteLine($"       Name = \"{prefferedName}\";");

            // TODO: fix that
            // Right now we override platform, environment and optimization level,
            // maybe it makes sense to expose that as console inputs
            string platform = "Platform.win64";
            string devenv = "DevEnv.vs2022";
            string optimization = "Optimization.Debug | Optimization.Release";
            //if (settings != null)
            //{
            //    platform = settings.Platform;
            //    devenv = settings.DevEnv;
            //    optimization = settings.Optimization;
            //}

            file.WriteLine($"       AddTargets(new Target({platform}, {devenv}, {optimization}));");
            file.WriteLine(@"    }");

            file.WriteLine();
            file.WriteLine(@"    [Configure]");
            file.WriteLine($"    public void Configure(Configuration conf, Target target)"); // Configure
            file.WriteLine(@"    {");

            if (dependency.IncludePaths.Count() > 0)
            {
                file.WriteLine(@"      // --- Include directives ---");
                foreach (var include in dependency.IncludePaths)
                {
                    file.WriteLine($"      conf.IncludePaths.Add(@\"{include}\");");
                }
            }

            if (dependency.LibPaths.Count() > 0)
            {
                file.WriteLine();
                file.WriteLine(@"      // --- Library path directives ---");
                foreach (var libPaths in dependency.LibPaths)
                {
                    file.WriteLine($"      conf.LibraryPaths.Add(@\"{libPaths}\");");
                }
            }
            
            if (dependency.Libs.Count() > 0)
            {
                file.WriteLine();
                file.WriteLine(@"      // --- Library file directives ---");
                foreach (var libFiles in dependency.Libs)
                {
                    if (libIgnoreList.Contains(libFiles))
                    {
                        continue;
                    }

                    file.WriteLine($"      conf.LibraryFiles.Add(@\"{libFiles}\");");
                }
            }

            if (dependency.Defines.Count() > 0)
            {
                file.WriteLine();
                file.WriteLine(@"      // --- Defines ---");
                foreach (var defines in dependency.Defines)
                {
                    file.WriteLine($"      conf.Defines.Add(@\"{defines}\");");
                }
            }

            if (dependency.BinPaths.Count() > 0)
            {
                // Windows only
                string ext = "*.dll";
                // if (Linux)
                // {
                //    ext = "*.so";
                // }

                bool createdComment = false;
                file.WriteLine();
                foreach (string binPath in dependency.BinPaths)
                {
                    if (!createdComment)
                    {
                        file.WriteLine(@"      // --- DLL Copies ---");
                        createdComment = true;
                    }

                    string[] dllFiles = Directory.GetFiles(binPath, ext);
                    foreach (string dllFile in dllFiles)
                    {
                        file.WriteLine($"      conf.TargetCopyFiles.Add(@\"{dllFile}\");");
                    }
                }
            }

            file.WriteLine(@"    }");
            file.WriteLine();
            file.WriteLine(@"  }");

            file.WriteLine();
            file.WriteLine(@"}");
        }

        public void Generate(GeneratorParameters parameters)
        {
            Console.WriteLine(@"ConanSharpmake - generating packages");
            Console.WriteLine($"Input directory  : {parameters.WorkingDirectory}");
            Console.WriteLine($"Output directory : {parameters.OutputDirectory}");
            Console.WriteLine($"Package namespace: {parameters.PackageNamespace}");

            string workingDirectory = parameters.WorkingDirectory;
            string buildInfoPath = workingDirectory + "/conanbuildinfo.json";
            string buildInfoText = File.ReadAllText(buildInfoPath);

            // Optional ignorelibs
            var ignoreListPath = workingDirectory + "/ignorelibs.txt";
            var ignoreListText = Array.Empty<string>();
            if (File.Exists(ignoreListPath))
            {
                ignoreListText = File.ReadAllLines(ignoreListPath);
            }
            else
            {
                Console.WriteLine("Warning: ignorelibs.txt not found, continuing without ignore list.");
            }

            var buildInfo = JsonSerializer.Deserialize<ConanBuildInfo>(buildInfoText);
            if (buildInfo == null)
            {
                throw new Exception("Cannot read conanbuildinfo.json as JSON. Please, make sure that provided file is not empty.");
            }

            if (buildInfo.Dependencies == null)
            {
                throw new Exception("Cannot read conanbuildinfo.json. Cannot find dependencies.");
            }

            foreach (var dependency in buildInfo.Dependencies)
            {
                GenerateOne(dependency, buildInfo.Settings, ignoreListText, parameters.OutputDirectory, parameters.PackageNamespace);
            }

            Console.WriteLine($"Successfully generated {buildInfo.Dependencies.Count()} packages.");
        }

    }

}
