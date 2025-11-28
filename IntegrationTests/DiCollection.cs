namespace IntegrationTests;

using Xunit;

[CollectionDefinition("DI")]
public class DiCollection : ICollectionFixture<ServiceProviderFixture> { }