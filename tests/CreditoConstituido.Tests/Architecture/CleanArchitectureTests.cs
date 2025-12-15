using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetArchTest.Rules;
using System.Reflection;

namespace CreditoConstituido.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    private static readonly Assembly Domain = typeof(CreditoConstituido.Domain.Entities.Credito).Assembly;
    private static readonly Assembly Application = typeof(CreditoConstituido.Application.DependencyInjection).Assembly;
    private static readonly Assembly Infrastructure = typeof(CreditoConstituido.Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly Api = typeof(Program).Assembly;

    [Fact]
    public void Domain_NaoDeveDepender_DeApplicationInfrastructureOuApi()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                Application.GetName().Name!,
                Infrastructure.GetName().Name!,
                Api.GetName().Name!
            )
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailMessage(result));
    }

    [Fact]
    public void Application_NaoDeveDepender_DeInfrastructureOuApi()
    {
        var result = Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                Infrastructure.GetName().Name!,
                Api.GetName().Name!
            )
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailMessage(result));
    }

    [Fact]
    public void Infrastructure_NaoDeveDepender_DeApi()
    {
        var result = Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(Api.GetName().Name!)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailMessage(result));
    }

    [Fact]
    public void Api_DeveDepender_DeApplication_E_PodeDepender_DeInfrastructure_E_NaoDeveDepender_DeDomain()
    {
        var appName = Application.GetName().Name!;
        var infraName = Infrastructure.GetName().Name!;
        var domainName = Domain.GetName().Name!;

        var dependeDeApp = Types.InAssembly(Api)
            .Should()
            .HaveDependencyOnAny(appName)
            .GetResult();

        dependeDeApp.IsSuccessful.Should().BeTrue(BuildFailMessage(dependeDeApp));

        var naoDependeDeDomain = Types.InAssembly(Api)
            .ShouldNot()
            .HaveDependencyOnAny(domainName)
            .GetResult();

        naoDependeDeDomain.IsSuccessful.Should().BeTrue(BuildFailMessage(naoDependeDeDomain));

        var podeDependerDeInfra = Types.InAssembly(Api)
            .Should()
            .HaveDependencyOnAny(infraName)
            .GetResult();

        podeDependerDeInfra.IsSuccessful.Should().BeTrue(BuildFailMessage(podeDependerDeInfra));
    }


    private static string BuildFailMessage(TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        if (result.FailingTypeNames is null || result.FailingTypeNames.Count == 0) return "Regra de arquitetura falhou, mas não retornou tipos violadores.";
        return $"Tipos que violaram: {string.Join(", ", result.FailingTypeNames)}";
    }
}
