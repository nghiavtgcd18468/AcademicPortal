using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AcademicPortal.Controllers
{
	public class ErrorController : Controller
	{
		// GET: Error
		public ActionResult Index()
		{
			return View();
		}

		// GET: Error/AccessDenied
		public ActionResult AccessDenied()
		{
			return View();
		}
	}
}