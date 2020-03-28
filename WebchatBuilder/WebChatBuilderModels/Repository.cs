using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using WebChatBuilderModels.Models;

namespace WebChatBuilderModels
{
    public class Repository : IdentityDbContext<ApplicationUser>
    {
        public Repository()
            : base("Repository", false)
        {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Workgroup> Workgroups { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Utilization> Utilizations { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<UserData> UsersData { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormOption> FormOptions { get; set; }
        public DbSet<CustomMessage> CustomMessages { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleRecurrence> ScheduleRecurrences { get; set; }
        public DbSet<VisitorMessage> VisitorMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .HasMany<Workgroup>(x => x.ActiveInWorkgroups)
                .WithMany(x => x.ActiveMembers)
                .Map(
                    m =>
                    {
                        m.ToTable("AgentWorkgroups");
                        m.MapLeftKey("AgentId");
                        m.MapRightKey("WorkgroupId");
                    });

            modelBuilder.Entity<Agent>()
                .HasMany<Skill>(x => x.Skills)
                .WithMany(x => x.AssignedAgents)
                .Map(
                    m =>
                    {
                        m.ToTable("AgentSkills");
                        m.MapLeftKey("AgentId");
                        m.MapRightKey("SkillId");
                    });

            modelBuilder.Entity<Profile>()
                .HasMany<Skill>(x => x.Skills)
                .WithMany(x => x.ConfiguredProfiles)
                .Map(
                    m =>
                    {
                        m.ToTable("ProfileSkills");
                        m.MapLeftKey("ProfileId");
                        m.MapRightKey("SkillId");
                    });

            modelBuilder.Entity<Profile>()
                .HasMany<Schedule>(x => x.Schedules)
                .WithMany(x => x.Profiles)
                .Map(
                    m =>
                    {
                        m.ToTable("ProfileSchedules");
                        m.MapLeftKey("ProfileId");
                        m.MapRightKey("ScheduleId");
                    });

            modelBuilder.Entity<Agent>().HasMany<Utilization>(x => x.Utilizations);
            modelBuilder.Entity<Workgroup>().HasMany<Utilization>(x => x.Utilizations);
            modelBuilder.Entity<Workgroup>().HasMany<Profile>(x => x.Profiles);
            modelBuilder.Entity<Form>().HasMany<FormField>(x => x.FormFields);
            modelBuilder.Entity<FormField>().HasMany<FormOption>(x => x.SelectOptions);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public static Repository Create()
        {
            return new Repository();
        }
    }
}