using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripWebData;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController:ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public Results<int> Test()
        {
            return Results<int>.DataResult(1);
        }

    }
}
