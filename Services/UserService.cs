using System.Collections.Generic;
using lokiloggerreporter.ViewModel;

namespace lokiloggerreporter.Services {
	public interface IUserService {
		OperationResult Login(string username, string password);
		List<UserModel> GetAllUser();
		OperationResult CreateUser(string username, string password, bool isAdmin);
		OperationResult UpdateUser(string userId, string password, bool isAdmin);
	}
	public class UserService :IUserService{
		public OperationResult Login(string username, string password)
		{
			throw new System.NotImplementedException();
		}

		public List<UserModel> GetAllUser()
		{
			throw new System.NotImplementedException();
		}

		public OperationResult CreateUser(string username, string password, bool isAdmin)
		{
			throw new System.NotImplementedException();
		}

		public OperationResult UpdateUser(string userId, string password, bool isAdmin)
		{
			throw new System.NotImplementedException();
		}
	}

}