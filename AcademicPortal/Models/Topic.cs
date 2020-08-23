using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class Topic
	{
		public Topic()
		{
			this.TrainerInfoes = new HashSet<TrainerInfo>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Course Course { get; set; }
		[Display(Name = "Trainers")]
		public ICollection<TrainerInfo> TrainerInfoes { get; set; }
	}
}