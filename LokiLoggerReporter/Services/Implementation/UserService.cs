using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LokiLogger.WebExtension.ViewModel;
using lokiloggerreporter.Models;
using lokiloggerreporter.ViewModel;
using lokiloggerreporter.ViewModel.User;
using Microsoft.AspNetCore.Identity;

namespace lokiloggerreporter.Services.Implementation
{
    public class UserService
    {
        public const string AdminRole = "Admin";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
		
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<OperationResult<bool>> Login(LoginModel model)
        {
            
            if(model == null) throw new ArgumentNullException();
            User user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return OpRes.Fail<bool>("User", "User not found");
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (result.Succeeded) return OpRes.Success(true);
            if(result.IsLockedOut) return OperationResult.Fail<bool>("Login","User is disabled");
            return OpRes.Fail<bool>("Login", "Login Failed");
        }

        public OperationResult<List<UserModel>> GetAllUser()
        {
            var result = _userManager.Users.AsEnumerable().Select(x =>
                new UserModel()
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    IsAdmin = _userManager.IsInRoleAsync(x,"Admin").Result,
                }).ToList();
            return OpRes.Success(result);
        }
        
        public async Task<OperationResult<UserModel>> CreateUser(UserCreateModel model)
        {
            if(model == null) throw new ArgumentNullException();
            User tmp = new User()
            {
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(tmp, model.Password);
            if (result.Succeeded)
            {
                if(model.IsAdmin)
                    result = await _userManager.AddToRoleAsync(tmp, "Admin");
                return OpRes.Success(new UserModel()
                {
                    IsAdmin = await _userManager.IsInRoleAsync(tmp,"Admin"),
                    UserId = tmp.Id,
                    UserName = tmp.UserName
                });
            }

            return OpRes.Fail<UserModel>("Error", result.Errors.FirstOrDefault()?.Description);
        }

        public async Task<OperationResult<UserModel>> UpdateUser(string userId, UserUpdateModel model)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if(user == null) return OpRes.Fail<UserModel>("User","User not found");

            if(!string.IsNullOrEmpty(model.Password)){
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    if (model.IsAdmin)
                    {
                        result = await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    return OpRes.Success(new UserModel()
                    {
                        IsAdmin = await _userManager.IsInRoleAsync(user,"Admin"),
                        UserId = user.Id,
                        UserName = user.UserName
                    });
                }

                return OpRes.Fail<UserModel>("Password", result.Errors.FirstOrDefault()?.Description);
            }

            return OpRes.Success<UserModel>(null);
        }

        public async Task<OperationResult<bool>> AbleUser(bool disable,string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if(user == null) return OpRes.Fail<bool>("User","User not found");
            if (disable)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            else
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
            
            return OperationResult.Success(true);  
        }
    }
}