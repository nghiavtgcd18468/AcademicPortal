namespace AcademicPortal.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class SeedUsersToDb : DbMigration
	{
		public override void Up()
		{
			Sql(@"
				INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'79366a11-b12c-4c36-aae7-a4e3b3862c41', N'admin@demo.com', 0, N'AP3O25WBTI68THFauhpWvVBKt/29pOkp5jzoYcgKx0MKAI/NeayKFKu/eWXyYcTIvA==', N'408efdd4-a417-4e8f-a5a3-cf0f13e9e74b', NULL, 0, 0, NULL, 1, 0, N'admin')
				INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'1', N'admin')
				INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'2', N'staff')
				INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'3', N'trainer')
				INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'4', N'trainee')
				INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'79366a11-b12c-4c36-aae7-a4e3b3862c41', N'1')
			");
		}

		public override void Down()
		{
		}
	}
}
