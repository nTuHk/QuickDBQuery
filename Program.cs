using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//proptechplus
using DBRepository;
using Helper;

namespace ProjectTemplate
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
//			string key = "4S Riverside Garden";
//			Console.WriteLine(StringHelper.ConvertToSearchPattern("NAME",key));
//			string name = "riverside";
//			var jsonQuery = @"
//				 DU_AN [property](ucase(name) like '%" + name + @"%' AND (ab =cd)):
//				{
//					ID,
//					TEN_DU_AN [NAME],
//					TRANG_THAI [STATUS],
//					CHU_DAU_TU [INVESTOR],
//					TINH_THANH [CITY],
//					QUAN_HUYEN [DISTRICT],
//				}
//					";
//			IRepository dbRepository = new DB2Repository("Server=tunguyen-XPS-13-9360:50000;database=SAMPLE;uid=db2inst1;pwd=db2inst1;");
//			string jsonDataSet =	dbRepository.Read(jsonQuery);
//			Console.WriteLine(jsonDataSet);
			//Console.WriteLine("SQL query: {0}", new DB2SqlParser().ReadSqlParser(jsonQuery));

		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
			.UseStartup<Startup>();
	}
}
