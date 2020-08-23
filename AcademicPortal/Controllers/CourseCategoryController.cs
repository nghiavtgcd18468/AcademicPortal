using AcademicPortal.Attributes;
using AcademicPortal.Extensions;
using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using System.Web.Mvc;

namespace AcademicPortal.Controllers
{
	public class CourseCategoryController : Controller
	{
		private ApplicationDbContext _context;

		public CourseCategoryController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: CourseCategory
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Index()
		{
			var courseCategories = _context.CourseCategories.ToList();

			return View(courseCategories);
		}

		// GET: CourseCategory/Create
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Create()
		{
			var courseCategory = new CourseCategory();
			return View(courseCategory);
		}

		// POST: CourseCategory/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Create(CourseCategory model)
		{
			_context.CourseCategories.Add(model);
			_context.SaveChanges();

			this.AddNotification("Added a new course category!", NotificationType.SUCCESS);
			return RedirectToAction("Index");
		}

		// GET: CourseCategory/Edit
		[HttpGet]
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			var courseCategory = _context.CourseCategories.SingleOrDefault(p => p.Id == id);

			if (courseCategory == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			return View(courseCategory);
		}

		// POST: CourseCategory/Edit
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(CourseCategory model)
		{
			var courseCategoryInDb = _context.CourseCategories.SingleOrDefault(p => p.Id == model.Id);

			courseCategoryInDb.Name = model.Name;
			courseCategoryInDb.Description = model.Description;
			_context.SaveChanges();

			this.AddNotification("Course category updated!", NotificationType.SUCCESS);

			return RedirectToAction("Details/" + model.Id);
		}

		// DELETE: CourseCategory/delete
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
		{
			var courseCategoryToRemove = _context.CourseCategories.SingleOrDefault(p => p.Id == id);

			if (courseCategoryToRemove == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			_context.CourseCategories.Remove(courseCategoryToRemove);
			_context.SaveChanges();

			this.AddNotification("Course category deleted", NotificationType.SUCCESS);

			return RedirectToAction("Index");
		}

		// GET: CourseCategory/Details
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Details(int id)
		{
			var courseCategory = _context.CourseCategories.Include("Courses").SingleOrDefault(p => p.Id == id);

			if (courseCategory == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			return View(courseCategory);
		}

		//POST: CourseCategory/AddCourse
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult AddCourse(PostData data)
		{
			var courseCategoryInDb = _context.CourseCategories.SingleOrDefault(p => p.Id == data.courseCategoryId);
			var courseInDb = _context.Courses.Include("CourseCategory").SingleOrDefault(p => p.Id == data.courseId);

			if (courseCategoryInDb == null || courseInDb == null)
				return HttpNotFound();

			if (courseInDb.CourseCategory == null)
			{
				courseInDb.CourseCategory = courseCategoryInDb;
				_context.SaveChanges();

				this.AddNotification("Course added!", NotificationType.SUCCESS);
			}
			else
				this.AddNotification(String.Format("{0} is already added to {1}", courseInDb.Name, courseInDb.CourseCategory.Name), NotificationType.INFO);

			return RedirectToAction("Details/" + data.courseCategoryId);
		}

		// POST: CourseCategory/RemoveCourse
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveCourse(PostData data)
		{
			var courseCategoryInDb = _context.CourseCategories.SingleOrDefault(p => p.Id == data.courseCategoryId);
			var courseInDb = _context.Courses.Include("CourseCategory").SingleOrDefault(p => p.Id == data.courseId);

			if (courseCategoryInDb == null || courseInDb == null)
				return HttpNotFound();

			if (courseInDb.CourseCategory == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			else
			{
				courseInDb.CourseCategory = null;
				_context.SaveChanges();

				this.AddNotification("Course removed!", NotificationType.SUCCESS);
			}

			return RedirectToAction("Details/" + data.courseCategoryId);
		}

		public class PostData
		{
			public int courseId { get; set; }
			public int courseCategoryId { get; set; }
		}
	}
}