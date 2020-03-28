namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedFormFieldAndVisitorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormFields", "IsPhoneNumber", c => c.Boolean(nullable: false));
            AddColumn("dbo.VisitorMessages", "Workgroup", c => c.String());
            AddColumn("dbo.VisitorMessages", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.VisitorMessages", "Skills", c => c.String());
            AddColumn("dbo.VisitorMessages", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VisitorMessages", "PhoneNumber");
            DropColumn("dbo.VisitorMessages", "Skills");
            DropColumn("dbo.VisitorMessages", "Type");
            DropColumn("dbo.VisitorMessages", "Workgroup");
            DropColumn("dbo.FormFields", "IsPhoneNumber");
        }
    }
}
