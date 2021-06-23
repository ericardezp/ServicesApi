namespace ServicesApi.Utilities
{
    using System.Threading.Tasks;

    using ServicesApi.DTOs;

    public interface ITokenGenerator
    {
        Task<ResponseAuthentication> GenerateJwt(UserData userData);
    }
}