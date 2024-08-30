// Copyright (C) 2024 Peter Guld Leth

#region

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#endregion

namespace Editor
{
    public abstract class BuildPostProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // Define source and destination paths
            var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            var sourceDir = Path.Combine(projectRoot, "PythonEnv");
            var destinationDir = Path.Combine(Path.GetDirectoryName(pathToBuiltProject)!, "PythonEnv");

            // Copy the Python environment to the build folder
            if (Directory.Exists(destinationDir)) Directory.Delete(destinationDir, true);

            DirectoryCopy(sourceDir, destinationDir, true);
            Debug.Log("Python environment copied to build directory.");

            // Copy all Python files from the root of PythonEnv to the build folder
            var pythonFiles = Directory.GetFiles(sourceDir, "*.py");
            foreach (var pythonFile in pythonFiles)
            {
                var fileName = Path.GetFileName(pythonFile);
                var destPath = Path.Combine(destinationDir, fileName);
                File.Copy(pythonFile, destPath, true);
                Debug.Log($"{fileName} copied to build directory.");
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " +
                                                     sourceDirName);

            // If the destination directory does not exist, create it.
            Directory.CreateDirectory(destDirName);

            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true); // Set overwrite to true
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
        }
    }
}