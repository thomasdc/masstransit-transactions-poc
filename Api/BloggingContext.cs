using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasKey(_ => _.BlogId);
            modelBuilder.Entity<Blog>().Property(_ => _.BlogId).UseIdentityColumn();
            modelBuilder.Entity<Blog>().Property(_ => _.Url);
        }
    }

    public class Blog
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int BlogId { get; private set; }
        public string Url { get; set; }
    }
}
