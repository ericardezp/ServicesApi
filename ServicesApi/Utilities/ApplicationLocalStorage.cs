namespace ServicesApi.Utilities
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class ApplicationLocalStorage : IApplicationAzureStorage
    {
        private readonly IWebHostEnvironment environment;

        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationLocalStorage(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFile(string container, IFormFile formFile)
        {
            var fileExtension = Path.GetExtension(formFile.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(this.environment.WebRootPath, container);
            if (Equals(!Directory.Exists(fullPath)))
            {
                Directory.CreateDirectory(fullPath);
            }

            var filePath = Path.Combine(fullPath, fileName);
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();
                await File.WriteAllBytesAsync(filePath, fileContent);
            }

            var actualUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var pathFileDb = Path.Combine(actualUrl, container, fileName).Replace("\\", "/");
            return pathFileDb;
        }

        public Task DeleteFile(string filePath, string container)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return Task.CompletedTask;
            }

            var fileName = Path.GetFileName(filePath);
            var fileDirectory = Path.Combine(this.environment.WebRootPath, container, fileName);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }

        public async Task<string> EditFile(string container, IFormFile formFile, string filePath)
        {
            await this.DeleteFile(filePath, container);
            return await this.SaveFile(container, formFile);
        }
    }
}