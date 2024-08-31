import torch
from diffusers import AutoencoderTiny, StableDiffusionPipeline
from streamdiffusion import StreamDiffusion
from streamdiffusion.image_utils import postprocess_image
import numpy as np
import io
import requests
from requests.exceptions import ConnectionError, Timeout
import time

def initialize_diffusion(max_retries, retry_delay):

    for attempt in range(max_retries):
        try:
            # Load the StableDiffusionPipeline
            pipe = StableDiffusionPipeline.from_pretrained("KBlueLeaf/kohaku-v2.1", 
                                                           local_files_only=False, 
                                                           resume_download=True).to(
                device=torch.device("cuda"),
                dtype=torch.float16,
            )

            # Wrap the pipeline in StreamDiffusion
            stream = StreamDiffusion(
                pipe,
                t_index_list=[0, 16, 32, 45],
                torch_dtype=torch.float16,
                cfg_type="none",
            )

            # Merge LCM and use Tiny VAE for acceleration
            stream.load_lcm_lora()
            stream.fuse_lora()
            stream.vae = AutoencoderTiny.from_pretrained("madebyollin/taesd", 
                                                         local_files_only=False, 
                                                         resume_download=True).to(device=pipe.device, dtype=pipe.dtype)

            # Enable TensorRT acceleration
            """from streamdiffusion.acceleration.tensorrt import accelerate_with_tensorrt
            stream = accelerate_with_tensorrt(
                stream, "engines", max_batch_size=2,
            )"""
            pipe.enable_xformers_memory_efficient_attention()

            return stream

        except (ConnectionError, Timeout) as e:
            if attempt < max_retries - 1:
                print(f"Connection error occurred. Retrying in {retry_delay} seconds...")
                time.sleep(retry_delay)
            else:
                print("Max retries reached. Unable to initialize diffusion.")
                raise e

def generate_image(stream, prompt):
    # Prepare the stream
    stream.prepare(prompt)

    # Warmup
    for _ in range(4):
        stream()

    # Generate the image
    x_output = stream.txt2img()
    image = postprocess_image(x_output, output_type="pil")[0]

    # Convert PIL image to bytes
    img_byte_arr = io.BytesIO()
    image.save(img_byte_arr, format='PNG')
    img_byte_arr = img_byte_arr.getvalue()

    return img_byte_arr