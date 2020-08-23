using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AcademicPortal.Controllers.api
{
	public class TopicsController : ApiController
	{
		private ApplicationDbContext _context;
		public TopicsController()
		{
			_context = new ApplicationDbContext();
		}

		//GET: api/topics
		public IHttpActionResult GetTopics()
		{
			var topics = _context.Topics;

			if (topics == null)
				return NotFound();

			return Ok(topics);
		}
	}
}
