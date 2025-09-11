using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class GuestTestsId10
{
    string em = "308817@via.dk";
    string fm = "TomaSZ";
    string lm = "GRZesiak";
    string ppu = "https://example.com/pic.jpg";
    EmailAddress email;
    GuestName firstName;
    GuestName lastName;
    ProfilePictureUrl pictureUrl;

    public GuestTestsId10()
    {
        firstName = GuestName.Create(fm);
        lastName = GuestName.Create(lm);
        pictureUrl = ProfilePictureUrl.Create(ppu);
        email = EmailAddress.Create(em);
    }

    private string Normalize(string input, bool capitalizeFirstLetter = true)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        input = input.Trim().ToLower();

        if (!capitalizeFirstLetter)
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }


    [Fact]
    public void Id10_S1_SuccessfullyRegistersGuest_WhenAllDataValid()
    {
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsSuccess);
        var guest = result.Payload!;

        Assert.NotNull(guest.Id);
        Assert.Equal(Normalize(em, false), guest.Email.Value);
        Assert.Equal(Normalize(fm), guest.FirstName.Value);
        Assert.Equal(Normalize(lm), guest.LastName.Value);
        Assert.Equal(ppu, guest.ProfilePictureUrlAddress.Value);
    }

    [Fact]
    public void Id10_F1_Failure_WhenEmailDomainIncorrect()
    {
        email = EmailAddress.Create("308817@gmail.com");
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EmailMustEndWithViaDomain);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("text1@text2.dk")]
    [InlineData("308817@text2.dk")]
    [InlineData("togr@2via.dk")]
    [InlineData("text1@via.dk")]
    [InlineData("3081@via.dk")]
    public void Id10_F2_Failure_WhenEmailFormatInvalid(string emailAddress) // ()
    {
        email = EmailAddress.Create(emailAddress);
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e == Error.EmailInvalidFormat || e == Error.EmailMustEndWithViaDomain);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")] // 26 letters
    public void Id10_F3_Failure_WhenFirstNameInvalid(string name)
    {
        firstName = GuestName.Create(name);
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")] // 26 letters
    public void Id10_F4_Failure_WhenLastNameInvalid(string name)
    {
        lastName = GuestName.Create(name);
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    // todo: You need to save it to some sort of store. But it can't be in domain. So there must be a domain contract, that allows injecting a an email checker. 
    [Fact]
    public void Id10_F5_Failure_WhenEmailAlreadyRegistered()
    {
        email = EmailAddress.Create("989898@via.dk");
        var result = Guest.Register(email, firstName, lastName, pictureUrl);
        Assert.True(result.IsSuccess);

        var duplicateResult = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(duplicateResult.IsFailure);
        Assert.Contains(duplicateResult.Errors, e => e == Error.EmailAlreadyRegistered);
    }

    [Fact]
    public void Id10_F6_Failure_WhenNamesContainNonLetters()
    {
        firstName = GuestName.Create("Tom1"); // invalid first name
        lastName = GuestName.Create("Fran!"); // invalid last name
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    [Theory]
    [InlineData("htp:/wrong-url")] // invalid URL
    [InlineData("")]
    public void Id10_F7_Failure_WhenUrlIsInvalid(string badUrl)
    {
        pictureUrl = ProfilePictureUrl.Create(badUrl);
        var result = Guest.Register(email, firstName, lastName, pictureUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidProfilePictureUrlEmpty);
    }
}