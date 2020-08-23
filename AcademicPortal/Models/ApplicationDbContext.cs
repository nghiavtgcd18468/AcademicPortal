using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AcademicPortal.Models
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
		: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
		public DbSet<UserInfo> UserInfoes { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<CourseCategory> CourseCategories { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Topic> Topics { get; set; }
	}
}