namespace AcademicPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTraineeAndCourseRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TraineeInfoCourses",
                c => new
                    {
                        TraineeInfo_UserId = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TraineeInfo_UserId, t.Course_Id })
                .ForeignKey("dbo.UserInfoes", t => t.TraineeInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.TraineeInfo_UserId)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TraineeInfoCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TraineeInfoCourses", "TraineeInfo_UserId", "dbo.UserInfoes");
            DropIndex("dbo.TraineeInfoCourses", new[] { "Course_Id" });
            DropIndex("dbo.TraineeInfoCourses", new[] { "TraineeInfo_UserId" });
            DropTable("dbo.TraineeInfoCourses");
        }
    }
}
