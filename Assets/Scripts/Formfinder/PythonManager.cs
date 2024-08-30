// Copyright (C) 2024 Peter Guld Leth

#region

using System;
using System.IO;
using Python.Runtime;
using UnityEngine;

#endregion

namespace Formfinder
{
    public class PythonManager : MonoBehaviour
    {
        private static PythonManager _instance;
        private bool _isInitialized;

        public static PythonManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("PythonManager");
                    _instance = go.AddComponent<PythonManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePythonEnvironment();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            if (_isInitialized)
            {
                PythonEngine.Shutdown();
                Debug.Log("Python engine shut down");
            }
        }

        private void InitializePythonEnvironment()
        {
            try
            {
                var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
                var pythonHome = Path.Combine(projectRoot, "PythonEnv", "env");
                var pythonPath = Path.Combine(projectRoot, "PythonEnv");

                Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome);
                Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath);

                var pythonDll = Path.Combine(pythonHome, "python310.dll");
                if (!File.Exists(pythonDll)) throw new FileNotFoundException($"Python DLL not found: {pythonDll}");
                Runtime.PythonDLL = pythonDll;

                PythonEngine.Initialize();
                _isInitialized = true;
                Debug.Log("Python environment initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing Python environment: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
            }
        }

        public static dynamic GetPythonModule(string moduleName)
        {
            if (!Instance._isInitialized)
            {
                Debug.LogError("Python environment is not initialized.");
                return null;
            }

            try
            {
                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
                    var pythonPath = Path.Combine(projectRoot, "PythonEnv");
                    sys.path.append(pythonPath);

                    return Py.Import(moduleName);
                }
            }
            catch (PythonException pyEx)
            {
                Debug.LogError($"Python Exception when importing {moduleName}: {pyEx.Message}");
                Debug.LogError($"Python Traceback: {pyEx.StackTrace}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error importing Python module {moduleName}: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}