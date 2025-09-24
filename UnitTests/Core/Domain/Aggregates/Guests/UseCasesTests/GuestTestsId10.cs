using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class GuestTestsId10
{
    private const string Em = "308817@via.dk";
    private const string Fm = "TomaSZ";
    private const string Lm = "GRZesiak";
    private const string Ppu = "https://example.com/pic.jpg";
    private EmailAddress _email = EmailAddress.Create(Em).Payload!;
    private GuestName _firstName = GuestName.Create(Fm).Payload!;
    private GuestName _lastName = GuestName.Create(Lm).Payload!;
    private ProfilePictureUrl _pictureUrl = ProfilePictureUrl.Create(Ppu).Payload!;

    private IEmailUnusedChecker _emailUnusedChecker = new FakeUniqueEmailChecker([]);

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
    public async void Id10_S1_SuccessfullyRegistersGuest_WhenAllDataValid()
    {
        var result = await Guest.Register(GuestId.CreateUnique(), _email, _firstName, _lastName, _pictureUrl, _emailUnusedChecker);

        Assert.True(result.IsSuccess);
        var guest = result.Payload!;

        Assert.NotNull(guest.Id);
        Assert.Equal(Normalize(Em, false), guest.Email.Value);
        Assert.Equal(Normalize(Fm), guest.FirstName.Value);
        Assert.Equal(Normalize(Lm), guest.LastName.Value);
        Assert.Equal(Ppu, guest.ProfilePictureUrlAddress.Value);
    }

    [Fact]
    public void Id10_F1_Failure_WhenEmailDomainIncorrect()
    {
        var result = EmailAddress.Create("308817@gmail.com");

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
        var result = EmailAddress.Create(emailAddress);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e =>
            e == Error.EmailInvalidFormat || e == Error.EmailMustEndWithViaDomain);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")] // 26 letters
    public void Id10_F3_Failure_WhenFirstNameInvalid(string name)
    {
        var result = GuestName.Create(name);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")] // 26 letters
    public void Id10_F4_Failure_WhenLastNameInvalid(string name)
    {
        var result = GuestName.Create(name);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    [Fact]
    public async void Id10_F5_Failure_WhenEmailAlreadyRegistered()
    {
        // Arrange
        _emailUnusedChecker = new FakeUniqueEmailChecker(["308817@via.dk"]);
        _email = EmailAddress.Create("308817@via.dk").Payload!;

        // Act
        var result = await Guest.Register(GuestId.CreateUnique(), _email, _firstName, _lastName, _pictureUrl, _emailUnusedChecker);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EmailAlreadyRegistered);
    }

    [Theory]
    [InlineData("Tom1")]
    [InlineData("Fran!")]
    [InlineData("12345")]
    public async void Id10_F6_Failure_WhenNamesContainNonLetters(string invalidName)
    {
        var result = GuestName.Create(invalidName); // invalid first name

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidNameFormat);
    }

    [Theory]
    [InlineData("htp:/wrong-url")] // invalid URL
    [InlineData("")]
    public async void Id10_F7_Failure_WhenUrlIsInvalid(string badUrl)
    {
        var result = ProfilePictureUrl.Create(badUrl);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvalidProfilePictureUrlEmpty);
    }
}