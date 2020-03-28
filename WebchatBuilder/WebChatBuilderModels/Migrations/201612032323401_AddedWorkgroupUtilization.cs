namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedWorkgroupUtilization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Utilizations", "Workgroup_WorkgroupId", c => c.Int());
            CreateIndex("dbo.Utilizations", "Workgroup_WorkgroupId");
            AddForeignKey("dbo.Utilizations", "Workgroup_WorkgroupId", "dbo.Workgroups", "WorkgroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Utilizations", "Workgroup_WorkgroupId", "dbo.Workgroups");
            DropIndex("dbo.Utilizations", new[] { "Workgroup_WorkgroupId" });
            DropColumn("dbo.Utilizations", "Workgroup_WorkgroupId");
        }
    }
}
