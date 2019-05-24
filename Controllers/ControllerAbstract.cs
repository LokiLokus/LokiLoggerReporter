using lokiloggerreporter.Database;
using lokiloggerreporter.Database.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lokiloggerreporter.Controllers {
	[ApiController]
	public abstract class ControllerAbstract : Microsoft.AspNetCore.Mvc.Controller{
		protected readonly DatabaseCtx _dbContext;
		protected readonly SignInManager<User> _signInManager;
		protected readonly UserManager<User> _userManager;
		
		public ControllerAbstract(UserManager<User> userManager, SignInManager<User> signInManager, DatabaseCtx context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_dbContext = context;
		}
	}

}