using LMS.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Presentation.Attributes
{
    public class AuthorizeRoles : AuthorizeAttribute
    {
        public AuthorizeRoles(params RoleListEnum[] roleLists)
        {
            Roles = string.Join(",", roleLists.Select(x => x.ToString()));
        }
    }
}
