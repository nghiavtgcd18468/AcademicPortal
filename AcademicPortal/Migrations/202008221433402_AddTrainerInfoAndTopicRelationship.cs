namespace AcademicPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrainerInfoAndTopicRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainerInfoTopics",
                c => new
                    {
                        TrainerInfo_UserId = c.String(nullable: false, maxLength: 128),
                        Topic_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainerInfo_UserId, t.Topic_Id })
                .ForeignKey("dbo.UserInfoes", t => t.TrainerInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Topics", t => t.Topic_Id, cascadeDelete: true)
                .Index(t => t.TrainerInfo_UserId)
                .Index(t => t.Topic_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainerInfoTopics", "Topic_Id", "dbo.Topics");
            DropForeignKey("dbo.TrainerInfoTopics", "TrainerInfo_UserId", "dbo.UserInfoes");
            DropIndex("dbo.TrainerInfoTopics", new[] { "Topic_Id" });
            DropIndex("dbo.TrainerInfoTopics", new[] { "TrainerInfo_UserId" });
            DropTable("dbo.TrainerInfoTopics");
        }
    }
}
