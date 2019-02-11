using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//proptechplus
using DBRepository;

namespace APIController
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompanyController : Controller
	{

		private IRepository _dbRepository;

		public CompanyController(IRepository dbRepository)
		{
			this._dbRepository = dbRepository;
		}

		[HttpGet]
		public ActionResult<string> Get()
		{
			var jsonQuery = @"
				CONG_TY [COMPANY]:
				{
					ID,
					TEN [NAME],
					TEN_RUT_GON [SHORT_NAME],
					TEN_NUOC_NGOAI [FOREIGN_NAME],
					NGAY_THANH_LAP [ESTABLISH_DATE],
					NGUOI_DAI_DIEN_PHAP_LUAT [LAW_PRESIDENT],
					DIA_CHI [ADDRESS],
				}
					";
			string jsonDataSet = this._dbRepository.Read(jsonQuery);
			return Ok(jsonDataSet);
		}

		[HttpGet("{id}")]
		public ActionResult<string> GetById(int id)
		{
			var jsonQuery = @"
				COMPANY(ID = " + id + @") <1>:
				{
					ID,
					NAME,
					SHORT_NAME,
					FOREIGN_NAME,
					ESTABLISH_DATE,
					LAW_PRESIDENT,
					ADDRESS,
				}";
			string jsonDataSet = this._dbRepository.Read(jsonQuery);
			return Ok(jsonDataSet);
		}

	}
}


