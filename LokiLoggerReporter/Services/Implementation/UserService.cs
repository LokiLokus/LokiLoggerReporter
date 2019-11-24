using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LokiLogger.WebExtension.ViewModel;
using lokiloggerreporter.Models;
using lokiloggerreporter.ViewModel;
using lokiloggerreporter.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace lokiloggerreporter.Services.Implementation
{
    public class UserService
    {
        public const string AdminRole = "Admin";
        public const string ValidAudience = "http://localhost:5000";
        public const string ValidIssuer = "http://localhost:5000";
        public const string SecureKey = "sdfsdffdssasdaasdasd";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
		
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<OperationResult<LoginResponseModel>> Login(LoginModel model)
        {
            
            if(model == null) throw new ArgumentNullException();
            User user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return OpRes.Fail<LoginResponseModel>("User", "User not found");
            bool result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (result)
            {
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(UserService.SecureKey));

                var token = new JwtSecurityToken(
                    issuer: UserService.ValidIssuer,
                    audience: UserService.ValidAudience,
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return OperationResult.Success( new LoginResponseModel()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireDate = token.ValidTo
                });
            }
            return OpRes.Fail<LoginResponseModel>("Login", "Login Failed");
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