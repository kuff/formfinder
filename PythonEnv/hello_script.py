def get_hello_message():
    import torch
    cuda_available = torch.cuda.is_available()
    return f"PyTorch will run with CUDA: {cuda_available}"
