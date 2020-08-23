using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class UserInfo
	{
		[Key]
		[ForeignKey("User")]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		[Required]
		[DisplayName("Full name")]
		public string FullName { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		[DisplayName("Date of birth")]
		public DateTime DateOfBirth { get; set; }
		[Required]
		[DisplayName("Phone number")]
		public string PhoneNumber { get; set; }

		[DisplayName("Avatar")]
		public string Avatar { get; set; }
	}
}