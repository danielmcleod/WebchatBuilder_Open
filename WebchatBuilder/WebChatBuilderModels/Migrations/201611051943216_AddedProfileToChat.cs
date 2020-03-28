namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProfileToChat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chats", "Profile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chats", "Profile");
        }
    }
}
