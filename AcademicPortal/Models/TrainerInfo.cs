using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcademicPortal.Models
{
	public class TrainerInfo : UserInfo
	{
		public TrainerInfo()
		{
			this.Topics = new HashSet<Topic>();
		}
		public string WorkingPlace { get; set; }
		public ICollection<Topic> Topics { get; set; }
	}
}