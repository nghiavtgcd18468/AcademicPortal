using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class CourseCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<Course> Courses { get; set; }
	}
}