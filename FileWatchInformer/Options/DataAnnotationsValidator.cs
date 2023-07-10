using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//https://github.com/LionelVallet/ReHackt.Extensions.Options.Validation/blob/7.0.1/src/DataAnnotationsValidator.cs

namespace FileWatchInformer.Options
{
	internal class DataAnnotationsValidator
	{
		public bool TryValidateObject(object obj, ICollection<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
		{
			return Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems), results, true);
		}

		public bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
		{
			return TryValidateObjectRecursive(obj, results, new HashSet<object>(), validationContextItems);
		}

		private bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems = null)
		{
			//short-circuit to avoid infinit loops on cyclical object graphs
			if (validatedObjects.Contains(obj))
			{
				return true;
			}

			validatedObjects.Add(obj);
			bool result = TryValidateObject(obj, results, validationContextItems);

			var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead
				&& prop.GetIndexParameters().Length == 0).ToList();

			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

				var value = obj.GetType().GetProperty(property.Name)?.GetValue(obj, null) ?? string.Empty;

				if (value == null) continue;

				if (value is IEnumerable asEnumerable)
				{
					foreach (var enumObj in asEnumerable)
					{
						if (enumObj != null)
						{
							var nestedResults = new List<ValidationResult>();
							if (!TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems))
							{
								result = false;
								foreach (var validationResult in nestedResults)
								{
									PropertyInfo property1 = property;
									results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
								}
							};
						}
					}
				}
				else
				{
					var nestedResults = new List<ValidationResult>();
					if (!TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems))
					{
						result = false;
						foreach (var validationResult in nestedResults)
						{
							PropertyInfo property1 = property;
							results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
						}
					};
				}
			}

			return result;
		}
	}
}
