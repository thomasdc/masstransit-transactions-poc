using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using MassTransit.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly BloggingContext _context;
        private readonly ITransactionalBus _transactionalBus;
        private readonly ILoggerFactory _loggerFactory;

        public BlogsController(BloggingContext context, ITransactionalBus transactionalBus, ILoggerFactory loggerFactory)
        {
            _context = context;
            _transactionalBus = transactionalBus;
            _loggerFactory = loggerFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Blog>> Get()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var blog = new Blog { Url = $"http://foo.bar/{Guid.NewGuid()}" };
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            Transaction.Current.EnlistVolatile(new FakeEnlistmentNotification(_loggerFactory.CreateLogger<FakeEnlistmentNotification>()),
                EnlistmentOptions.EnlistDuringPrepareRequired);

            await _transactionalBus.Publish<INewBlogRegistered>(new {blog.BlogId});

            var blogs = await _context.Blogs.ToListAsync();
            scope.Complete();
            return blogs;
        }

        class FakeEnlistmentNotification : IEnlistmentNotification
        {
            private readonly ILogger<FakeEnlistmentNotification> _logger;

            public FakeEnlistmentNotification(ILogger<FakeEnlistmentNotification> logger)
            {
                _logger = logger;
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                _logger.LogInformation($"{nameof(Prepare)} was called");
                preparingEnlistment.Prepared();
            }

            public void Commit(Enlistment enlistment)
            {
                _logger.LogInformation($"{nameof(Commit)} was called");
                enlistment.Done();
            }

            public void Rollback(Enlistment enlistment)
            {
                _logger.LogInformation($"{nameof(Rollback)} was called");
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                _logger.LogInformation($"{nameof(InDoubt)} was called");
                enlistment.Done();
            }
        }
    }
}
