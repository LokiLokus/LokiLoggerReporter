using System;

namespace lokiloggerreporter.ViewModel.User {
	public class LoginResponseModel {
		public string Token { get; set; }
		public DateTime ExpireDate { get; set; }
	}
}