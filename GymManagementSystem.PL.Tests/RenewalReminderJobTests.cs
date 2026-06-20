using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using GymManagementSystem.PL.Jobs;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Quartz;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class RenewalReminderJobTests
{
    [Fact]
    public async Task Execute_TwoExpiringMemberships_SendsTwoEmails()
    {
        var membershipRepo = Substitute.For<IRepository<Membership>>();
        var emailService = Substitute.For<IEmailService>();
        var notificationService = Substitute.For<INotificationService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<RenewalReminderJob>>();

        var now = DateTime.UtcNow.Date;

        var memberships = new List<Membership>
        {
            new()
            {
                Id = 1,
                MemberId = 1,
                Member = new Member { Id = 1, Email = "member1@test.com", FirstName = "A", LastName = "B" },
                Plan = new Plan { Id = 1, Name = "Gold" },
                EndDate = now.AddDays(7),
                IsActive = true,
                StartDate = now.AddDays(-30),
                ReminderDaysSent = 0
            },
            new()
            {
                Id = 2,
                MemberId = 2,
                Member = new Member { Id = 2, Email = "member2@test.com", FirstName = "C", LastName = "D" },
                Plan = new Plan { Id = 2, Name = "Silver" },
                EndDate = now.AddDays(7),
                IsActive = true,
                StartDate = now.AddDays(-30),
                ReminderDaysSent = 0
            }
        };

        var queryable = TestAsyncEnumerable(memberships);
        membershipRepo.Query().Returns(queryable);

        var scope = Substitute.For<IServiceScope>();
        var sp = Substitute.For<IServiceProvider>();
        sp.GetService(typeof(IRepository<Membership>)).Returns(membershipRepo);
        sp.GetService(typeof(IEmailService)).Returns(emailService);
        sp.GetService(typeof(INotificationService)).Returns(notificationService);
        sp.GetService(typeof(IUnitOfWork)).Returns(unitOfWork);
        sp.GetService(typeof(ILogger<RenewalReminderJob>)).Returns(logger);
        scope.ServiceProvider.Returns(sp);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        var job = new RenewalReminderJob(scopeFactory);
        var context = Substitute.For<IJobExecutionContext>();

        await job.Execute(context);

        await emailService.Received(2).SendRenewalReminderAsync(
            Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());
        await notificationService.Received(2).SendToUserAsync(
            Arg.Any<int>(), Arg.Any<string>());
        await unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Execute_AlreadySentReminder_DoesNotSendDuplicate()
    {
        var membershipRepo = Substitute.For<IRepository<Membership>>();
        var emailService = Substitute.For<IEmailService>();
        var notificationService = Substitute.For<INotificationService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<RenewalReminderJob>>();

        var now = DateTime.UtcNow.Date;

        var memberships = new List<Membership>
        {
            new()
            {
                Id = 1,
                MemberId = 1,
                Member = new Member { Id = 1, Email = "member1@test.com", FirstName = "A", LastName = "B" },
                Plan = new Plan { Id = 1, Name = "Gold" },
                EndDate = now.AddDays(7),
                IsActive = true,
                StartDate = now.AddDays(-30),
                ReminderDaysSent = 7,
                ReminderSentAt = now
            },
            new()
            {
                Id = 2,
                MemberId = 2,
                Member = new Member { Id = 2, Email = "member2@test.com", FirstName = "C", LastName = "D" },
                Plan = new Plan { Id = 2, Name = "Silver" },
                EndDate = now.AddDays(1),
                IsActive = true,
                StartDate = now.AddDays(-30),
                ReminderDaysSent = 1,
                ReminderSentAt = now
            }
        };

        var queryable = TestAsyncEnumerable(memberships);
        membershipRepo.Query().Returns(queryable);

        var scope = Substitute.For<IServiceScope>();
        var sp = Substitute.For<IServiceProvider>();
        sp.GetService(typeof(IRepository<Membership>)).Returns(membershipRepo);
        sp.GetService(typeof(IEmailService)).Returns(emailService);
        sp.GetService(typeof(INotificationService)).Returns(notificationService);
        sp.GetService(typeof(IUnitOfWork)).Returns(unitOfWork);
        sp.GetService(typeof(ILogger<RenewalReminderJob>)).Returns(logger);
        scope.ServiceProvider.Returns(sp);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        var job = new RenewalReminderJob(scopeFactory);
        var context = Substitute.For<IJobExecutionContext>();

        await job.Execute(context);

        await emailService.DidNotReceiveWithAnyArgs().SendRenewalReminderAsync(
            Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());
        await notificationService.DidNotReceiveWithAnyArgs().SendToUserAsync(
            Arg.Any<int>(), Arg.Any<string>());
        await unitOfWork.DidNotReceive().CompleteAsync();
    }

    private static IQueryable<T> TestAsyncEnumerable<T>(List<T> source) where T : class
    {
        return new TestAsyncEnumerableQueryable<T>(source);
    }

    private class TestAsyncEnumerableQueryable<T> : List<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly IQueryable<T> _queryable;

        public TestAsyncEnumerableQueryable(IEnumerable<T> items) : base(items)
        {
            _queryable = this.AsQueryable();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(_queryable.Provider);
        Expression IQueryable.Expression => _queryable.Expression;
        Type IQueryable.ElementType => _queryable.ElementType;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
    }

    private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerableQueryable<T>(_inner.CreateQuery<T>(expression));
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerableQueryable<TElement>(_inner.CreateQuery<TElement>(expression));
        public object? Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute(expression);
            if (result is Task task) return (TResult)((dynamic)task);
            return (TResult)result;
        }
    }
}
