namespace ServicesApi.Utilities
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public class ApplicationAzureStorage : IApplicationAzureStorage
    {
        // Connection string for azure storage instance
        private readonly string azureConnectionString;
        public ApplicationAzureStorage(IConfiguration configuration)
        {
            this.azureConnectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<string> SaveFile(string container, IFormFile formFile)
        {
            var blobContainerClient = new BlobContainerClient(this.azureConnectionString, container);
            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var fileExtension = Path.GetExtension(formFile.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(formFile.OpenReadStream());

            return blobClient.Uri.ToString();
        }

        public async Task DeleteFile(string filePath, string container)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var blobContainerClient = new BlobContainerClient(this.azureConnectionString, container);
            await blobContainerClient.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(filePath);
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(string container, IFormFile formFile, string filePath)
        {
            await this.DeleteFile(filePath, container);
            return await this.SaveFile(container, formFile);
        }
    }
}