using AcademicPortal.Attributes;
using AcademicPortal.Extensions;
using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AcademicPortal.Controllers
{
	public class TopicController : Controller
	{
		private ApplicationDbContext _context;

		public TopicController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: Topic
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Index()
		{
			var topics = _context.Topics.ToList();

			return View(topics);
		}

		// GET: Topic/Create
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Create()
		{
			var topic = new Topic();
			return View(topic);
		}

		// POST: Topic/Create
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Topic model)
		{
			_context.Topics.Add(model);
			_context.SaveChanges();

			this.AddNotification("Added a new topic!", NotificationType.SUCCESS);
			return RedirectToAction("Index");
		}

		// GET: Topic/Edit
		[HttpGet]
		[AccessAuthorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			var topic = _context.Topics.SingleOrDefault(p => p.Id == id);

			if (topic == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			return View(topic);
		}

		// POST: Topic/Edit
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Topic model)
		{
			var topicInDb = _context.Topics.SingleOrDefault(p => p.Id == model.Id);

			topicInDb.Name = model.Name;
			topicInDb.Description = model.Description;
			_context.SaveChanges();

			this.AddNotification("Topic updated!", NotificationType.SUCCESS);

			return RedirectToAction("Details/" + model.Id);
		}

		// DELETE: Topic/delete
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
		{
			var topicToRemove = _context.Topics.SingleOrDefault(p => p.Id == id);

			if (topicToRemove == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			_context.Topics.Remove(topicToRemove);
			_context.SaveChanges();

			this.AddNotification("Topic deleted", NotificationType.SUCCESS);

			return RedirectToAction("Index");
		}

		// GET: Topic/Details
		[AccessAuthorize(Roles = "staff,trainer,trainee")]
		public ActionResult Details(int id)
		{
			var topic = _context.Topics
				.Include("TrainerInfoes")
				.SingleOrDefault(p => p.Id == id);

			if (topic == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			foreach (var trainer in topic.TrainerInfoes)
			{
				var user = _context.Users.SingleOrDefault(p => p.Id == trainer.UserId);
				trainer.User = user;
			}

			return View(topic);
		}

		// POST: Topic/AddTrainer
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult AddTrainer(TrainerPostData data)
		{
			var topicInDb = _context.Topics.SingleOrDefault(p => p.Id == data.topicId);
			var trainerInDb = _context.UserInfoes
				.OfType<TrainerInfo>()
				.Include("Topics")
				.Include("User")
				.SingleOrDefault(p => p.UserId == data.userId);

			if (topicInDb == null || trainerInDb == null)
				return HttpNotFound();

			if (trainerInDb.Topics.SingleOrDefault(p => p.Id == topicInDb.Id) != null)
				this.AddNotification(String.Format("{0} is already added to {1}", trainerInDb.User.UserName, topicInDb.Name), NotificationType.INFO);
			else
			{
				trainerInDb.Topics.Add(topicInDb);
				_context.SaveChanges();

				this.AddNotification("Trainee added!", NotificationType.SUCCESS);
			}

			return RedirectToAction("Details/" + data.topicId);
		}

		// POST: Topic/RemoveTrainer
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveTrainer(TrainerPostData data)
		{
			var topicInDb = _context.Topics.SingleOrDefault(p => p.Id == data.topicId);
			var trainerInDb = _context.UserInfoes
				.OfType<TrainerInfo>()
				.Include("Topics")
				.SingleOrDefault(p => p.UserId == data.userId);

			if (topicInDb == null || trainerInDb == null)
				return HttpNotFound();

			if (trainerInDb.Topics.SingleOrDefault(p => p.Id == topicInDb.Id) != null)
			{
				trainerInDb.Topics.Remove(topicInDb);
				_context.SaveChanges();

				this.AddNotification("Trainer removed!", NotificationType.SUCCESS);
			}
			else
				this.AddNotification("Trainer not found", NotificationType.ERROR);

			return RedirectToAction("Details/" + data.topicId);
		}

		public class TrainerPostData
		{
			public string userId { get; set; }
			public int topicId { get; set; }
		}

	}
}