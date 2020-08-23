namespace AcademicPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfoesToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        DepartmentId = c.Int(),
                        TOEICScore = c.Short(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.UserInfoes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserInfoes", new[] { "DepartmentId" });
            DropIndex("dbo.UserInfoes", new[] { "UserId" });
            DropTable("dbo.UserInfoes");
        }
    }
}
