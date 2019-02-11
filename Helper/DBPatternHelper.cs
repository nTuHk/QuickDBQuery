using Constant;

namespace Helper
{
	public class DBPatternHelper
	{
		public static string GetQueryPattern()
		{
			return StringHelper.Simpler(Pattern.JSON_QUERY_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);	
		}

		public static string GetQueryObjPattern()
		{
			return StringHelper.Simpler(Pattern.OBJ_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);
		}

		public static string GetQueryAttrPattern()
		{
			return StringHelper.Simpler(Pattern.ATTR_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);
		}

		public static string GetQueryAttrAnotationPattern()
		{
			return StringHelper.Simpler(Pattern.ATTR_ANOTATION_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);
		}

		public static string GetQueryJoinPattern()
		{
			return StringHelper.Simpler(Pattern.JOIN_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);
		}

		public static string GetQueryObjPackPagePattern()
		{
			return StringHelper.Simpler(Pattern.OBJ_PACK_PAGE_PATTERN, Pattern.PATTERN_SPECIAL_CHARS);
		}
	}
}
