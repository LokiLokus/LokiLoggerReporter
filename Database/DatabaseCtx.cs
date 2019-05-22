using lokiloggerreporter.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.Database {
	public class DatabaseCtx :DbContext{
		
		public DatabaseCtx(DbContextOptions<DatabaseCtx> options) : base(options)
		{
            
		}
		public DbSet<Log> Logs { get; set; }

	}
}