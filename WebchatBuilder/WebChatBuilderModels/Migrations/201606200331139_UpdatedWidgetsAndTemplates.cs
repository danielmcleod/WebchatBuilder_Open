namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedWidgetsAndTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Templates", "UseUnstyledHeaderIcons", c => c.Boolean(nullable: false));
            AddColumn("dbo.Templates", "CloseButtonIcon", c => c.String());
            AddColumn("dbo.Widgets", "ResumeIconPath", c => c.String());
            AddColumn("dbo.Widgets", "ResumeLaunchText", c => c.String());
            AddColumn("dbo.Widgets", "ResumeTooltipText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Widgets", "ResumeTooltipText");
            DropColumn("dbo.Widgets", "ResumeLaunchText");
            DropColumn("dbo.Widgets", "ResumeIconPath");
            DropColumn("dbo.Templates", "CloseButtonIcon");
            DropColumn("dbo.Templates", "UseUnstyledHeaderIcons");
        }
    }
}
