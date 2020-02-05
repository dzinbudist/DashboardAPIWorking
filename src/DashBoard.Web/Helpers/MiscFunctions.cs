using System.Security.Claims;

namespace DashBoard.Web.Helpers
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
