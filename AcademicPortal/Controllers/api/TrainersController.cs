using AcademicPortal.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AcademicPortal.Controllers.api
{
	public class TrainersController : ApiController
	{
		private ApplicationDbContext _context;
		private ApplicationUserManager _userManager;
		public TrainersController()
		{
			_context = new ApplicationDbContext();
		}

		public TrainersController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//GET: api/trainers
		public IHttpActionResult GetTrainers()
		{
			var trainerId = Convert.ToInt32(UserRole.Trainer).ToString();

			var users = UserManager.Users
					.Where(u => u.Roles.Any(r => r.RoleId == trainerId))
					.ToList();

			if (users == null)
				return NotFound();

			var usernames = new List<SendUser>();

			foreach (var user in users)
			{
				var newUser = new SendUser
				{
					Id = user.Id,
					UserName = user.UserName
				};

				usernames.Add(newUser);
			}

			return Ok(usernames);
		}

		public class SendUser
		{
			public string Id { get; set; }
			public string UserName { get; set; }
		}
	}
}
