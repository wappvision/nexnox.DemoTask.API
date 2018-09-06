using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace nexnox.DemoTask.API.Data
{
    public interface ITokenProvider<TUser>
    {
        string CreateToken(TUser user, DateTime expiry, List<Claim> claims);
    }
}
