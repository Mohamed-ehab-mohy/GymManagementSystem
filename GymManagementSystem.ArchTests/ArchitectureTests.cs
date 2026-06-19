using System.Reflection;
using NetArchTest.Rules;
using Shouldly;

namespace GymManagementSystem.ArchTests;

public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(GymManagementSystem.Domain.Member).Assembly;
    private static readonly Assembly BllAssembly = typeof(GymManagementSystem.BLL.Interfaces.IMemberService).Assembly;
    private static readonly Assembly DalAssembly = typeof(GymManagementSystem.DAL.DbContexts.GymDbContext).Assembly;
    private static readonly Assembly PlAssembly = typeof(Program).Assembly;

    [Fact]
    public void BLL_Must_Not_Reference_DAL()
    {
        var result = Types.InAssembly(BllAssembly)
            .ShouldNot().HaveDependencyOn("GymManagementSystem.DAL")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Controllers_Must_Not_Import_EF_Directly()
    {
        var result = Types.InAssembly(PlAssembly)
            .That().Inherit(typeof(Microsoft.AspNetCore.Mvc.Controller))
            .ShouldNot().HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void All_Service_Classes_Must_Implement_Corresponding_Interface()
    {
        var serviceClasses = Types.InAssembly(BllAssembly)
            .That().HaveNameEndingWith("Service")
            .And().AreClasses()
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var service in serviceClasses)
        {
            var interfaces = service.GetInterfaces();
            interfaces.ShouldNotBeEmpty($"'{service.FullName}' does not implement any interface");
        }
    }

    [Fact]
    public void All_Repositories_Must_Live_In_DataAccess()
    {
        var result = Types.InAssembly(DalAssembly)
            .That().HaveNameEndingWith("Repository")
            .Should().ResideInNamespace("GymManagementSystem.DAL")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
