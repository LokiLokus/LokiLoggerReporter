using System.Collections.Generic;
using System.Linq;

namespace lokiloggerreporter.ViewModel {
	public class OperationResult {
		public bool Succeeded { get; set; }
		public object SuccessResult { get; set; }
		public IEnumerable<OperationOutput> Errors { get; set; }

		public void Failed(params OperationOutput[] output)
		{
			if(output == null) output = new OperationOutput[0];
			if(Errors == null) Errors = new List<OperationOutput>();
			foreach (OperationOutput tmp in output)
			{
				Errors.Append(tmp);
			}
		}
		
	}

	public class OperationOutput {
		public string Code { get; set; }
		public string Description { get; set; }
	}

}