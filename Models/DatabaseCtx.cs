using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.Models {
	public class DatabaseCtx :DbContext{

		public DatabaseCtx(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Source> Sources { get; set; }
		public DbSet<WebRequest> WebRequest { get; set; }
		public DbSet<Log> Logs { get; set; }
	}
}