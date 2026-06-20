using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace GymManagementSystem.BLL.Tests.Infrastructure;

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}

public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(_inner.CreateQuery<T>(expression));
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(_inner.CreateQuery<TElement>(expression));

    public object? Execute(Expression expression) => _inner.Execute(expression);
    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var result = Execute(expression);
        if (result is Task task)
        {
            var taskType = task.GetType();
            if (taskType.IsGenericType)
            {
                var innerResult = taskType.GetProperty("Result")?.GetValue(task);
                var tResultType = typeof(TResult);
                if (tResultType.IsGenericType && tResultType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var innerType = tResultType.GetGenericArguments()[0];
                    var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(innerType);
                    return (TResult)fromResult.Invoke(null, [innerResult])!;
                }
            }
            return (TResult)(object)Task.CompletedTask;
        }
        var resultType = typeof(TResult);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(innerType);
            return (TResult)fromResult.Invoke(null, [result])!;
        }
        return (TResult)result;
    }
}
