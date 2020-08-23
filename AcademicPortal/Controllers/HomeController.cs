using AcademicPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AcademicPortal.Controllers
{
	public class HomeController : Controller
	{
		private ApplicationDbContext _context;
		public HomeController()
		{
			_context = new ApplicationDbContext();
		}
		public ActionResult Index()
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login", "Account");

			var userId = User.Identity.GetUserId();

			if (User.IsInRole("trainee"))
			{
				var courses = _context.Courses
					.Include("TraineeInfoes")
					.Where(p => p.TraineeInfoes.Any(u => u.UserId == userId))
					.ToList();

				return View("TraineeHome", courses);
			}

			if (User.IsInRole("trainer"))
			{
				var topics = _context.Topics
					.Include("TrainerInfoes")
					.Where(p => p.TrainerInfoes.Any(u => u.UserId == userId))
					.ToList();

				return View("TrainerHome", topics);
			}

			return View();
		}
	}
}