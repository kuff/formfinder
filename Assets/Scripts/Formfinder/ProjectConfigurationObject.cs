// Copyright (C) 2024 Peter Guld Leth

#region

using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Formfinder
{
    [CreateAssetMenu(fileName = "ProjectConfigurationObject", menuName = "Formfinder/Project Configuration")]
    public class ProjectConfigurationObject : ScriptableObject
    {
        [Header("Editor Configuration")]
        [Tooltip(
            "If true, the game will start from the initiating scene specified above. If false, it will use the default Unity scene loading behavior.")]
        public bool useInitiatingScene = true;

        [FormerlySerializedAs("startScenePath")]
        [Tooltip("The path to the initiating scene. This is used when 'Use Initiating Scene' is enabled.")]
        public string initiatingScenePath;

        [FormerlySerializedAs("resourcesPath")]
        [Tooltip(
            "The path where Colorcrush resources are stored. This should be a subfolder of the Unity 'Resources' folder.")]
        public string mainResourcesPath = "Assets/Resources/Colorcrush";

        [FormerlySerializedAs("scenesPath")] [Tooltip("The path where Colorcrush scenes are stored.")]
        public string mainScenesPath = "Assets/Scenes";

        [Tooltip("The path where Colorcrush scripts are stored.")]
        public string mainScriptsPath = "Assets/Scripts/Colorcrush";

        [Header("Python Configuration")]
        [Tooltip("The name of the folder containing the Python environment relative to the project root.")]
        public string pythonEnvFolderName = "PythonEnv";

        [Tooltip("The name of the virtual environment folder within the Python environment folder.")]
        public string virtualEnvFolderName = "env";

        [Tooltip("The name of the Python DLL file to be used.")]
        public string pythonDllName = "python310.dll";

        [Tooltip(
            "Additional paths to be added to the Python sys.path. Each path should be relative to the project root.")]
        public string[] additionalPythonPaths;
    }
}