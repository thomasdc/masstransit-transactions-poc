using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
            
        }
    }

    public class Blog
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int BlogId { get; private set; }
        public string Url { get; set; }
    }
}
