#define IS_BUILD_IN_SOLUTION

using System;
using System.IO;

namespace R3Modeller.Core {
    public class ResourceLocator {
        public static readonly string AppDir;
        public static readonly string ResourceDirectory;

        static ResourceLocator() {
            AppDir = Directory.GetCurrentDirectory();
#if IS_BUILD_IN_SOLUTION
            // Where <root> is the user projects folder (e.g. C:\Users\<user>\source\repos, E:\VSProjects, etc)
            // Solution build folder structure is: <root>\R3Modeller\R3Modeller\bin\x64\Debug
            // Meaning to reach the project directory, go back 4 times
            AppDir = Path.GetFullPath(Path.Combine(AppDir, "..\\..\\..\\..\\"));
#endif
            ResourceDirectory = Path.Combine(AppDir, "Resources");
        }

        public static void Setup() {
            if (!Directory.Exists(AppDir)) {
                throw new Exception("App launch directory not found");
            }

            if (!Directory.Exists(ResourceDirectory)) {
                throw new Exception("Resource directory not found");
            }
        }

        public static string GetResourceFile(string path) {
            return Path.Combine(ResourceDirectory, path);
        }

        public static string ReadFile(string path) {
            string filePath = GetResourceFile(path);
            return File.ReadAllText(filePath);
        }

        public static string[] ReadFileLines(string path) {
            string filePath = GetResourceFile(path);
            return File.ReadAllLines(filePath);
        }
    }
}