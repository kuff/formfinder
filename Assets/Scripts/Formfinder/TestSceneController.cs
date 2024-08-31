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
            else if (Input.GetMouseButtonDown(1)) HideImage();
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

            const string prompt = "A beautiful landscape with mountains and a lake";
            byte[] imageBytes = _diffusionTest.generate_image(_stream, prompt);

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