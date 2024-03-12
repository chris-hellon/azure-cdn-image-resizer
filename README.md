# Azure CDN Image Resizer

Azure CDN Image Resizer is a project designed to dynamically resize images served through Azure Blob Storage CDN. By intercepting Azure Blob CDN URLs and utilizing query parameters, this solution enables on-the-fly image resizing based on specific dimensions provided in the URL.

## How It Works

1. **Interception**: When an image request is made to the Azure Blob CDN URL, the request is intercepted by the Azure CDN Image Resizer service.

2. **Parsing Parameters**: The service parses the query parameters in the URL to determine the desired dimensions for resizing the image.

3. **Image Resizing**: Using the dimensions specified in the URL, the service dynamically resizes the original image stored in Azure Blob Storage.

4. **Delivery**: The resized image is then delivered to the client, fulfilling the original request.

## Features

- **Dynamic Image Resizing**: Resize images on-the-fly by specifying dimensions in the query parameters of the Azure Blob CDN URL.
- **Preserve Aspect Ratio**: Maintain the aspect ratio of the original image during resizing to prevent distortion.
- **Caching**: Implement caching mechanisms to optimize performance and reduce processing overhead for frequently requested images.
- **Customizable**: Configure the resizing behavior and caching policies according to specific requirements and performance considerations.
- **Scalable**: Built on Azure infrastructure, allowing seamless scaling to accommodate increasing demands and traffic.

## Tech Stack

- **Azure Blob Storage**: Store original images securely and efficiently.
- **Azure CDN**: Deliver images with low latency and high throughput globally.
- **Azure Functions**: Implement serverless computing for intercepting and resizing image requests.
- **C#**: Utilize C# programming language for developing Azure Functions.
- **.NET Core**: Build and deploy Azure Functions using .NET Core framework.
- **Azure Portal**: Manage and monitor the solution through the Azure Portal interface.
