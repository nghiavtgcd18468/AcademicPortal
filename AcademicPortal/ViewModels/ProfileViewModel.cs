using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AcademicPortal.ViewModels
{
	public class ProfileViewModel
	{
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public string RoleName { get; set; }
		public string FullName { get; set; }
		public string Avatar { get; set; }
		public AdminInfo AdminInfo { get; set; }
		public TraineeInfo TraineeInfo { get; set; }
		public StaffInfo StaffInfo { get; set; }
		public TrainerInfo TrainerInfo { get; set; }
	}
}