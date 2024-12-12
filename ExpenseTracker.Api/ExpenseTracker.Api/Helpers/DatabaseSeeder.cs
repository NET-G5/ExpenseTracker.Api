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
        // CreateWallets(context); 
        // CreateCategories(context);
        CreateTransfers(context);
    }

    private static void CreateCategories(IApplicationDbContext context)
    {
        if (context.Categories.Any()) return;

        var user = context.Users.FirstOrDefault();
        if (user is null)
        {
            return;
        }

        for (int i = 0; i < 25; i++)
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

    private static void CreateWallets(IApplicationDbContext context)
    {
        if (context.Wallets.Any()) return;

        var user = context.Users.FirstOrDefault();
        if (user is null)
        {
            return;
        }

        for (int i = 0; i < 25; i++)
        {
            var wallet = new Wallet
            {
                Name = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                Balance = _faker.Finance.Amount(min: 1000, max: 100000),
                Owner = user,
                OwnerId = user.Id,
                CreatedBy = user.UserName!
            };

            context.Wallets.Add(wallet);
        }

        context.SaveChanges();
    }

    private static void CreateTransfers(IApplicationDbContext context)
    {
        if (context.Transfers.Any())
        {
            return;
        }

        var user = context.Users.FirstOrDefault(x => x.Id == new Guid("4784c320-ad4b-45bd-611b-08dd176e0e67"));
        var wallet = context.Wallets.FirstOrDefault(x => x.OwnerId == new Guid("4784c320-ad4b-45bd-611b-08dd176e0e67"));
        if (user is null)
        {
            return;
        }

        var categoryIds = context.Categories
            .Where(x => x.OwnerId == user.Id)
            .Select(x => x.Id)
            .ToList();

        foreach (var categoryId in categoryIds)
        {
            var numberOfTransfers = _faker.Random.Int(100, 150);
            for (int i = 0; i < numberOfTransfers; i++)
            {
                var transfer = new Transfer

                {
                    Title = _faker.Commerce.ProductName(),
                    Amount = _faker.Random.Decimal(10, 5_000),
                    Date = _faker.Date.Between(DateTime.Now.AddMonths(-7), DateTime.Now).ToUniversalTime(),
                    Notes = _faker.Lorem.Sentence(),
                    CategoryId = categoryId,
                    Category = null,
                    Wallet = wallet!,
                    CreatedBy = user.UserName!
                };

                context.Transfers.Add(transfer);
            }
        }

        context.SaveChanges();
    }
}
