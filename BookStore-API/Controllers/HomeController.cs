using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
	/// <summary>
	/// General Book Store API Controller
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
			_logger = logger;

		}	

		/// <summary>
		/// Get full details
		/// </summary>
		/// <returns></returns>
		// GET: api/<HomeController>
		[HttpGet]
		public IEnumerable<string> Get()
		{
			_logger.LogInfo(" This is an info msg from get method accessed from Home controller");
			return new string[] { "value1", "value2" };
		}

		/// <summary>
		/// Gets only details with parmeter
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		// GET api/<HomeController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			_logger.LogWarn(" This is an warning msg from get method with parm accessed from Home controller");

			return "value";
		}
		/// <summary>
		/// Methos for saving data using API
		/// </summary>
		/// <param name="value"></param>
		// POST api/<HomeController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
			_logger.LogError(" This is an Error msg from post method  from Home controller");
		}

		// PUT api/<HomeController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}
		/// <summary>
		/// Delete methos using API with parameter
		/// </summary>
		/// <param name="id"></param>
		// DELETE api/<HomeController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
			_logger.LogDebug(" This is a Debug msg from delete method with parm accessed from Home controller");
		}
	}
}
