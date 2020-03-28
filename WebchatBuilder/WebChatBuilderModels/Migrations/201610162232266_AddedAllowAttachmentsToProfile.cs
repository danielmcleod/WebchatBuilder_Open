namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAllowAttachmentsToProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "AllowAttachments", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "AllowAttachments");
        }
    }
}
