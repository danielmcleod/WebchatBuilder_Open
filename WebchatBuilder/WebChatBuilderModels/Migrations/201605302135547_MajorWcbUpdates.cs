namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MajorWcbUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConfigId = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        Description = c.String(),
                        OverrideMessage = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Keywords = c.String(),
                        ClosedOnly = c.Boolean(nullable: false),
                        DateLastModified = c.DateTime(nullable: false),
                        IsAssignable = c.Boolean(nullable: false),
                        MarkedForDeletion = c.Boolean(nullable: false),
                        ScheduleRecurrence_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScheduleRecurrences", t => t.ScheduleRecurrence_Id)
                .Index(t => t.ScheduleRecurrence_Id);
            
            CreateTable(
                "dbo.ScheduleRecurrences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConfigId = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        Dates = c.String(),
                        Days = c.String(),
                        StartDate = c.DateTime(),
                        StartTime = c.DateTime(),
                        EndDate = c.DateTime(),
                        EndTime = c.DateTime(),
                        IsAllDay = c.Boolean(nullable: false),
                        IsDaySpan = c.Boolean(nullable: false),
                        IsRelative = c.Boolean(nullable: false),
                        Month = c.Int(),
                        PatternType = c.Int(nullable: false),
                        RelativeDayType = c.Int(nullable: false),
                        RelativeMonthlyType = c.Int(nullable: false),
                        WeeklyStartTime = c.DateTime(),
                        WeeklyEndTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VisitorMessages",
                c => new
                    {
                        VisitorMessageId = c.Long(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        IsProcessed = c.Boolean(nullable: false),
                        Message = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.VisitorMessageId);
            
            CreateTable(
                "dbo.ProfileSchedules",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfileId, t.ScheduleId })
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.ScheduleId);
            
            AddColumn("dbo.Widgets", "ShowUnavailableIfOpenNoAgents", c => c.Boolean(nullable: false));
            AddColumn("dbo.Widgets", "UnavailableIconPath", c => c.String());
            AddColumn("dbo.Widgets", "UnavailableLaunchText", c => c.String());
            AddColumn("dbo.Widgets", "UnavailableTooltipText", c => c.String());
            AddColumn("dbo.Widgets", "UnavailableForm_FormId", c => c.Int());
            AddColumn("dbo.Forms", "ShowFormMessage", c => c.Boolean(nullable: false));
            AddColumn("dbo.Forms", "UseScheduleMessage", c => c.Boolean(nullable: false));
            AddColumn("dbo.Forms", "FormMessage", c => c.String());
            AddColumn("dbo.Forms", "FormSubmittedMessage", c => c.String());
            CreateIndex("dbo.Widgets", "UnavailableForm_FormId");
            AddForeignKey("dbo.Widgets", "UnavailableForm_FormId", "dbo.Forms", "FormId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Widgets", "UnavailableForm_FormId", "dbo.Forms");
            DropForeignKey("dbo.ProfileSchedules", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.ProfileSchedules", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Schedules", "ScheduleRecurrence_Id", "dbo.ScheduleRecurrences");
            DropIndex("dbo.ProfileSchedules", new[] { "ScheduleId" });
            DropIndex("dbo.ProfileSchedules", new[] { "ProfileId" });
            DropIndex("dbo.Widgets", new[] { "UnavailableForm_FormId" });
            DropIndex("dbo.Schedules", new[] { "ScheduleRecurrence_Id" });
            DropColumn("dbo.Forms", "FormSubmittedMessage");
            DropColumn("dbo.Forms", "FormMessage");
            DropColumn("dbo.Forms", "UseScheduleMessage");
            DropColumn("dbo.Forms", "ShowFormMessage");
            DropColumn("dbo.Widgets", "UnavailableForm_FormId");
            DropColumn("dbo.Widgets", "UnavailableTooltipText");
            DropColumn("dbo.Widgets", "UnavailableLaunchText");
            DropColumn("dbo.Widgets", "UnavailableIconPath");
            DropColumn("dbo.Widgets", "ShowUnavailableIfOpenNoAgents");
            DropTable("dbo.ProfileSchedules");
            DropTable("dbo.VisitorMessages");
            DropTable("dbo.ScheduleRecurrences");
            DropTable("dbo.Schedules");
        }
    }
}
