using Hourglass.Models;

namespace Hourglass.Services.Interfaces
{
    public interface ITokenService
    {
        string Generate(User user);
    }
}
