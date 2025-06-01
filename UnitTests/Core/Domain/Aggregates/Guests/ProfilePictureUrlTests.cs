using System;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;
using Xunit;

public class ProfilePictureUrlTests
{
    [Fact]
    public void Create_WithValidHttpsUrl_ReturnsSuccess()
    {
        var input = "https://example.com/images/profile.jpg";

        var result = ProfilePictureUrl.Create(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal(input, result.Payload.Value.ToString());
    }

    [Fact]
    public void Create_WithNullOrWhitespace_ReturnsFailure_EmptyError()
    {
        var result1 = ProfilePictureUrl.Create(null);
        var result2 = ProfilePictureUrl.Create("   ");

        Assert.True(result1.IsFailure);
        Assert.Single(result1.Errors);
        Assert.Equal(Error.InvalidProfilePictureUrlEmpty, result1.Errors[0]);

        Assert.True(result2.IsFailure);
        Assert.Single(result2.Errors);
        Assert.Equal(Error.InvalidProfilePictureUrlEmpty, result2.Errors[0]);
    }

    [Theory]
    [InlineData("http:/badformat.com")]
    [InlineData("example.com/image.jpg")]
    [InlineData("/relative/path.jpg")]
    [InlineData("not_a_url")]
    public void Create_WithInvalidFormat_ReturnsFailure_OtherError(string input)
    {
        var result = ProfilePictureUrl.Create(input);

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(Error.InvalidProfilePictureUrlOther, result.Errors[0]);
    }

    [Fact]
    public void Create_WithUnsupportedScheme_ReturnsFailure_SchemeError()
    {
        var input = "ftp://example.com/image.png";
        var result = ProfilePictureUrl.Create(input);

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(Error.OnlyHttpOrHttpsAllowed, result.Errors[0]);
    }

    [Fact]
    public void Equality_WorksForSameUrl()
    {
        var r1 = ProfilePictureUrl.Create("https://example.com/me.png").Payload;
        var r2 = ProfilePictureUrl.Create("https://example.com/me.png").Payload;

        Assert.Equal(r1, r2);
    }
}