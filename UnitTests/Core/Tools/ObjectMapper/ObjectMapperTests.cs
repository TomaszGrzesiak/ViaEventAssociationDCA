using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using Xunit;

namespace UnitTests.Core.Tools.ObjectMapper;

public class ObjectMapperTests
{
    private static IObjectMapper CreateMapperWith(params object[] configs)
    {
        var services = new ServiceCollection();
        services.AddObjectMapper();

        foreach (var config in configs)
        {
            var configType = config.GetType();

            // Register it for all IMappingConfig<,> interfaces it implements
            var mappingInterfaces = configType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMappingConfig<,>));

            foreach (var iface in mappingInterfaces)
            {
                services.AddSingleton(iface, config);
            }

            // Optional: also register as itself (not strictly needed)
            services.AddSingleton(configType, config);
        }

        return services.BuildServiceProvider().GetRequiredService<IObjectMapper>();
    }

    private sealed record Person(string FirstName, string LastName);
    private sealed record PersonsFullName(string FullName);

    private sealed class PersonsFullNameConfig : IMappingConfig<Person, PersonsFullName>
    {
        public PersonsFullName Map(Person person) => new($"{person.FirstName} {person.LastName}");
    }

    [Fact]
    public void Map_UsesSpecificConfig_WhenRegistered()
    {
        // arrange
        var mapper = CreateMapperWith(new PersonsFullNameConfig());
        var input = new Person("John", "Doe");

        // act
        var result = mapper.Map<PersonsFullName>(input);

        // assert
        Assert.Equal("John Doe", result.FullName);
    }

    private sealed record SameShape(string FirstName, string LastName);

    [Fact]
    public void Map_UsesJsonFallback_WhenNoConfigRegistered()
    {
        // arrange
        var mapper = CreateMapperWith(); // (no mapping in the mapper, still we get some result coz mapper tries to do the JSON stuff)
        var input = new SameShape("Jane", "Doe");

        // act
        var result = mapper.Map<SameShape>(input);

        // assert
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }
}