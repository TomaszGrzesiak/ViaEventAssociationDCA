namespace ViaEventAssociation.Core.Domain.Contracts;

public interface IProfanityChecker
{
    // make use of the API that it returns what word has been classified as profanity
    Task<string> ContainsProfanity(string text);
}