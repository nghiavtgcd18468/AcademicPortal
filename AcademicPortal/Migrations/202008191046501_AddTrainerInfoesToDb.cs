namespace AcademicPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrainerInfoesToDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "WorkingPlace", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "WorkingPlace");
        }
    }
}
