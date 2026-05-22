using System.Reflection;
using TransactionService.Domain.Core;

namespace TransactionService.Domain.Test.Core;

[TestFixture]
public class Result_Test
{
    [Test]
    public void Result_OK_Returns_Data_With_No_Errors()
    {
        var randomNumber = new Random().NextInt64();
        var result = Result<long>.OK(randomNumber);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Type, Is.EqualTo(ResultType.Ok));
        Assert.That(result.GetErrors().Count, Is.EqualTo(0));
        Assert.That(result.GetValue(), Is.EqualTo(randomNumber));
    }

    [TestCase(ResultType.NotFound)]
    [TestCase(ResultType.ValidationError)]
    [TestCase(ResultType.NoRecordsFound)]
    [TestCase(ResultType.ServerError)]
    public void Result_Fail_Produces_Error(ResultType type)
    {
        var error = "Unable to do whatever thing we were doing.";
        var result = Result<long>.Fail(type, error);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Type, Is.EqualTo(type));
        Assert.That(result.GetErrors().Count, Is.EqualTo(1));
        Assert.That(result.GetValue(), Is.EqualTo(default(long)));
    }

    [Test]
    public void Result_Fail_Keeps_All_Errors()
    {
        string[] errors = ["error1", "error2", "error3"];

        var result = Result<int>.Fail(ResultType.ValidationError, errors);
        Assert.That(result.Success, Is.False);
        Assert.That(result.GetErrors().Count, Is.EqualTo(errors.Length));
        Assert.That(result.GetErrors(), Is.EqualTo(errors.ToList()));
        Assert.That(result.GetValue(), Is.EqualTo(default(int)));
    }
    
    [Test]
    public void Result_Cannot_Be_Instantiated_Directly()
    {
        var constructor = typeof(Result<string>)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes);
        
        Assert.That(constructor?.IsPrivate, Is.True);
    }
    
    [Test]
    public void Result_OK_With_Null_Value_Is_Still_Success()
    {
        var result = Result<string>.OK(null);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Type, Is.EqualTo(ResultType.Ok));
        Assert.That(result.GetValue(), Is.Null);
    }
    
    [Test]
    public void Result_Fail_With_Ok_Type_Throws()
    {
        Assert.Throws<ArgumentException>(() => Result<string>.Fail(ResultType.Ok, "error"));
    }
}
