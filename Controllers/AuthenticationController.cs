using System;
using System.Collections.Generic;
using lokiloggerreporter.Database;
using lokiloggerreporter.Database.Model;
using lokiloggerreporter.Services;
using lokiloggerreporter.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Controllers {
	[Route("/api/User")]
	public class AuthenticationController :ControllerAbstract{
		public IUserService UserService { get; set; }
		public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, DatabaseCtx context,IUserService userService) : base(userManager, signInManager, context)
		{
			UserService = userService;
		}

		[HttpGet("All")]
		public ActionResult<List<UserModel>> GetAllUser()
		{
			try
			{
				return Ok(UserService.GetAllUser());
			}
			catch (Exception e)
			{
				return BadRequest("Ein Fehler ist aufgetreten");
			}
		}
		
		[Authorize(Roles = "Admin")]
		[HttpPost("Create")]
		public ActionResult<OperationResult> CreateUser([FromBody] UserModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var result = UserService.CreateUser(model.UserName, model.Password, model.IsAdmin);
					if (result.Succeeded)
					{
						return Ok(result.SuccessResult);
					}
					else
					{
						return BadRequest(result.Errors);
					}
				}
				catch (Exception e)
				{
					return BadRequest("Ein Fehler ist aufgetreten");
				}
			}
			else
				return BadRequest(ModelState);
		}
		
		[Authorize(Roles = "Admin")]
		[HttpPut("Update/{userId}")]
		public ActionResult<OperationResult> UpdateUser([FromRoute]string userId,[FromBody] UserModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var result = UserService.UpdateUser(model.User_ID,model.Password, model.IsAdmin);
					if (result.Succeeded)
					{
						return Ok(result.SuccessResult);
					}
					else
					{
						return BadRequest(result.Errors);
					}
				}
				catch (Exception e)
				{
					return BadRequest("Ein Fehler ist aufgetreten");
				}
			}
			else
				return BadRequest(ModelState);
		}
		
		[AllowAnonymous]
		[HttpPost("Login")]
		public ActionResult<OperationResult> Login([FromBody] UserModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var result = UserService.Login(model.UserName, model.Password);
					if (result.Succeeded)
					{
						return Ok(result.SuccessResult);
					}
					else
					{
						return BadRequest(result.Errors);
					}
				}
				catch (Exception e)
				{
					return BadRequest("Ein Fehler ist aufgetreten");
				}
			}
			else
				return BadRequest(ModelState);
		}

	}
}