using Microsoft.EntityFrameworkCore;

namespace EF_Course.Entities
{
    public class BoardsDbContext : DbContext
    {
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WorkItemState> WorkItemStates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(wi => wi.Efford).HasColumnType("decimal(5,2)");
                eb.Property(wi => wi.EndDate).HasPrecision(3);
                eb.Property(wi => wi.Activity).HasMaxLength(200);
                eb.Property(wi => wi.RemainingWork).HasPrecision(14,2);
                //eb.Property(wi => wi.State).IsRequired();
                eb.Property(wi => wi.Area).HasColumnType("varchar(200)");
                eb.Property(wi => wi.Priority).HasDefaultValue(1);
                eb.HasMany(wi => wi.Comments) //1-many
                    .WithOne(c => c.WorkItem)
                    .HasForeignKey(c => c.WorkItemId);
                
                eb.HasOne(wi => wi.Author) // 1-many
                    .WithMany(u => u.WorkItems)
                    .HasForeignKey(wi => wi.AuthorId);

                eb.HasMany(wi => wi.Tags)
                    .WithMany(t => t.WorkItems)
                    .UsingEntity<WorkItemTag>(
                        wi => wi.HasOne(wit => wit.Tag)
                        .WithMany()
                        .HasForeignKey(wit => wit.TagId),

                        wi => wi.HasOne(wit => wit.WorkItem)
                        .WithMany()
                        .HasForeignKey(wit => wit.WorkItemId),

                        wit =>
                        {
                            wit.HasKey(x => new { x.TagId, x.WorkItemId });
                            wit.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                        });

                eb.HasOne(wi => wi.State)
                    .WithMany()
                    .HasForeignKey(wi => wi.StateId);
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(c => c.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(c => c.CreatedDate).ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Address) //1-1
                .WithOne(a=>a.User)
                .HasForeignKey<Address>(a=>a.UserId);

            modelBuilder.Entity<WorkItemState>()
                .Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(50);

            //modelBuilder.Entity<WorkItemTag>()
            //      .HasKey(x => new { x.WorkItemId, x.TagId }); //CompoudKey

        }

        public BoardsDbContext(DbContextOptions<BoardsDbContext> options) : base(options)
        {

        }

    }
}
