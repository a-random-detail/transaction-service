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
        Assert.That(result.GetErrors().Count, Is.EqualTo(0));
        Assert.That(result.GetValue(), Is.EqualTo(randomNumber));
    }

    [Test]
    public void Result_Fail_Produces_Error()
    {
        var error = "Unable to do whatever thing we were doing.";
        var result = Result<long>.Fail(error);
        Assert.That(result.Success, Is.False);
        Assert.That(result.GetErrors().Count, Is.EqualTo(1));
        Assert.That(result.GetValue(), Is.EqualTo(default(long)));
    }

    [Test]
    public void Result_Fail_Keeps_All_Errors()
    {
        string[] errors = ["error1", "error2", "error3"];

        var result = Result<int>.Fail(errors);
        Assert.That(result.Success, Is.False);
        Assert.That(result.GetErrors().Count, Is.EqualTo(errors.Length));
        Assert.That(result.GetErrors(), Is.EqualTo(errors.ToList()));
        Assert.That(result.GetValue(), Is.EqualTo(default(int)));
    }
}
