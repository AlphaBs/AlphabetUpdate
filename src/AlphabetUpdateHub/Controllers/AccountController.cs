using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateHub.Controllers
{
    [Route("accounts")]
    [ApiController]
    [Authorize]
    public class AccountController
    {
        public AccountController()
        {

        }
    }
}