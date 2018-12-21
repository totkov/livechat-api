using System;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

namespace LiveChat.API.Controllers
{
    public class BaseController : Controller
    {
        public int LoggedUserId
        {
            get
            {
                try
                {
                    var userIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
                    if (userIdClaim != null)
                    {
                        return int.Parse(userIdClaim.Value);
                    }

                    return -1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }
    }
}
