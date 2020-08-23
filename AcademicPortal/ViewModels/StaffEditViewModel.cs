using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcademicPortal.ViewModels
{
	public class StaffEditViewModel
	{
		[Required]
		public string UserId { get; set; }
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
		[Required]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
		public int RoleId { get; set; }
		public string RoleName { get; set; }
		public TrainerInfo TrainerInfo { get; set; }
		public TraineeInfo TraineeInfo { get; set; }
	}
}