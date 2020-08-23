namespace AcademicPortal.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class SeedDepartmentsToDb : DbMigration
	{
		public override void Up()
		{
			Sql(@"
						SET IDENTITY_INSERT [dbo].[Departments] ON
						INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (1, N'Information Technology')
						INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (2, N'Business')
						INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (3, N'Law')
						SET IDENTITY_INSERT [dbo].[Departments] OFF
					");
		}

		public override void Down()
		{
		}
	}
}
