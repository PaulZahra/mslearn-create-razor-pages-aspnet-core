using System.Linq;
using ContosoPizza.Data;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Tests;

public class PizzaServiceTests
{
    [Fact]
    public void GetPizzas_ReturnsAllSeededEntries()
    {
        using var context = CreateContext(
            new Pizza { Name = "Margherita", Size = PizzaSize.Medium, Price = 12.50m },
            new Pizza { Name = "Pepperoni", Size = PizzaSize.Large, Price = 14.00m }
        );
        var service = new PizzaService(context);

        var pizzas = service.GetPizzas();

        Assert.Equal(2, pizzas.Count);
        Assert.Contains(pizzas, pizza => pizza.Name == "Margherita");
        Assert.Contains(pizzas, pizza => pizza.Size == PizzaSize.Large && pizza.Name == "Pepperoni");
    }

    [Fact]
    public void AddPizza_PersistsPizzaToContext()
    {
        using var context = CreateContext();
        var service = new PizzaService(context);
        var pizza = new Pizza
        {
            Name = "Garden Special",
            Size = PizzaSize.Small,
            Price = 9.25m,
            IsGlutenFree = true
        };

        service.AddPizza(pizza);

        var storedPizza = context.Pizzas!.Single();
        Assert.Equal("Garden Special", storedPizza.Name);
        Assert.True(storedPizza.IsGlutenFree);
        Assert.NotEqual(0, storedPizza.Id);
    }

    [Fact]
    public void DeletePizza_RemovesMatchingEntity()
    {
        using var context = CreateContext(new Pizza { Name = "BBQ Chicken", Size = PizzaSize.Medium, Price = 15.75m });
        var pizzaId = context.Pizzas!.Single().Id;
        var service = new PizzaService(context);

        service.DeletePizza(pizzaId);

        Assert.Empty(context.Pizzas!);
    }

    private static PizzaContext CreateContext(params Pizza[] pizzas)
    {
        var options = new DbContextOptionsBuilder<PizzaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new PizzaContext(options);

        if (pizzas.Length > 0)
        {
            context.Pizzas!.AddRange(pizzas);
            context.SaveChanges();
        }

        return context;
    }

private void Pizza_IsGlutenFree_DefaultIsFalse()
    {
        // Arrange & Act
        var pizza = new Pizza();

        // Assert
        Assert.False(pizza.IsGlutenFree);
    }

    [Fact]
    private void Pizza_IsGlutenFree_CanBeSetToTrue()
    {
        // Arrange
        var pizza = new Pizza { IsGlutenFree = true };

        // Act & Assert
        Assert.True(pizza.IsGlutenFree);
    }

    [Fact]
    private void Pizza_IsGlutenFree_CanBeSetToFalse()
    {
        // Arrange
        var pizza = new Pizza { IsGlutenFree = false };

        // Act & Assert
        Assert.False(pizza.IsGlutenFree);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Pizza_IsGlutenFree_AcceptsValidValues(bool isGlutenFree)
    {
        // Arrange & Act
        var pizza = new Pizza { IsGlutenFree = isGlutenFree };

        // Assert
        Assert.Equal(isGlutenFree, pizza.IsGlutenFree);
    }


}
