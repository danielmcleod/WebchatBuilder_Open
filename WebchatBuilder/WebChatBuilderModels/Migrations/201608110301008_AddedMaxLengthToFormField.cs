namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMaxLengthToFormField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormFields", "MaxLength", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormFields", "MaxLength");
        }
    }
}
