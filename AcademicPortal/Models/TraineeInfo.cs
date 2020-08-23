using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class TraineeInfo : UserInfo
	{
		public TraineeInfo()
		{
			this.Courses = new HashSet<Course>();
		}

		[Display(Name = "Department")]
		public int DepartmentId { get; set; }
		public Department Department { get; set; }
		[Range(0, 990)]
		[DisplayName("TOEIC Score")]
		public short TOEICScore { get; set; }
		[Required]
		public ICollection<Course> Courses { get; set; }
	}
}