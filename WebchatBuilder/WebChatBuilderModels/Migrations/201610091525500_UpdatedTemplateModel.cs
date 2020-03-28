namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTemplateModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Templates", "DisconnectButtonIcon", c => c.String());
            AddColumn("dbo.Templates", "PrintButtonIcon", c => c.String());
            AddColumn("dbo.Templates", "EmailButtonIcon", c => c.String());
            AddColumn("dbo.Templates", "IncludeDisconnect", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Templates", "IncludeDisconnect");
            DropColumn("dbo.Templates", "EmailButtonIcon");
            DropColumn("dbo.Templates", "PrintButtonIcon");
            DropColumn("dbo.Templates", "DisconnectButtonIcon");
        }
    }
}
