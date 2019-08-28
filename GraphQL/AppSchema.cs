using GraphQL;
using GraphQL.Types;

namespace lokiloggerreporter.GraphQL {
	public class AppSchema :Schema {
		public AppSchema(IDependencyResolver resolver) : base(resolver)  
		{  
			Query = resolver.Resolve<LogQuery>();  
		}  
	}
}