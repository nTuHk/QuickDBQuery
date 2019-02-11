namespace Constant
{
	public class Pattern
	{
		public static readonly string PATTERN_SPECIAL_CHARS = "\\[\\]{}\\(\\):,@\"";

		public static readonly string JSON_QUERY_PATTERN = @"
			^(?<OBJECT>[^:]*):
			{
				((?<ATTRIBUTES>[^,{}:]*),)*
					((?<JOIN_OBJECTS>@JOIN[^:]*:{[^{}]*}),)*
			}";

		public static readonly string OBJ_PATTERN = @"
			^(?<OBJ_NAME>[^\[\]()<>]*)
			(\[(?<OBJ_TABLE_NAME>[^\[\]]*)\])?
			(\((?<OBJ_CONDITION>.*)\))?
			(<(?<OBJ_PACK>[^<>]*)>)?
			";

		public static readonly string ATTR_PATTERN = @"
			^(?<ANOTATIONS>@
					(?<ANOTATION_NAME>[^@\[\]\s]*)
					(\[(?<ANOTATION_META>[^\[\]]*)\])?
			 )*
			(?<ATTR>\s*
			 (?<ATTR_NAME>[^\[\]()]*)
			 (\[(?<COL_NAME>[^\[\]]*)\])?
			 (\((?<ATTR_FORMAT>[^()]*)\))?
			)
			";

		public static readonly string ATTR_ANOTATION_PATTERN = @"
			^@(?<ANOTATION_NAME>[^@\[\]\s]*)
			(\[(?<ANOTATION_META>[^\[\]]*)\])?	
			";

		public static readonly string JOIN_PATTERN = @"
			^(?<JOIN_META>
					@JOIN(
						\[(?<JOIN_TYPE>[^()]*)
						\((?<JOIN_CONDITION>[^()]*)\)\]
					     )?)
					(?<JOIN_TABLE>((?<JOIN_ATTR_NAME>[^\[\]()]*)
					(\[(?<JOIN_TABLE_NAME>[^\[\]]*)\])?
					(\((?<JOIN_TABLE_CONDITION>[^()]*)\))?
					(<(?<JOIN_PACK>[^<>]*)>)?):
			{
				((?<JOIN_VALUES>[^{},]*),)*
			})	
		";

		//Default PACK IS ALL
		public static readonly string OBJ_PACK_PAGE_PATTERN =@"^(?<PAGE_INDEX>\d*),(?<AMOUNT>\d*)";

	}
}
