using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public sealed class ProfilePictureUrl : ValueObject
{
    public string? Value { get; }

    private ProfilePictureUrl(string? url)
    {
        Value = url;
    }

    public static ProfilePictureUrl Create(string? input)
    {
        return new ProfilePictureUrl(input);
    }

    public static Result<ProfilePictureUrl> Validate(ProfilePictureUrl? pictureUrl)
    {
        var url = pictureUrl.Value;
        if (string.IsNullOrWhiteSpace(url))
            return Result<ProfilePictureUrl>.Failure(Error.InvalidProfilePictureUrlEmpty);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return Result<ProfilePictureUrl>.Failure(Error.InvalidProfilePictureUrlEmpty);

        if ((uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return Result<ProfilePictureUrl>.Failure(Error.InvalidProfilePictureUrlEmpty);

        return Result<ProfilePictureUrl>.Success(new ProfilePictureUrl(url));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? "";
    }

    public override string ToString() => Value ?? "";
}