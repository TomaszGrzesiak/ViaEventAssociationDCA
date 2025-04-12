using Domain;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResult;

public class EmailAddressUnitTests
{
    [Fact]
    public void GivenProperEmail_CanCreateEmailAddress_Successfully()
    {
        string correctEmail1MixedCase = "TOgr@via.dk"; // should be converted to lower case
        string correctEmail1 = "togr@via.dk"; 
        string correctEmail2 = "fra@via.dk";
        string correctEmail3 = "123456@via.dk";
        
        Result<EmailAddress> result1MixedCase = EmailAddress.Create(correctEmail1MixedCase);
        Result<EmailAddress> result1 = EmailAddress.Create(correctEmail1);
        Result<EmailAddress> result2 = EmailAddress.Create(correctEmail2);
        Result<EmailAddress> result3 = EmailAddress.Create(correctEmail3);
        
        Assert.True(result1MixedCase.IsSuccess);
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsSuccess);
        
        Assert.NotNull(result1MixedCase.Payload);
        Assert.NotNull(result1.Payload);
        Assert.NotNull(result2.Payload);
        Assert.NotNull(result3.Payload);
        
        Assert.Equal(correctEmail1, result1MixedCase.Payload.Value);
        Assert.Equal(correctEmail1, result1.Payload.Value);
        Assert.Equal(correctEmail2, result2.Payload.Value);
        Assert.Equal(correctEmail3, result3.Payload.Value);
    }
    
    // another way of Unit Testing of different cases:
    // [Theory] and [InlineData(...)] are xUnit attributes used to write parameterized tests, meaning the same test logic runs with different input values.
    [Theory]
    [InlineData("abc@via.dk", "abc@via.dk")]     // 3 letters
    [InlineData("ABCD@via.dk", "abcd@via.dk")]   // 4 letters
    [InlineData("123456@via.dk", "123456@via.dk")] // 6 digits
    public void ValidViaEmails_ShouldSucceed(string input, string expected)
    {
        var result = EmailAddress.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Payload!.Value);
    }
    
    [Fact]
    public void GivenNonViaEmail_ShouldFailWithDomainError()
    {
        string testEmail = "abcd@gmail.com";
        Result<EmailAddress> result = EmailAddress.Create(testEmail);

        Assert.True(result.IsFailure);
        Assert.Contains("must end with '@via.dk'", result.ErrorMessages[0], StringComparison.OrdinalIgnoreCase);
    }
    
    [Theory]
    // (only 3–4 or 6 digits)
    [InlineData("12abcd@via.dk")] // text1 more than 6 characters
    [InlineData("ab@via.dk")] // text1 too short
    [InlineData("abcde@via.dk")] // 5-letter text1
    
    // other
    [InlineData("abcd.via.dk")] // missing @
    [InlineData("abcd@viadk")] // no dot
    [InlineData("12ab@via.dk")] // mixed letters and digits
    public void GivenIncorrectEmailFormat_ShouldFail(string testEmail)
    {
        Result<EmailAddress> result = EmailAddress.Create(testEmail);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.ErrorMessages);
    }
}

