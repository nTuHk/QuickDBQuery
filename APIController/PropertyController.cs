using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//proptechplus
using DBRepository;
using Helper;

namespace APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : Controller
    {

        private IRepository _dbRepository;

        public PropertyController(IRepository dbRepository)
        {
            this._dbRepository = dbRepository;
        }

        [HttpGet]
        public ActionResult<string> GetAll()
        {
            var jsonQuery = @"
				 DU_AN [property]:
				{
					@COUNT[>10] @ORDER[DESC]
					SO_LUON_DU_AN [ID],
					TIN_THANH [city],	
					QUAN_HUYEN [DISTRICT],
				}
					";
            string jsonDataSet = this._dbRepository.Read(jsonQuery);
            return Ok(jsonDataSet);
        }

        [HttpGet("{page}/{amount}")]
        public ActionResult<string> GetByPage(int page, int amount)
        {
            var jsonQuery = @"
				 DU_AN [property]<" + page + "," + amount + @">:
				{
					ID,
					TEN_DU_AN [NAME],
					TRANG_THAI [STATUS],
					CHU_DAU_TU [INVESTOR],
					TINH_THANH [CITY],
					QUAN_HUYEN [DISTRICT],
				}
					";
            string jsonDataSet = this._dbRepository.Read(jsonQuery);
            return Ok(jsonDataSet);
        }

        [HttpGet("/search/{name}")]
        public ActionResult<string> GetByName(string name)
        {
            string searchPattern = StringHelper.ConvertToSearchPattern("INVESTOR", name);
            var jsonQuery = @"
				 DU_AN [property](" + searchPattern + @"):
				{
					ID,
					TEN_DU_AN [NAME],
					TRANG_THAI [STATUS],
					CHU_DAU_TU [INVESTOR],
					TINH_THANH [CITY],
					QUAN_HUYEN [DISTRICT],
				}
					";
            string jsonDataSet = this._dbRepository.Read(jsonQuery);
            return Ok(jsonDataSet);
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetById(int id)
        {
            var jsonQuery = @"
				PROPERTY(ID = " + id + @") <1>:
				{
					ID,
					TEN_DU_AN [NAME],
					TRANG_THAI [STATUS],
					CHU_DAU_TU [INVESTOR],
					TINH_THANH [CITY],
					QUAN_HUYEN [DISTRICT],
				}";
            string jsonDataSet = this._dbRepository.Read(jsonQuery);
            return Ok(jsonDataSet);
        }

        /// <summary>
        /// Creates a new employee
        /// </summary>
        /// <param name="value">Employee Object</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /employee
        ///     {
        ///        "id": 007,
        ///        "name": "James Bond",
        ///        "isPermanant": true
        ///     }
        ///
        /// </remarks>
        // POST api/employees
        [HttpPost]
        public void Post([FromBody] string value)
        {
			
        }

    }
}