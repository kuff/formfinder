// Copyright (C) 2024 Peter Guld Leth

#region

using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Formfinder
{
    public class TestSceneController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI outputText;
        [SerializeField] private Image diffusionImage;
        [SerializeField] private Camera mainCamera;

        private dynamic _diffusionTest;
        private dynamic _stream;

        private void Start()
        {
            InitializeDiffusion();
            diffusionImage.enabled = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(GenerateDiffusionImage());
            else if (Input.GetMouseButtonDown(1) || Input.mouseScrollDelta.y != 0) HideImage();
        }

        private void InitializeDiffusion()
        {
            _diffusionTest = PythonManager.GetPythonModule("diffusion_test");

            if (_diffusionTest == null)
            {
                outputText.text = "Error: Failed to import Python module";
                return;
            }

            Debug.Log("Initializing diffusion...");
            var config = ProjectConfig.InstanceConfig;
            _stream = _diffusionTest.initialize_diffusion(max_retries: config.maxDiffusionRetries,
                retry_delay: config.diffusionRetryDelay);
            Debug.Log("Diffusion initialized successfully!");
        }

        private void HideImage()
        {
            diffusionImage.enabled = false;
            outputText.text = "Image hidden. Generate a new image to show.";
        }

        private IEnumerator GenerateDiffusionImage()
        {
            if (_diffusionTest == null || _stream == null)
            {
                outputText.text = "Error: Diffusion not initialized";
                yield break;
            }

            outputText.text = "Generating image...";

            const string prompt = "A modern villa surrounded by a lush garden";

            // Capture the current camera view
            RenderTexture tempRT = new RenderTexture(512, 512, 24);
            mainCamera.targetTexture = tempRT;
            mainCamera.Render();
            RenderTexture.active = tempRT;

            Texture2D screenshot = new Texture2D(512, 512, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
            screenshot.Apply();

            // Reset camera to render to the screen
            mainCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(tempRT);

            // Convert texture to PNG byte array
            byte[] screenshotBytes = screenshot.EncodeToPNG();

            // Generate image using the screenshot data
            byte[] imageBytes = _diffusionTest.generate_image(_stream, prompt, screenshotBytes);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                outputText.text = "Error: Failed to generate diffusion image";
                yield break;
            }

            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            diffusionImage.sprite = sprite;
            diffusionImage.enabled = true;

            outputText.text = "Diffusion image generated successfully!";

            yield return null;
        }
    }
}