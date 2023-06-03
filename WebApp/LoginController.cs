using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Net.Http.Headers;

namespace WebApp
{
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IAccountDatabase _db;

        public LoginController(IAccountDatabase db)
        {
            _db = db;
        }

        [HttpPost("sign-in/{userName}")]
        public async Task<IActionResult> Login([FromRoute]string userName)
        {
            var account = await _db.FindByUserNameAsync(userName);
            if (account != null)
            {
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name, account.ExternalId),
                    new Claim(ClaimTypes.NameIdentifier,account.UserName),
                    new Claim(ClaimTypes.Role,account.Role)
                    };
                
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    principal, new AuthenticationProperties() { IsPersistent = true });
                                
                //TODO 1: Generate auth cookie for user 'userName' with external id

                return Redirect("/api/account"); // i think its a good idea to make a redirect to the account controller,
                                                 // which also calls LoadOrCreateAsync for this account
                // return Ok();
            }
            return NotFound();//TODO 2: return 404 if user not found
        }


    }
}