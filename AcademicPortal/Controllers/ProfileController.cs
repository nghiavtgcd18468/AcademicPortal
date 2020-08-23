using AcademicPortal.Attributes;
using AcademicPortal.Extensions;
using AcademicPortal.Models;
using AcademicPortal.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AcademicPortal.Controllers
{
	[AccessAuthorize]
	public class ProfileController : Controller
	{
		private ApplicationDbContext _context;
		private ApplicationUserManager _userManager;

		public ProfileController()
		{
			_context = new ApplicationDbContext();
		}

		public ProfileController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		// GET: Profile
		public ActionResult Index()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());

			if (user == null)
				return HttpNotFound();

			var roleId = user.Roles.SingleOrDefault().RoleId;
			var viewModel = GetViewModel(user, roleId);

			return View(viewModel);
		}

		// GET: /Profile/Details
		public ActionResult Details(string id)
		{
			var user = UserManager.FindById(id);

			if (user == null)
				return HttpNotFound();

			var roleId = user.Roles.SingleOrDefault().RoleId;

			var viewModel = GetViewModel(user, roleId);

			return View("Index", viewModel);
		}

		// GET: /Profile/UploadAva
		public ActionResult UploadAva(PostAvaInfo data)
		{
			if (data.File == null)
			{
				this.AddNotification("Please choose an image first", NotificationType.ERROR);
				return RedirectToAction("Index");
			}

			byte[] fileInBytes = new byte[data.File.ContentLength];
			using (BinaryReader reader = new BinaryReader(data.File.InputStream))
			{
				fileInBytes = reader.ReadBytes(data.File.ContentLength);
			}

			var UserInfoInDb = _context.UserInfoes.SingleOrDefault(p => p.UserId == data.userId);
			UserInfoInDb.Avatar = Convert.ToBase64String(fileInBytes);
			_context.SaveChanges();

			this.AddNotification("Avatar updated!", NotificationType.SUCCESS);

			if (User.Identity.GetUserId() != data.userId)
				return RedirectToAction("Details/" + data.userId);

			return RedirectToAction("Index");
		}

		// GET: Profile/Edit
		public ActionResult Edit()
		{
			var id = User.Identity.GetUserId();
			var user = UserManager.FindById(id);

			if (user == null)
				return HttpNotFound();

			var roleId = user.Roles.SingleOrDefault().RoleId;
			var viewModel = GetViewModel(user, roleId);

			viewModel.UserId = id;

			return View(viewModel);
		}

		// POST: Profile/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(ProfileViewModel model)
		{
			var userInfoInDb = _context.UserInfoes.SingleOrDefault(p => p.UserId == model.UserId);
			var userInDb = UserManager.FindById(model.UserId);

			if (userInfoInDb == null || userInDb == null)
				return HttpNotFound();

			userInfoInDb.FullName = model.FullName;
			userInDb.Email = model.User.Email;

			SetUserInfo(userInDb, userInfoInDb, model);
			_context.SaveChanges();
			UserManager.Update(userInDb);

			this.AddNotification("Profile updated!", NotificationType.SUCCESS);

			return RedirectToAction("Index");
		}

		private ProfileViewModel GetViewModel(ApplicationUser user, string roleId)
		{
			var userInfo = _context.UserInfoes.SingleOrDefault(p => p.UserId == user.Id);
			string ava = "";

			if (userInfo.Avatar == null)
				ava = GetDefaultImage();
			else
				ava = userInfo.Avatar;

			var viewModel = new ProfileViewModel
			{
				UserId = user.Id,
				User = user,
				RoleName = ((UserRole)Convert.ToInt32(roleId)).ToString(),
				FullName = userInfo.FullName,
				Avatar = ava
			};

			switch ((UserRole)Convert.ToInt32(roleId))
			{
				case UserRole.Admin:
					viewModel.AdminInfo = (AdminInfo)userInfo;
					break;
				case UserRole.Staff:
					viewModel.StaffInfo = (StaffInfo)userInfo;
					break;
				case UserRole.Trainer:
					viewModel.TrainerInfo = (TrainerInfo)userInfo;
					break;
				case UserRole.Trainee:
					viewModel.TraineeInfo = (TraineeInfo)userInfo;
					viewModel.TraineeInfo.Department = _context.Departments.SingleOrDefault(p => p.Id == viewModel.TraineeInfo.DepartmentId);
					break;
			}
			return viewModel;
		}

		private string GetDefaultImage()
		{
			string url = "https://989me.vn/themes/989me/images/guestbook/no_avatar.png";
			byte[] imgBytes = null;

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

			HttpWebResponse response = (HttpWebResponse)req.GetResponse();
			var stream = response.GetResponseStream();

			using (BinaryReader reader = new BinaryReader(stream))
			{
				int len = (int)(response.ContentLength);
				imgBytes = reader.ReadBytes(len);
			}

			return Convert.ToBase64String(imgBytes);
		}

		private void SetUserInfo(ApplicationUser user, UserInfo userInfo, ProfileViewModel model)
		{
			var roleId = user.Roles.SingleOrDefault().RoleId;
			switch ((UserRole)Convert.ToInt32(roleId))
			{
				case UserRole.Admin:
					var adminInfo = (AdminInfo)userInfo;
					adminInfo.Address = model.AdminInfo.Address;
					adminInfo.PhoneNumber = model.AdminInfo.PhoneNumber;
					break;
				case UserRole.Staff:
					var staffInfo = (StaffInfo)userInfo;
					staffInfo.Address = model.StaffInfo.Address;
					staffInfo.PhoneNumber = model.StaffInfo.PhoneNumber;
					break;
				case UserRole.Trainer:
					var trainerInfo = (TrainerInfo)userInfo;
					trainerInfo.Address = model.TrainerInfo.Address;
					trainerInfo.PhoneNumber = model.TrainerInfo.PhoneNumber;
					trainerInfo.WorkingPlace = model.TrainerInfo.WorkingPlace;
					break;
				case UserRole.Trainee:
					var traineeInfo = (TraineeInfo)userInfo;
					traineeInfo.Address = model.TraineeInfo.Address;
					traineeInfo.PhoneNumber = model.TraineeInfo.PhoneNumber;
					break;
				default:
					break;
			}
		}

		public class PostAvaInfo
		{
			public HttpPostedFileBase File { get; set; }
			public string userId { get; set; }
		}
	}
}