using Bogus;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Api.Helpers;

public static class DatabaseSeeder
{
    private static readonly Faker _faker = new();

    public static void SeedDatabase(IApplicationDbContext context)
    {
        CreateCategories(context);
    }

    private static void CreateCategories(IApplicationDbContext context)
    {
        if (context.Categories.Any()) return;
        
        var user = context.Users.FirstOrDefault();
        if (user is null)
        {
            return;
        }

        for (int i =0; i < 25; i++)
        {
            var category = new Category
            {
                Name = _faker.Commerce.Categories(1)[0],
                Description = _faker.Commerce.ProductDescription(),
                Type = _faker.Random.Bool() ? CategoryType.Expense : CategoryType.Income,
                Owner = user,
                OwnerId = user.Id,
                CreatedBy = user.UserName!,
            };

            context.Categories.Add(category);
        }

        context.SaveChanges();
    }
}
