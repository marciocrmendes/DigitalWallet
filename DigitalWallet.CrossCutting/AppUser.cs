using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DigitalWallet.CrossCutting
{
    public sealed class AppUser(IHttpContextAccessor httpContextAccessor)
    {
        private Guid _userId;
        private string? _userName;

        public Guid UserId
        {
            get
            {
                if (_userId != Guid.Empty)
                    return _userId;

                var userId = httpContextAccessor
                    .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
                    ?.Value;

                _ = Guid.TryParse(userId, out var id);

                return _userId = id;
            }
        }

        public string UserName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_userName))
                    return _userName;

                return _userName =
                    httpContextAccessor
                        .HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Name)
                        ?.Value ?? string.Empty;
            }
        }
    }
}
