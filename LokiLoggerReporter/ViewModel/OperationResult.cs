using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace lokiloggerreporter.ViewModel
{/*
    public class OperationResult : OperationResult<object> {
	}

	public class OperationResult<T> {
		public bool Succeeded { get; set; }
		public object SuccessResult { get; set; }

		public Dictionary<string, List<string>> Result
		{
			get
			{
				Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
				if (Errors != null)
				{
					IEnumerable<string> codes = Errors.Select(x => x.Code).Distinct();
					foreach (string code in codes)
						result[code] = Errors.Where(x => x.Code == code).Select(x => x.Description).ToList();
				}

				return result;
			}
		}

		public IEnumerable<OperationOutput> Errors { get; set; }

		public void Failed(params OperationOutput[] output)
		{
			if (output == null) output = new OperationOutput[0];
			if (Errors == null) Errors = new List<OperationOutput>();
			foreach (OperationOutput tmp in output) Errors.Append(tmp);
		}

		public static OperationResult Fail(params OperationOutput[] output)
		{
			IEnumerable<OperationOutput> errors = new List<OperationOutput>();
			if (output != null) errors = output.ToList();
			return new OperationResult
			{
				Succeeded = false,
				Errors = errors
			};
		}

		public static OperationResult Failed(string code, string description)
		{
			return new OperationResult
			{
				Succeeded = false,
				Errors = new List<OperationOutput>
				{
					new OperationOutput(code, description)
				}
			};
		}

		public static OperationResult IdentityResult(IdentityResult identityResult)
		{
			if (identityResult == null) throw new ArgumentNullException(nameof(identityResult));
			if (identityResult.Succeeded) return Success();
			return Fail(identityResult.Errors);
		}

		public static OperationResult Fail()
		{
			return new OperationResult
			{
				Succeeded = false,
				Errors = new List<OperationOutput>()
			};
		}

		public static OperationResult Success(object result = null)
		{
			return new OperationResult
			{
				Succeeded = true,
				SuccessResult = result
			};
		}

		public static OperationResult Fail(IEnumerable<IdentityError> resultErrors)
		{
			List<OperationOutput> errors = new List<OperationOutput>();
			foreach (IdentityError identityError in resultErrors)
				errors.Add(new OperationOutput(identityError.Code, identityError.Description));
			return new OperationResult
			{
				Succeeded = false,
				Errors = errors
			};
		}
	}

	public class OperationOutput {
		public OperationOutput(string code, string desc)
		{
			Code = code;
			Description = desc;
		}

		public OperationOutput()
		{
		}

		public string Code { get; set; }
		public string Description { get; set; }

		public static OperationOutput Operation(string code, string desc)
		{
			return new OperationOutput(code, desc);
		}
	}*/

}