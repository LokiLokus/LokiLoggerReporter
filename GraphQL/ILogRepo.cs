using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using lokiloggerreporter.Models;
using Microsoft.EntityFrameworkCore;

namespace lokiloggerreporter.GraphQL {
	public interface ILogRepo {
		Task<List<Log>> GetLogs();  
	}
	
	public class LogType : ObjectGraphType<Log>  
	{  
		public LogType()  
		{  
			Field(a => a.Class);  
			Field(a => a.Data);  
			Field(a => a.Exception);  
			Field(a => a.Line);  
			Field(a => a.Message);  
			Field(a => a.Method);  
			Field(a => a.Time);  
			Field(a => a.SourceId);  
			Field(a => a.ID);
			Field<IntGraphType>("logLevel", resolve: context => (int)context.Source.LogLevel);
			Field<IntGraphType>("logTyp", resolve: context => (int)context.Source.LogTyp);
			Field(a => a.ThreadId);
		}  
	}  
	public class LogQuery : ObjectGraphType  
	{  
		
		public LogQuery(DatabaseCtx database)  
		{  
			Field<ListGraphType<LogType>>(  
				"logs",  
				arguments: new QueryArguments(
					new QueryArgument<IntGraphType> { Name = "logLevel" },
					new QueryArgument<IntGraphType> { Name = "logTyp" }),
				resolve: context =>
				{
					LogLevel? logLevel = context.HasArgument("logLevel") ? context.GetArgument<LogLevel>("logLevel"):(LogLevel?)null;
					LogTyp? logTyp = context.HasArgument("logTyp") ? context.GetArgument<LogTyp>("logTyp"):(LogTyp?)null;

					var start = database.Logs;
					
					return start.Where(x => 
						(logLevel != null && x.LogLevel == logLevel) &&	
						(logTyp != null && x.LogTyp == logTyp)
						);
				});
		}  
	}  
}