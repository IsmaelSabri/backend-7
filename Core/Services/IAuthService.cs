using Core.Models;

namespace Core.Services
{
    public interface IAuthService
    {
        Task RequestPasswordReset(string email);
    }
}
