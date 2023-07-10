using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileWatchInformer.Utils
{
	internal static class RegexUtils
	{
		public static ValidateOptionsResult ValidateRegexMask(string mask, string name)
		{
			try
			{
				_ = new Regex(mask, RegexOptions.None, TimeSpan.FromSeconds(30));
			}
			catch (ArgumentException)
			{
				return ValidateOptionsResult.Fail($"Некорректный формат маски `{name}`");
			}
			catch (RegexMatchTimeoutException)
			{
				return ValidateOptionsResult.Fail($"Слишком сложное выражение маски `{name}`");
			}

			return ValidateOptionsResult.Success;
		}
	}
}
