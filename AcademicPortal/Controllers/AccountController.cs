using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AcademicPortal.Models;
using AcademicPortal.Attributes;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using AcademicPortal.Extensions;
using System.Net;
using AcademicPortal.ViewModels;
using Microsoft.Ajax.Utilities;

namespace AcademicPortal.Controllers
{
	public class AccountController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		private ApplicationDbContext _context;
		private RoleManager<IdentityRole> _roleManager;

		public AccountController()
		{
			_context = new ApplicationDbContext();
			_roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;

		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
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

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Console.WriteLine(model.UserName);

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AccessAuthorize(Roles = "admin,staff")]
		public ActionResult Register()
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login", "Account");


			if (User.IsInRole("admin"))
			{
				var viewModel = new RegisterViewModel
				{
					Roles = GetRoles(UserRole.Admin)
				};

				return View(viewModel);
			}
			else if (User.IsInRole("staff"))
			{
				var viewModel = new TraineeRegisterViewModel
				{
					Roles = GetRoles(UserRole.Staff),
					UserRoleId = Convert.ToInt32(UserRole.Trainee),
					Deparments = _context.Departments.ToList()
				};

				return View("TraineeRegister", viewModel);
			}

			return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AccessAuthorize(Roles = "admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (_context.Users.Any(p => p.UserName == model.UserName))
			{
				this.AddNotification("Sorry, this username is already available", NotificationType.ERROR);
				return RedirectToAction("Register");
			}

			var errors = ModelState
		.Where(x => x.Value.Errors.Count > 0)
		.Select(x => new { x.Key, x.Value.Errors })
		.ToArray();

			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					var roleName = _context.Roles.SingleOrDefault(p => p.Id == model.UserRoleId.ToString()).Name;

					await UserManager.AddToRoleAsync(user.Id, roleName);

					dynamic UserInfoToInsert = null;

					if (roleName == "staff")
					{
						UserInfoToInsert = new StaffInfo();

						UserInfoToInsert.UserId = user.Id;
						UserInfoToInsert.FullName = model.UserInfo.FullName;
						UserInfoToInsert.Address = model.UserInfo.Address;
						UserInfoToInsert.DateOfBirth = model.UserInfo.DateOfBirth;
						UserInfoToInsert.PhoneNumber = model.UserInfo.PhoneNumber;
						UserInfoToInsert.FullName = model.UserInfo.FullName;
					}
					else if (roleName == "trainer")
					{
						UserInfoToInsert = new TrainerInfo();

						UserInfoToInsert.UserId = user.Id;
						UserInfoToInsert.FullName = model.UserInfo.FullName;
						UserInfoToInsert.Address = model.UserInfo.Address;
						UserInfoToInsert.DateOfBirth = model.UserInfo.DateOfBirth;
						UserInfoToInsert.PhoneNumber = model.UserInfo.PhoneNumber;
						UserInfoToInsert.FullName = model.UserInfo.FullName;
					}

					if (UserInfoToInsert != null)
					{
						_context.UserInfoes.Add(UserInfoToInsert);
						_context.SaveChanges();
					}

					//await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					this.AddNotification("An account has been created!", NotificationType.SUCCESS);
					return RedirectToAction("Register", "Account");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return RedirectToAction("Register");
		}

		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RegisterForTrainee(TraineeRegisterViewModel model)
		{
			if (_context.Users.Any(p => p.UserName == model.UserName))
			{
				this.AddNotification("Sorry, this username is already available", NotificationType.ERROR);
				return RedirectToAction("Register");
			}

			var errors = ModelState
		.Where(x => x.Value.Errors.Count > 0)
		.Select(x => new { x.Key, x.Value.Errors })
		.ToArray();

			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await UserManager.AddToRoleAsync(user.Id, UserRole.Trainee.ToString());

					model.TraineeInfo.UserId = user.Id;
					model.TraineeInfo.DepartmentId = model.DepartmentId;
					model.TraineeInfo.Department = _context.Departments.SingleOrDefault(p => p.Id == model.DepartmentId);

					_context.UserInfoes.Add(model.TraineeInfo);
					_context.SaveChanges();

					this.AddNotification("An account has been created!", NotificationType.SUCCESS);
					return RedirectToAction("Register", "Account");
				}
				AddErrors(result);
			}

			return RedirectToAction("Register");
		}

		// GET: /Account
		[AccessAuthorize(Roles = "admin,staff")]
		public ActionResult Index()
		{
			var adminId = Convert.ToInt32(UserRole.Admin).ToString();
			var staffId = Convert.ToInt32(UserRole.Staff).ToString();
			var trainerId = Convert.ToInt32(UserRole.Trainer).ToString();
			var traineeId = Convert.ToInt32(UserRole.Trainee).ToString();

			var users = new List<ApplicationUser>();

			if (User.IsInRole("admin"))
			{
				users = UserManager.Users
					.Where(u => u.Roles.Any(r => r.RoleId == staffId || r.RoleId == trainerId))
					.ToList();
			}
			else if (User.IsInRole("staff"))
			{
				users = UserManager.Users
					.Where(u => u.Roles.Any(r => r.RoleId == trainerId || r.RoleId == traineeId))
					.ToList();
			}

			var viewModel = new List<UserViewModel>();

			users.ForEach(u =>
			{
				var user = new UserViewModel
				{
					User = u,
					RoleName = UserManager.GetRoles(u.Id).FirstOrDefault(),
					FullName = _context.UserInfoes.SingleOrDefault(p => p.UserId == u.Id).FullName
				};

				viewModel.Add(user);
			});

			return View(viewModel);
		}

		// GET: /Account/Edit
		[HttpGet]
		[AccessAuthorize(Roles = "admin,staff")]
		public ActionResult Edit(string id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var user = _context.Users.SingleOrDefault(p => p.Id == id);
			var roleId = Convert.ToInt32(user.Roles.SingleOrDefault().RoleId);

			if (user == null)
				return HttpNotFound();

			if (User.IsInRole("admin"))
			{
				var roles = _context.Roles.ToList();
				roles.Remove(roles.SingleOrDefault(p => p.Name == "admin"));
				roles.Remove(roles.SingleOrDefault(p => p.Name == "trainee"));

				var viewModel = new AdminEditViewModel
				{
					UserId = id,
					UserName = user.UserName,
					UserRoleId = Convert.ToInt32(user.Roles.FirstOrDefault().RoleId),
					Roles = roles
				};

				return View("AdminEdit", viewModel);
			}
			else if (User.IsInRole("staff"))
			{
				var roleName = (UserRole)roleId;
				var userInfo = _context.UserInfoes.SingleOrDefault(p => p.UserId == id);

				var viewModel = new StaffEditViewModel
				{
					UserId = id,
					UserName = user.UserName,
					Email = user.Email,
					RoleName = roleName.ToString(),
					RoleId = roleId
				};

				switch (roleName)
				{
					case UserRole.Trainee:
						viewModel.TraineeInfo = (TraineeInfo)userInfo;
						break;
					case UserRole.Trainer:
						viewModel.TrainerInfo = (TrainerInfo)userInfo;
						break;
					default:
						break;
				}

				return View("StaffEdit", viewModel);
			}
			else
				return RedirectToAction("AccessDenied", "Error");
		}

		// POST: /Account/AdminEdit
		[HttpPost]
		[AccessAuthorize(Roles = "admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AdminEdit(AdminEditViewModel model)
		{
			var user = await UserManager.FindByIdAsync(model.UserId);

			user.UserName = model.UserName;

			UserManager.Update(user);

			if (model.Password != null)
			{
				var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
			}

			var oldRoleId = user.Roles.SingleOrDefault().RoleId;

			if (oldRoleId != model.UserRoleId.ToString())
			{
				var oldRoleName = _context.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
				var newRoleName = _context.Roles.SingleOrDefault(r => r.Id == model.UserRoleId.ToString()).Name;

				await UserManager.RemoveFromRoleAsync(user.Id, oldRoleName);
				await UserManager.AddToRoleAsync(user.Id, newRoleName);
			}

			this.AddNotification("Account updated", NotificationType.SUCCESS);

			return RedirectToAction("Index");
		}

		// POST: Account/StaffEdit
		[HttpPost]
		[AccessAuthorize(Roles = "staff")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> StaffEdit(StaffEditViewModel model)
		{
			var user = await UserManager.FindByIdAsync(model.UserId);
			var userInfo = _context.UserInfoes.SingleOrDefault(p => p.UserId == model.UserId);

			user.UserName = model.UserName;
			user.Email = model.Email;

			UserManager.Update(user);

			if (model.Password != null)
			{
				var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
			}

			switch ((UserRole)model.RoleId)
			{
				case UserRole.Trainer:
					var trainerInfo = (TrainerInfo)userInfo;

					trainerInfo.FullName = model.TrainerInfo.FullName;
					trainerInfo.Address = model.TrainerInfo.Address;
					trainerInfo.PhoneNumber = model.TrainerInfo.PhoneNumber;
					trainerInfo.WorkingPlace = model.TrainerInfo.WorkingPlace;
					break;
				case UserRole.Trainee:
					var traineeInfo = (TraineeInfo)userInfo;

					traineeInfo.FullName = model.TraineeInfo.FullName;
					traineeInfo.Address = model.TraineeInfo.Address;
					traineeInfo.PhoneNumber = model.TraineeInfo.PhoneNumber;
					traineeInfo.TOEICScore = model.TraineeInfo.TOEICScore;
					break;
				default:
					break;
			}

			_context.SaveChanges();

			this.AddNotification("Account updated", NotificationType.SUCCESS);

			return RedirectToAction("Details/" + model.UserId, "Profile");
		}

		// POST: /Account/Delete
		[HttpPost]
		[AccessAuthorize(Roles = "admin,staff")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(string id)
		{
			if (ModelState.IsValid)
			{
				if (id == null)
				{
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
				}

				var user = await UserManager.FindByIdAsync(id);
				var logins = user.Logins;
				var rolesForUser = await UserManager.GetRolesAsync(id);

				var userInfo = _context.UserInfoes.SingleOrDefault(p => p.UserId == id);
				_context.UserInfoes.Remove(userInfo);
				_context.SaveChanges();

				using (var transaction = _context.Database.BeginTransaction())
				{
					foreach (var login in logins.ToList())
					{
						await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
					}

					if (rolesForUser.Count() > 0)
					{
						foreach (var item in rolesForUser.ToList())
						{
							var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
						}

						await UserManager.DeleteAsync(user);
						transaction.Commit();

						this.AddNotification("Account deleted", NotificationType.SUCCESS);
					}
				}
			}

			return RedirectToAction("Index");
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				// string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
				// await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				// return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			this.AddNotification("Logged off", NotificationType.SUCCESS);
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		private List<IdentityRole> GetRoles(UserRole currentUserRole)
		{
			var roles = _context.Roles.ToList();
			switch (currentUserRole)
			{
				case UserRole.Admin:
					roles.Remove(roles.SingleOrDefault(p => p.Name == "admin"));
					roles.Remove(roles.SingleOrDefault(p => p.Name == "trainee"));
					break;
				case UserRole.Staff:
					roles = roles.Where(r => r.Name == "trainee").ToList();
					break;
				case UserRole.Trainee:
				case UserRole.Trainer:
					roles = null;
					break;
				default:
					break;
			}

			return roles;
		}

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
					: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}

	public enum UserRole
	{
		Admin = 1,
		Staff,
		Trainer,
		Trainee
	}
}