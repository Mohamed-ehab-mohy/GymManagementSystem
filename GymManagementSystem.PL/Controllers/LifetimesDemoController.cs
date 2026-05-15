using GymManagementSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class LifetimesDemoController : Controller
{
    private readonly IScopedService    _scoped1;
    private readonly ITransientService _transient1;
    private readonly ISingletonService _singleton1;

    private readonly IScopedService    _scoped2;
    private readonly ITransientService _transient2;
    private readonly ISingletonService _singleton2;

    public LifetimesDemoController(
        IScopedService    scoped1,
        ITransientService transient1,
        ISingletonService singleton1,
        IScopedService    scoped2,
        ITransientService transient2,
        ISingletonService singleton2)
    {
        _scoped1    = scoped1;
        _transient1 = transient1;
        _singleton1 = singleton1;
        _scoped2    = scoped2;
        _transient2 = transient2;
        _singleton2 = singleton2;
    }

    public IActionResult Index()
    {
        ViewBag.Scoped1    = _scoped1.OperationId;
        ViewBag.Scoped2    = _scoped2.OperationId;
        ViewBag.ScopedSame = _scoped1.OperationId == _scoped2.OperationId;

        ViewBag.Transient1    = _transient1.OperationId;
        ViewBag.Transient2    = _transient2.OperationId;
        ViewBag.TransientSame = _transient1.OperationId == _transient2.OperationId;

        ViewBag.Singleton1    = _singleton1.OperationId;
        ViewBag.Singleton2    = _singleton2.OperationId;
        ViewBag.SingletonSame = _singleton1.OperationId == _singleton2.OperationId;

        return View();
    }
}
