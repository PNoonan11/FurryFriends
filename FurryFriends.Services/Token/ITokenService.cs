using FurryFriends.Data.Entities;
using FurryFriends.Models.Token;

namespace FurryFriends.Services.Token
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest model);

    }
}