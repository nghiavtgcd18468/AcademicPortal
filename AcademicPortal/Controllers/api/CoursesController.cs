using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace AcademicPortal.Controllers.api
{
	public class CoursesController : ApiController
	{
		private ApplicationDbContext _context;
		public CoursesController()
		{
			_context = new ApplicationDbContext();
		}

		//GET: api/courses
		public IHttpActionResult GetCourses()
		{
			var courses = _context.Courses;

			if (courses == null)
				return NotFound();

			return Ok(courses);
		}
	}
}
