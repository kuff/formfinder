// Copyright (C) 2024 Peter Guld Leth

#region

using System;
using System.IO;
using Python.Runtime;
using TMPro;
using UnityEngine;

#endregion

namespace Formfinder
{
    public class PythonInterop : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI outputText;

        private void Start()
        {
            Debug.Log("Running Python script...");

            try
            {
                // Adjust the path to your Python environment
                var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
                var pythonHome = Path.Combine(projectRoot, "PythonEnv", "env");
                var pythonPath = Path.Combine(projectRoot, "PythonEnv");

                Debug.Log($"Project Root: {projectRoot}");
                Debug.Log($"Python Home: {pythonHome}");
                Debug.Log($"Python Path: {pythonPath}");

                // Set the PYTHONHOME environment variable
                Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome);
                Debug.Log($"PYTHONHOME set to: {Environment.GetEnvironmentVariable("PYTHONHOME")}");

                // Set the PYTHONPATH environment variable to include your scripts directory
                Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath);
                Debug.Log($"PYTHONPATH set to: {Environment.GetEnvironmentVariable("PYTHONPATH")}");

                // Set the Runtime.PythonDLL property
                var pythonDll = Path.Combine(pythonHome, "python310.dll");
                if (!File.Exists(pythonDll)) throw new FileNotFoundException($"Python DLL not found: {pythonDll}");
                Runtime.PythonDLL = pythonDll;
                Debug.Log($"Python DLL set to: {Runtime.PythonDLL}");

                // Initialize the Python engine
                PythonEngine.Initialize();
                Debug.Log("Python engine initialized");

                // var a = 0;
                // var _ = a / a;

                using (Py.GIL())
                {
                    Debug.Log("Attempting to import hello_script...");
                    try
                    {
                        dynamic sys = Py.Import("sys");
                        Debug.Log($"Python sys.path before: {sys.path}");

                        // Add the PythonEnv directory to sys.path
                        sys.path.append(pythonPath);
                        Debug.Log($"Python sys.path after: {sys.path}");

                        Debug.Log($"Current Directory: {Environment.CurrentDirectory}");

                        dynamic helloScript = Py.Import("hello_script");
                        Debug.Log("hello_script imported successfully");
                        string message = helloScript.get_hello_message();
                        Debug.Log($"Message from Python: {message}");

                        // Update the TextMeshProUGUI component with the message
                        outputText.text = message;
                    }
                    catch (PythonException pyEx)
                    {
                        Debug.LogError($"Python Exception: {pyEx.Message}");
                        Debug.LogError($"Python Traceback: {pyEx.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing Python: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
                outputText.text = "Error: Failed to initialize Python";
            }
            finally
            {
                // Shut down the Python engine
                PythonEngine.Shutdown();
                Debug.Log("Python engine shut down");
            }
        }
    }
}