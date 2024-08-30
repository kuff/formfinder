// Copyright (C) 2024 Peter Guld Leth

#region

using System;
using TMPro;
using UnityEngine;

#endregion

namespace Formfinder
{
    public class TestSceneController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI outputText;

        private void Start()
        {
            Debug.Log("Running Python script...");

            try
            {
                // Get the Python module using PythonManager
                var helloScript = PythonManager.GetPythonModule("hello_script");

                if (helloScript != null)
                {
                    string message = helloScript.get_hello_message();
                    Debug.Log($"Message from Python: {message}");

                    // Update the TextMeshProUGUI component with the message
                    outputText.text = message;
                }
                else
                {
                    Debug.LogError("Failed to import hello_script module");
                    outputText.text = "Error: Failed to import Python module";
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error running Python script: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
                outputText.text = "Error: Failed to run Python script";
            }
        }
    }
}