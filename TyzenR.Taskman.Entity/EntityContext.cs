using Microsoft.EntityFrameworkCore;

namespace TyzenR.Taskman.Entity
{
    public class EntityContext : DbContext
    {
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskAttachmentEntity> TaskAttachments { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }

        public DbSet<ActionTrackerEntity> ActionTrackers { get; set; }
        public DbSet<EmailTrackerEntity> EmailTrackers { get; set; }
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
