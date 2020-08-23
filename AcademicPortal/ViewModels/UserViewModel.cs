using AcademicPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AcademicPortal.ViewModels
{
	public class UserViewModel
	{
		public ApplicationUser User { get; set; }
		public string RoleName { get; set; }
		public string FullName { get; set; }
	}
}