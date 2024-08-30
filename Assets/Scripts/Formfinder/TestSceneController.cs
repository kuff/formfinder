// Copyright (C) 2024 Peter Guld Leth

#region

using System.Collections;
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

        private void Start()
        {
            StartCoroutine(GenerateDiffusionImage());
        }

        private IEnumerator GenerateDiffusionImage()
        {
            Debug.Log("Starting diffusion image generation process...");

            // Get the Python module using PythonManager
            var diffusionTest = PythonManager.GetPythonModule("diffusion_test");

            if (diffusionTest == null)
            {
                Debug.LogError("Failed to import diffusion_test module");
                outputText.text = "Error: Failed to import Python module";
                yield break;
            }

            Debug.Log("Successfully imported diffusion_test module");

            // Initialize diffusion
            Debug.Log("Initializing diffusion...");
            var config = ProjectConfig.InstanceConfig;
            var stream = diffusionTest.initialize_diffusion(max_retries: config.maxDiffusionRetries, retry_delay: config.diffusionRetryDelay);
            Debug.Log("Diffusion initialized successfully");

            // Generate image
            var prompt = "A beautiful landscape with mountains and a lake";
            Debug.Log($"Generating image with prompt: '{prompt}'");
            byte[] imageBytes = diffusionTest.generate_image(stream, prompt);
            Debug.Log("Image generation completed");

            if (imageBytes == null || imageBytes.Length == 0)
            {
                Debug.LogError("Failed to generate diffusion image: Image bytes are null or empty");
                outputText.text = "Error: Failed to generate diffusion image";
                yield break;
            }

            Debug.Log("Creating texture from image bytes...");
            // Create a texture from the image bytes
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            Debug.Log("Texture created successfully");

            Debug.Log("Creating sprite from texture...");
            // Create a sprite from the texture
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Sprite created successfully");

            Debug.Log("Setting sprite to Image component...");
            // Set the sprite to the Image component
            diffusionImage.sprite = sprite;
            Debug.Log("Sprite set to Image component");

            outputText.text = "Diffusion image generated successfully!";
            Debug.Log("Diffusion image generation process completed successfully");

            yield return null;
        }
    }
}