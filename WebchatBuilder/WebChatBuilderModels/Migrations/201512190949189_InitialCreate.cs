namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        AgentId = c.Int(nullable: false, identity: true),
                        ConfigId = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        DisplayImage = c.String(),
                        HasActiveClientLicense = c.Boolean(nullable: false),
                        MediaLevel = c.Int(nullable: false),
                        IsLicensedForChat = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AgentId);
            
            CreateTable(
                "dbo.Workgroups",
                c => new
                    {
                        WorkgroupId = c.Int(nullable: false, identity: true),
                        ConfigId = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsAcd = c.Boolean(nullable: false),
                        HasQueue = c.Boolean(nullable: false),
                        IsAssignable = c.Boolean(nullable: false),
                        MarkedForDeletion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WorkgroupId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        HeaderText = c.String(),
                        HeaderLogoPath = c.String(),
                        IncludeUserDataAsCustomInfo = c.Boolean(nullable: false),
                        IncludeUserDataAsAttributes = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        LastUpdatedOn = c.DateTime(nullable: false),
                        CreatedBy_Id = c.String(maxLength: 128),
                        LastUpdatedBy_Id = c.String(maxLength: 128),
                        Template_TemplateId = c.Int(),
                        Widget_WidgetId = c.Int(),
                        Workgroup_WorkgroupId = c.Int(),
                    })
                .PrimaryKey(t => t.ProfileId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LastUpdatedBy_Id)
                .ForeignKey("dbo.Templates", t => t.Template_TemplateId)
                .ForeignKey("dbo.Widgets", t => t.Widget_WidgetId)
                .ForeignKey("dbo.Workgroups", t => t.Workgroup_WorkgroupId)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.LastUpdatedBy_Id)
                .Index(t => t.Template_TemplateId)
                .Index(t => t.Widget_WidgetId)
                .Index(t => t.Workgroup_WorkgroupId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DisplayImage = c.String(),
                        Title = c.String(),
                        IcUser = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        SkillId = c.Int(nullable: false, identity: true),
                        ConfigId = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        IsAssignable = c.Boolean(nullable: false),
                        MarkedForDeletion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SkillId);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        TemplateId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        CustomCss = c.String(),
                        IncludeHeader = c.Boolean(nullable: false),
                        HeaderText = c.String(),
                        HeaderLogoPath = c.String(),
                        HeaderIcons = c.Boolean(nullable: false),
                        IncludeTranscript = c.Boolean(nullable: false),
                        IncludePrint = c.Boolean(nullable: false),
                        PlaceholderText = c.String(),
                        SendButtonIcon = c.String(),
                        SendIncludeIcon = c.Boolean(nullable: false),
                        ShowInitials = c.Boolean(nullable: false),
                        MessageArrows = c.Boolean(nullable: false),
                        ShowTime = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TemplateId);
            
            CreateTable(
                "dbo.Widgets",
                c => new
                    {
                        WidgetId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RecycleTime = c.Int(nullable: false),
                        StartTime = c.Int(nullable: false),
                        CheckForAgents = c.Boolean(nullable: false),
                        RequiredAgentsAvailable = c.Int(nullable: false),
                        MaxEstimatedWaitTime = c.Int(nullable: false),
                        ShowOnMobile = c.Boolean(nullable: false),
                        MobileWidth = c.Int(nullable: false),
                        UseIframe = c.Boolean(nullable: false),
                        PopOverlay = c.Boolean(nullable: false),
                        UseIcon = c.Boolean(nullable: false),
                        IconPath = c.String(),
                        IconWidth = c.Int(nullable: false),
                        LaunchText = c.String(),
                        Position = c.String(),
                        Rounded = c.Boolean(nullable: false),
                        Vertical = c.Boolean(nullable: false),
                        Background = c.String(),
                        TextColor = c.String(),
                        PlaceHolderBackground = c.String(),
                        ShowLoader = c.Boolean(nullable: false),
                        Height = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        OffsetX = c.Int(nullable: false),
                        OffsetY = c.Int(nullable: false),
                        TooltipText = c.String(),
                        TooltipColor = c.String(),
                        ShowTooltip = c.Boolean(nullable: false),
                        ShowTooltipAtStart = c.Int(nullable: false),
                        LaunchInNewWindow = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Form_FormId = c.Int(),
                    })
                .PrimaryKey(t => t.WidgetId)
                .ForeignKey("dbo.Forms", t => t.Form_FormId)
                .Index(t => t.Form_FormId);
            
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        FormId = c.Int(nullable: false, identity: true),
                        FormName = c.String(nullable: false),
                        LabelColor = c.String(),
                        Rounded = c.Boolean(nullable: false),
                        BackgroundColor = c.String(),
                        BorderColor = c.String(),
                        ButtonColor = c.String(),
                        ButtonTextColor = c.String(),
                        ButtonText = c.String(),
                    })
                .PrimaryKey(t => t.FormId);
            
            CreateTable(
                "dbo.FormFields",
                c => new
                    {
                        FormFieldId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Label = c.String(),
                        PlaceHolder = c.String(),
                        FieldType = c.Int(nullable: false),
                        SendAsAttribute = c.Boolean(nullable: false),
                        AppendToCustomInfo = c.Boolean(nullable: false),
                        IsUserField = c.Boolean(nullable: false),
                        Position = c.Int(nullable: false),
                        CustomClasses = c.String(),
                        Form_FormId = c.Int(),
                    })
                .PrimaryKey(t => t.FormFieldId)
                .ForeignKey("dbo.Forms", t => t.Form_FormId)
                .Index(t => t.Form_FormId);
            
            CreateTable(
                "dbo.Utilizations",
                c => new
                    {
                        UtilizationId = c.Int(nullable: false, identity: true),
                        MediaType = c.Int(nullable: false),
                        MaxAssignable = c.Int(nullable: false),
                        UtilizationPercent = c.Int(nullable: false),
                        Agent_AgentId = c.Int(),
                    })
                .PrimaryKey(t => t.UtilizationId)
                .ForeignKey("dbo.Agents", t => t.Agent_AgentId)
                .Index(t => t.Agent_AgentId);
            
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        ChatId = c.Long(nullable: false, identity: true),
                        SessionId = c.String(nullable: false),
                        ConnectionId = c.String(),
                        ChatIdentifier = c.String(),
                        ParticipantId = c.String(),
                        InteractionId = c.String(),
                        QueueName = c.String(),
                        DateCreated = c.DateTime(),
                        DateAnswered = c.DateTime(),
                        DateEnded = c.DateTime(),
                        UserData_UserDataId = c.Long(),
                    })
                .PrimaryKey(t => t.ChatId)
                .ForeignKey("dbo.UserDatas", t => t.UserData_UserDataId)
                .Index(t => t.UserData_UserDataId);
            
            CreateTable(
                "dbo.UserDatas",
                c => new
                    {
                        UserDataId = c.Long(nullable: false, identity: true),
                        FromUrl = c.String(),
                        IpAddress = c.String(),
                        UserAgent = c.String(),
                        CustomData = c.String(),
                    })
                .PrimaryKey(t => t.UserDataId);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        LanguageId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.LanguageId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingId = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.SettingId);
            
            CreateTable(
                "dbo.ProfileSkills",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfileId, t.SkillId })
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.AgentWorkgroups",
                c => new
                    {
                        AgentId = c.Int(nullable: false),
                        WorkgroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AgentId, t.WorkgroupId })
                .ForeignKey("dbo.Agents", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("dbo.Workgroups", t => t.WorkgroupId, cascadeDelete: true)
                .Index(t => t.AgentId)
                .Index(t => t.WorkgroupId);
            
            CreateTable(
                "dbo.AgentSkills",
                c => new
                    {
                        AgentId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AgentId, t.SkillId })
                .ForeignKey("dbo.Agents", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.AgentId)
                .Index(t => t.SkillId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Chats", "UserData_UserDataId", "dbo.UserDatas");
            DropForeignKey("dbo.Utilizations", "Agent_AgentId", "dbo.Agents");
            DropForeignKey("dbo.AgentSkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.AgentSkills", "AgentId", "dbo.Agents");
            DropForeignKey("dbo.AgentWorkgroups", "WorkgroupId", "dbo.Workgroups");
            DropForeignKey("dbo.AgentWorkgroups", "AgentId", "dbo.Agents");
            DropForeignKey("dbo.Profiles", "Workgroup_WorkgroupId", "dbo.Workgroups");
            DropForeignKey("dbo.Profiles", "Widget_WidgetId", "dbo.Widgets");
            DropForeignKey("dbo.Widgets", "Form_FormId", "dbo.Forms");
            DropForeignKey("dbo.FormFields", "Form_FormId", "dbo.Forms");
            DropForeignKey("dbo.Profiles", "Template_TemplateId", "dbo.Templates");
            DropForeignKey("dbo.ProfileSkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.ProfileSkills", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "LastUpdatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Profiles", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AgentSkills", new[] { "SkillId" });
            DropIndex("dbo.AgentSkills", new[] { "AgentId" });
            DropIndex("dbo.AgentWorkgroups", new[] { "WorkgroupId" });
            DropIndex("dbo.AgentWorkgroups", new[] { "AgentId" });
            DropIndex("dbo.ProfileSkills", new[] { "SkillId" });
            DropIndex("dbo.ProfileSkills", new[] { "ProfileId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Chats", new[] { "UserData_UserDataId" });
            DropIndex("dbo.Utilizations", new[] { "Agent_AgentId" });
            DropIndex("dbo.FormFields", new[] { "Form_FormId" });
            DropIndex("dbo.Widgets", new[] { "Form_FormId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Profiles", new[] { "Workgroup_WorkgroupId" });
            DropIndex("dbo.Profiles", new[] { "Widget_WidgetId" });
            DropIndex("dbo.Profiles", new[] { "Template_TemplateId" });
            DropIndex("dbo.Profiles", new[] { "LastUpdatedBy_Id" });
            DropIndex("dbo.Profiles", new[] { "CreatedBy_Id" });
            DropTable("dbo.AgentSkills");
            DropTable("dbo.AgentWorkgroups");
            DropTable("dbo.ProfileSkills");
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Languages");
            DropTable("dbo.UserDatas");
            DropTable("dbo.Chats");
            DropTable("dbo.Utilizations");
            DropTable("dbo.FormFields");
            DropTable("dbo.Forms");
            DropTable("dbo.Widgets");
            DropTable("dbo.Templates");
            DropTable("dbo.Skills");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Profiles");
            DropTable("dbo.Workgroups");
            DropTable("dbo.Agents");
        }
    }
}
