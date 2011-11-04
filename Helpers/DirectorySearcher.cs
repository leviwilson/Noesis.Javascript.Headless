using System.Collections.Generic;
using System.IO;

namespace Noesis.Javascript.Headless.Helpers
{
    public static class DirectorySearcher
    {
        public static List<string> GetFilesRecursive(string baseDirectory, string searchPattern = "*.*")
        {
            var foundFiles = new List<string>();
            var directoryStack = new Stack<string>();

            directoryStack.Push(baseDirectory);

            while (directoryStack.Count > 0)
            {
                var currentDirectory = directoryStack.Pop();

                foundFiles.AddRange(Directory.GetFiles(currentDirectory, searchPattern));

                foreach (var directory in Directory.GetDirectories(currentDirectory))
                    directoryStack.Push(directory);
            }

            return foundFiles;
        }
    }
}