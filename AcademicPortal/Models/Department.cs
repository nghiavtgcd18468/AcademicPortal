using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class Department
	{
		public int Id { get; set; }
		[Display(Name = "Department")]
		public string Name { get; set; }
	}
}