using Domain;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResult;

public class EmailAddressUnitTests
{
    [Fact]
    public void GivenProperEmail_CanCreateEmailAddress()
    {
        String testEmailAddress = "togr@via.dk";
        Result<EmailAddress> result = EmailAddress.Create(testEmailAddress);
        Assert.True(result.IsSuccess);
        EmailAddress emailAddress = result.Payload;
        Assert.Equal(testEmailAddress, emailAddress.Value);
    }
    
    [Fact]
    public void GivenRawString_FailsWhenEmailIsIncorrectFormat()
    {
        String testEmailAddress = "rawString";
        Result<EmailAddress> result = EmailAddress.Create(testEmailAddress);
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessages);
    }
}