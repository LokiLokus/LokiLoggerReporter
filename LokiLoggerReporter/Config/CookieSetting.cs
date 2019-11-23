namespace lokiloggerreporter.Config
{
    public class CookieSetting
    {
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string AccessDeniedPath { get; set; }
        public bool SlidingExpiration { get; set; }
        public int ExpireTimeSpanInMin { get; set; }

        public static CookieSetting Default()
        {
            return new CookieSetting()
            {
                LoginPath = "/api/User/NotAuth",
                LogoutPath = "/api/User/NotAuth",
                SlidingExpiration = true,
                AccessDeniedPath = "/api/User/NotAuth",
                ExpireTimeSpanInMin = 1000
            };
        }
    }
}