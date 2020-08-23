namespace AcademicPortal.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class SeedUserInfoForAdmin : DbMigration
	{
		public override void Up()
		{
			Sql(@"
						INSERT INTO [dbo].[UserInfoes] ([UserId], [FullName], [Address], [DateOfBirth], [PhoneNumber], [TOEICScore], [Discriminator], [DepartmentId]) VALUES (N'79366a11-b12c-4c36-aae7-a4e3b3862c41', N'Vo Trong Nghia', N'1255  Hurry Street', N'2000-06-29 00:00:00', N'614-934-2354', NULL, N'AdminInfo', NULL)
					");
		}

		public override void Down()
		{
		}
	}
}
