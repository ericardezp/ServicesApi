namespace ServicesApi.Utilities
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IApplicationAzureStorage
    {
        Task<string> SaveFile(string container, IFormFile formFile);

        Task DeleteFile(string filePath, string container);

        Task<string> EditFile(string container, IFormFile formFile, string filePath);
    }
}