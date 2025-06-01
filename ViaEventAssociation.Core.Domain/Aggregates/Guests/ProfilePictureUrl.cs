using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public sealed class ProfilePictureUrl : ValueObject
{
    public Uri Value { get; }

    private ProfilePictureUrl(Uri uri)
    {
        Value = uri;
    }

    public static Result<ProfilePictureUrl> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result<ProfilePictureUrl>.Failure(Error.InvalidProfilePictureUrlEmpty);

        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
            return Result<ProfilePictureUrl>.Failure(Error.InvalidProfilePictureUrlOther);

        if ((uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return Result<ProfilePictureUrl>.Failure(Error.OnlyHttpOrHttpsAllowed);

        return Result<ProfilePictureUrl>.Success(new ProfilePictureUrl(uri));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}