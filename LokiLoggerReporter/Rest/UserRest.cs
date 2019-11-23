using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LokiLogger.WebExtension.Controller;
using LokiLogger.WebExtension.ViewModel;
using lokiloggerreporter.Services.Implementation;
using lokiloggerreporter.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Rest
{
    [Authorize]
    [Route("api/User")]
    public class UserRest : Controller
    {
        public UserService UserService { get; set; }
        
        public UserRest(UserService userService)
        {
            UserService = userService;
        }
        
        [HttpGet("All")]
        public IActionResult GetAllUser()
        {
            var res = UserService.GetAllUser();
            if (res.Succeeded)
            {
                return Ok(res.SuccessResult);
            }
            else
            {
                return BadRequest(res.Errors);
            }
        }
        [Authorize(Roles = Services.Implementation.UserService.AdminRole)]
        [HttpPost("Create")]
        public async Task<IActionResult> GetCreateUser([FromBody] UserCreateModel model)
        {
            var res = await UserService.CreateUser(model);
            if (res.Succeeded)
            {
                return Ok(res.SuccessResult);
            }
            else
            {
                return BadRequest(res.Errors);
            }
        }
        
        [Authorize(Roles = Services.Implementation.UserService.AdminRole)]
        [HttpPut("Update/{userId}")]
        public async Task<IActionResult> UpdateUser([FromRoute]string userId,[FromBody] UserUpdateModel model)
        {
            
            var res = await UserService.UpdateUser(userId,model);
            if (res.Succeeded)
            {
                return Ok(res.SuccessResult);
            }
            else
            {
                return BadRequest(res.Errors);
            }
        }
        
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            
            var res = await UserService.Login(model);
            if (res.Succeeded)
            {
                return Ok(res.SuccessResult);
            }
            else
            {
                return BadRequest(res.Errors);
            }
        }

        [Authorize(Roles = Services.Implementation.UserService.AdminRole)]
        [HttpGet("AbleUser/{userId}/{disable}")]
        public async Task<IActionResult> AbleUser([FromRoute] string userId, [FromRoute] bool disable)
        {
            
            var res = await UserService.AbleUser(disable,userId);
            if (res.Succeeded)
            {
                return Ok(res.SuccessResult);
            }
            else
            {
                return BadRequest(res.Errors);
            }
        }
        [AllowAnonymous]
        [HttpGet("NotAuth")]
        public IActionResult NotAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok();
            }
            else
            {
                var res = OperationResult<bool>.Failed<bool>("Auth", "Not Authorized").Errors;
                return Ok(res);
            }
        }

        [AllowAnonymous]
        [HttpGet("Cookies")]
        public IActionResult GetCookies()
        {
            if(Request.Cookies.Count != 0){
                var result = new List<string>()
                {
                    Request.Cookies.Select(x => x.Key + " : " + x.Value + "\n").Aggregate((a, b) => a + b)
                };
                return Ok(result);
            }

            return Ok();
        }
    }
}