namespace Pantry.Module.Recipe.IntegrationTests.Infrastructure;

[CollectionDefinition(nameof(RecipeTestCollection))]
public class RecipeTestCollection: ICollectionFixture<RecipeIntegrationTestWebAppFactory> { }