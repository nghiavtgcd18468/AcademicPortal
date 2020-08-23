using AcademicPortal.Attributes;
using AcademicPortal.Extensions;
using AcademicPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AcademicPortal.Controllers
{
	public class CourseController : Controller
	{
		private ApplicationDbContext _context;

		public CourseController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: Course
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Index()
		{
			var courses = _context.Courses.ToList();

			return View(courses);
		}

		// GET: Course/Create
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Create()
		{
			var course = new Course();
			return View(course);
		}

		// POST: Course/Create
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Course model)
		{
			_context.Courses.Add(model);
			_context.SaveChanges();

			this.AddNotification("Added a new course!", NotificationType.SUCCESS);
			return RedirectToAction("Index");
		}

		// GET: Course/Edit
		[HttpGet]
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			var course = _context.Courses.SingleOrDefault(p => p.Id == id);

			if (course == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			return View(course);
		}

		// POST: Course/Edit
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Course model)
		{
			var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == model.Id);

			courseInDb.Name = model.Name;
			courseInDb.Description = model.Description;
			_context.SaveChanges();

			this.AddNotification("Course updated!", NotificationType.SUCCESS);

			return RedirectToAction("Details/" + model.Id);
		}

		// DELETE: Course/delete
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
		{
			var courseToRemove = _context.Courses.SingleOrDefault(p => p.Id == id);

			if (courseToRemove == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			_context.Courses.Remove(courseToRemove);
			_context.SaveChanges();

			this.AddNotification("Course deleted", NotificationType.SUCCESS);

			return RedirectToAction("Index");
		}

		// GET: Course/Details
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Details(int id)
		{
			var course = _context.Courses
				.Include("Topics")
				.Include("TraineeInfoes")
				.SingleOrDefault(p => p.Id == id);

			if (course == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			foreach (var trainee in course.TraineeInfoes)
			{
				var user = _context.Users.SingleOrDefault(p => p.Id == trainee.UserId);
				trainee.User = user;
			}

			return View(course);
		}

		//POST: Course/AddTopic
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult AddTopic(TopicPostData data)
		{
			var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == data.courseId);
			var topicInDb = _context.Topics.Include("Course").SingleOrDefault(p => p.Id == data.topicId);

			if (courseInDb == null || topicInDb == null)
				return HttpNotFound();

			if (topicInDb.Course == null)
			{
				topicInDb.Course = courseInDb;
				_context.SaveChanges();

				this.AddNotification("Topic added!", NotificationType.SUCCESS);
			}
			else
				this.AddNotification(String.Format("{0} is already added to {1}", topicInDb.Name, topicInDb.Course.Name), NotificationType.INFO);

			return RedirectToAction("Details/" + data.courseId);
		}

		// POST: Course/RemoveTopic
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveTopic(TopicPostData data)
		{
			var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == data.courseId);
			var topicInDb = _context.Topics.Include("Course").SingleOrDefault(p => p.Id == data.topicId);

			if (courseInDb == null || topicInDb == null)
				return HttpNotFound();

			if (topicInDb.Course == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			else
			{
				topicInDb.Course = null;
				_context.SaveChanges();

				this.AddNotification("Topic removed!", NotificationType.SUCCESS);
			}

			return RedirectToAction("Details/" + data.courseId);
		}

		//POST: Course/AddTrainee
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult AddTrainee(TraineePostData data)
		{
			var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == data.courseId);
			var traineeInfoInDb = _context.UserInfoes
				.OfType<TraineeInfo>()
				.Include("Courses")
				.Include("User")
				.SingleOrDefault(p => p.UserId == data.userId);

			if (courseInDb == null || traineeInfoInDb == null)
				return HttpNotFound();

			if (traineeInfoInDb.Courses.SingleOrDefault(p => p.Id == courseInDb.Id) != null)
				this.AddNotification(String.Format("{0} is already added to {1}", traineeInfoInDb.User.UserName, courseInDb.Name), NotificationType.INFO);
			else
			{
				traineeInfoInDb.Courses.Add(courseInDb);
				_context.SaveChanges();

				this.AddNotification("Trainee added!", NotificationType.SUCCESS);
			}

			return RedirectToAction("Details/" + data.courseId);
		}

		//POST: Course/RemoveTrainee
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveTrainee(TraineePostData data)
		{
			var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == data.courseId);
			var traineeInfoInDb = _context.UserInfoes
				.OfType<TraineeInfo>()
				.Include("Courses")
				.Include("User")
				.SingleOrDefault(p => p.UserId == data.userId);

			if (courseInDb == null || traineeInfoInDb == null)
				return HttpNotFound();

			if (traineeInfoInDb.Courses.SingleOrDefault(p => p.Id == courseInDb.Id) != null)
			{
				traineeInfoInDb.Courses.Remove(courseInDb);
				_context.SaveChanges();

				this.AddNotification("Trainee removed!", NotificationType.SUCCESS);
			}
			else
				this.AddNotification("Trainee not found", NotificationType.ERROR);

			return RedirectToAction("Details/" + data.courseId);
		}

		public class TopicPostData
		{
			public int topicId { get; set; }
			public int courseId { get; set; }
		}

		public class TraineePostData
		{
			public string userId { get; set; }
			public int courseId { get; set; }
		}
	}
}