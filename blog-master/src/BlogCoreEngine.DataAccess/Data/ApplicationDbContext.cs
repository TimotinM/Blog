using BlogCoreEngine.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogCoreEngine.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<OptionDataModel> Options { get; set; }
        public DbSet<PostDataModel> Posts { get; set; }
        public DbSet<CommentDataModel> Comments { get; set; }
        public DbSet<BlogDataModel> Blogs { get; set; }
        public DbSet<UsersBlogs> UsersBlogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
               builder.Entity<UsersBlogs>()
                       .HasKey(x => new { x.BlogId, x.UserId });

               builder.Entity<UsersBlogs>()
                       .HasOne(x => x.User)
                       .WithMany(x => x.Blogs)
                       .HasForeignKey(x => x.UserId);

               builder.Entity<UsersBlogs>()
                       .HasOne(x => x.Blog)
                       .WithMany(x => x.Authors)
                       .HasForeignKey(x => x.BlogId);

               base.OnModelCreating(builder);
            ApplicationSeed.ApplySeed(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
