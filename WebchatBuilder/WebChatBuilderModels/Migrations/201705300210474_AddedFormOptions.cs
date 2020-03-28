namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFormOptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FormOptions",
                c => new
                    {
                        FormOptionId = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false),
                        Text = c.String(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        Profile_ProfileId = c.Int(),
                        FormField_FormFieldId = c.Int(),
                    })
                .PrimaryKey(t => t.FormOptionId)
                .ForeignKey("dbo.Profiles", t => t.Profile_ProfileId)
                .ForeignKey("dbo.FormFields", t => t.FormField_FormFieldId)
                .Index(t => t.Profile_ProfileId)
                .Index(t => t.FormField_FormFieldId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormOptions", "FormField_FormFieldId", "dbo.FormFields");
            DropForeignKey("dbo.FormOptions", "Profile_ProfileId", "dbo.Profiles");
            DropIndex("dbo.FormOptions", new[] { "FormField_FormFieldId" });
            DropIndex("dbo.FormOptions", new[] { "Profile_ProfileId" });
            DropTable("dbo.FormOptions");
        }
    }
}
