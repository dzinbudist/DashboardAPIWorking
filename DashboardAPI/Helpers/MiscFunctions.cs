using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebApi.Helpers
{
    public static class MiscFunctions
    {
        public static int GetCurentUser(ClaimsPrincipal claim)
        {
            var currentUserName = claim.FindFirst(ClaimTypes.Name).Value;

            if (int.TryParse(currentUserName, out int userId))
            {
                return userId;
            }
            else
            {
                return 0;
            }
        }
    }
}
