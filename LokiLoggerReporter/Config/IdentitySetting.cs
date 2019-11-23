namespace lokiloggerreporter.Config
{
   public class IdentitySetting
    {
        public UserSettings User { get; set; }
        public PasswordSettings Password { get; set; }
        public LockoutSettings Lockout { get; set; }
        public SignInSettings SignIn { get; set; }

        public static IdentitySetting Default()
        {
            var res = new IdentitySetting()
            {
                User = new UserSettings()
                {
                    RequireUniqueEmail = false
                },
                Password = new PasswordSettings()
                {
                    RequireDigit = false,
                    RequiredLength = 2,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false
                },
                Lockout = new LockoutSettings()
                {
                    AllowedForNewUsers = true,
                    MaxFailedAccessAttempts = 1000,
                    DefaultLockoutTimeSpanInMins = 1000
                },
                SignIn = new SignInSettings()
                {
                    RequireConfirmedEmail = false,
                    RequireConfirmedPhoneNumber = false
                }
            };
            return res;
        }
    }

    public class UserSettings
    {
        public bool RequireUniqueEmail { get; set; }
    }

    public class PasswordSettings
    {
        public int RequiredLength { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
    }

    public class LockoutSettings
    {
        public bool AllowedForNewUsers { get; set; }
        public int DefaultLockoutTimeSpanInMins { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }

    public class SignInSettings {
        public bool RequireConfirmedEmail { get; set; }
        public bool RequireConfirmedPhoneNumber { get; set; }
    }
}