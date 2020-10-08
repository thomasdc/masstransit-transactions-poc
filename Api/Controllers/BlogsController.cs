using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly ILogger<BlogsController> _logger;
        private readonly BloggingContext _context;

        public BlogsController(ILogger<BlogsController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Blog>> Get()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var maxId = await _context.Blogs.MaxAsync(_ => _.BlogId);
            var blog = new Blog { BlogId = maxId + 1, Url = $"http://foo.bar/{Guid.NewGuid()}" };
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            var blogs = await _context.Blogs.ToListAsync();
            scope.Complete();
            return blogs;
        }
    }
}
