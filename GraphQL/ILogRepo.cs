using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lokiloggerreporter.GraphQL {
	public interface ILogRepo {
		Task<List<Log>> GetLogs();
	}

	public class LogType : ObjectGraphType<Log> {
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
			Field<IntGraphType>("logLevel", resolve: context => (int) context.Source.LogLevel);
			Field<IntGraphType>("logTyp", resolve: context => (int) context.Source.LogTyp);
			Field(a => a.ThreadId);
		}
	}

	public class LogQuery : ObjectGraphType {
		public LogQuery(DatabaseCtx database,ISettingsService settingsService)
		{
			Field<ListGraphType<LogType>>(
				"logs",
				arguments: new QueryArguments(
					new QueryArgument<BooleanGraphType> {Name = "debug"},
					new QueryArgument<BooleanGraphType> {Name = "information"},
					new QueryArgument<BooleanGraphType> {Name = "warning"},
					new QueryArgument<BooleanGraphType> {Name = "error"},
					new QueryArgument<BooleanGraphType> {Name = "critical"},
					new QueryArgument<BooleanGraphType> {Name = "normal"},
					new QueryArgument<BooleanGraphType> {Name = "exception"},
					new QueryArgument<BooleanGraphType> {Name = "return"},
					new QueryArgument<BooleanGraphType> {Name = "invoke"},
					new QueryArgument<BooleanGraphType> {Name = "restCall"},
					new QueryArgument<StringGraphType> {Name = "sourceId"},
					new QueryArgument<StringGraphType> {Name = "included"},
					new QueryArgument<StringGraphType> {Name = "excluded"}),
				resolve: context =>
				{
					bool debug = context.GetArgument<bool?>("debug") ?? false;
					bool info = context.GetArgument<bool?>("information") ?? false;
					bool warn = context.GetArgument<bool?>("warning") ?? false;
					bool error = context.GetArgument<bool?>("error") ?? false;
					bool critical = context.GetArgument<bool?>("critical") ?? false;


					bool normal = context.GetArgument<bool?>("normal") ?? false;
					bool exception = context.GetArgument<bool?>("exception") ?? false;
					bool ret = context.GetArgument<bool?>("return") ?? false;
					bool invoke = context.GetArgument<bool?>("invoke") ?? false;
					bool restCall = context.GetArgument<bool?>("restCall") ?? false;

					string included = context.GetArgument<string>("included");
					string excluded = context.GetArgument<string>("excluded");


					string sourceId = context.GetArgument<string>("sourceId");

					
					return database.Logs.Where(x => x.SourceId == sourceId &&
					                        (debug && x.LogLevel == LogLevel.Debug ||
					                         debug && x.LogLevel == LogLevel.Verbose ||
					                         info && x.LogLevel == LogLevel.Information ||
					                         warn && x.LogLevel == LogLevel.Warning ||
					                         error && x.LogLevel == LogLevel.Critical ||
					                         critical && x.LogLevel == LogLevel.SystemCritical)
					                        &&
					                        (normal && x.LogTyp == LogTyp.Normal ||
					                         exception && x.LogTyp == LogTyp.Exception ||
					                         ret && x.LogTyp == LogTyp.Return ||
					                         invoke && x.LogTyp == LogTyp.Invoke ||
					                         restCall && x.LogTyp == LogTyp.RestCall)
					                         &&
					                         x.Time >= DateTime.UtcNow.AddDays(-10)
					).Take(settingsService.Get<int>("MaxResultCount")).OrderBy(x => x.Time).Where(x =>
						(string.IsNullOrWhiteSpace(included) ||
						 JsonConvert.SerializeObject(x).Contains(included)) &&
						(string.IsNullOrWhiteSpace(excluded) ||
						 !JsonConvert.SerializeObject(x).Contains(included)));
				});
		}
	}
}