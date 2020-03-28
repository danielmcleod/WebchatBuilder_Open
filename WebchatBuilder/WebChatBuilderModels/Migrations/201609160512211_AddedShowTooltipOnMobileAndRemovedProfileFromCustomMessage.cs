namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShowTooltipOnMobileAndRemovedProfileFromCustomMessage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomMessages", "Profile_ProfileId", "dbo.Profiles");
            DropIndex("dbo.CustomMessages", new[] { "Profile_ProfileId" });
            AddColumn("dbo.Widgets", "ShowTooltipOnMobile", c => c.Boolean(nullable: false));
            DropColumn("dbo.CustomMessages", "Profile_ProfileId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomMessages", "Profile_ProfileId", c => c.Int());
            DropColumn("dbo.Widgets", "ShowTooltipOnMobile");
            CreateIndex("dbo.CustomMessages", "Profile_ProfileId");
            AddForeignKey("dbo.CustomMessages", "Profile_ProfileId", "dbo.Profiles", "ProfileId");
        }
    }
}
