using System.Reflection;
using TransactionService.Contracts;

namespace TransactionService.Test.Contracts;

[TestFixture]
public class ApiResponse_Test
{
    [Test]
    public void ApiResponse_OK_Returns_Data_With_No_Errors()
    {
        var randomNumber = new Random().NextInt64();
        var result = ApiResponse<long>.OK(randomNumber);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Errors.Count, Is.EqualTo(0));
        Assert.That(result.Data, Is.EqualTo(randomNumber));
    }

    [Test]
    public void ApiResponse_Fail_Produces_Error()
    {
        var error = "Unable to do whatever thing we were doing.";
        var result = ApiResponse<long>.Fail(error);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Data, Is.EqualTo(default(long)));
    }

    [Test]
    public void ApiResponse_Fail_Keeps_All_Errors()
    {
        string[] errors = ["error1", "error2", "error3"];

        var result = ApiResponse<int>.Fail(errors);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(errors.Length));
        Assert.That(result.Errors, Is.EqualTo(errors.ToList()));
        Assert.That(result.Data, Is.EqualTo(default(int)));
    }

    [Test]
    public void ApiResponse_Cannot_Be_Instantiated_Directly()
    {
        var constructor = typeof(ApiResponse<string>)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes);
        
        Assert.That(constructor?.IsPrivate, Is.True);
    }
}
