namespace AcademicPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCoursesCourseCategoriesTopicsToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CourseCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseCategories", t => t.CourseCategory_Id)
                .Index(t => t.CourseCategory_Id);
            
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Course_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.Course_Id)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "CourseCategory_Id", "dbo.CourseCategories");
            DropForeignKey("dbo.Topics", "Course_Id", "dbo.Courses");
            DropIndex("dbo.Topics", new[] { "Course_Id" });
            DropIndex("dbo.Courses", new[] { "CourseCategory_Id" });
            DropTable("dbo.Topics");
            DropTable("dbo.Courses");
            DropTable("dbo.CourseCategories");
        }
    }
}
