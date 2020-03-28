namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsRequiredToFormField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormFields", "IsRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormFields", "IsRequired");
        }
    }
}
