using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.Models {
	public class DatabaseCtx :DbContext{
		
		public DatabaseCtx(DbContextOptions options):base(options){}

		public DbSet<Log> Logs { get; set; }
	}
}