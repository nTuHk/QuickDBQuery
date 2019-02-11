using System;
using System.Text.RegularExpressions;

namespace Helper
{
	public class StringHelper
	{
		private StringHelper()
		{
			//DONT need create instance
		}

		// clean MULTI SPACE
		public static string CleanSpace(string input)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			return Regex.Replace(input.Trim(), "\\s+", " ");
		}
		
		// clean MULTI SPACE (noSpace between specialChars)
		public static string CleanSpace(string input, string specialChars)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			return Regex.Replace(CleanSpace(input), "\\s*([" + specialChars + "])\\s*", "$1");
		}

		// clean Break Line
		public static string CleanBreakLine(string input)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			return Regex.Replace(input, "\\r\\n", string.Empty); 
		}

		// clean MULTI SPACE & BREAKLINE
		public static string Simpler(string input)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			return CleanSpace(CleanBreakLine(input));
		}

		// clean MULTI SPACE & BREAKLINE (noSpace between specialChars)
		public static string Simpler(string input, string specialChars)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			return CleanSpace(CleanBreakLine(input), specialChars);
		}

		public static string ConvertToSearchPattern(string fieldName, string keywords)
		{
			//simpler
			keywords = Simpler(keywords).ToLower();
			fieldName = Simpler(fieldName);
			fieldName = $"lower({fieldName})";

			//parse pattern
			string searchPattern = string.Empty;
			string[] keywordsSplit = keywords.Split(" ");
			foreach(string key in keywordsSplit)
			{
				if(!string.IsNullOrEmpty(searchPattern))
				{
					searchPattern += " AND ";
				}
				searchPattern += $"{fieldName} LIKE '%{key}%'";
			}

			return searchPattern;
		}

	}
}
