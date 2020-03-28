namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomInfoAndAttributesToVisitorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VisitorMessages", "CustomInfo", c => c.String());
            AddColumn("dbo.VisitorMessages", "AttributeNames", c => c.String());
            AddColumn("dbo.VisitorMessages", "AttributeValues", c => c.String());
            AddColumn("dbo.VisitorMessages", "RequestedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VisitorMessages", "RequestedTime");
            DropColumn("dbo.VisitorMessages", "AttributeValues");
            DropColumn("dbo.VisitorMessages", "AttributeNames");
            DropColumn("dbo.VisitorMessages", "CustomInfo");
        }
    }
}
