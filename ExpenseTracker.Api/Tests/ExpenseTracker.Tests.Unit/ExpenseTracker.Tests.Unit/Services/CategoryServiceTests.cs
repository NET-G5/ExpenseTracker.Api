using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;

namespace ExpenseTracker.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly CategoryService _service;
    private readonly Fixture _fixture;
    private readonly static Guid _ownerId = Guid.NewGuid();

    public CategoryServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryDto>();
            cfg.CreateMap<CreateCategoryRequest, Category>();
        });
        _mapper = config.CreateMapper();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _service = new CategoryService(_mapper, _mockDbContext.Object, _mockCurrentUserService.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _mockCurrentUserService.Setup(x => x.GetUserId())
            .Returns(_ownerId);
    }

    public static readonly TheoryData<CategoryQueryParameters> FilterQueryParameters = new()
    {
        new CategoryQueryParameters("", null),
        new CategoryQueryParameters(null, null),
        new CategoryQueryParameters("", null),
        new CategoryQueryParameters("   ", null),
        new CategoryQueryParameters("men", null),
        new CategoryQueryParameters("ice", null),
        new CategoryQueryParameters(null, CategoryType.Income),
        new CategoryQueryParameters(null, CategoryType.Expense),
        new CategoryQueryParameters("ice", CategoryType.Income),
        new CategoryQueryParameters("men", CategoryType.Expense),
    };

    public static readonly TheoryData<CategoryQueryParameters> SortQueryParameters = new()
    {
        new CategoryQueryParameters(null, null, null), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, ""), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, " "), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, "   "), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, "price_asc"), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, "price_desc"), // invalid case - should use default sorting
        new CategoryQueryParameters(null, null, "name_asc"),
        new CategoryQueryParameters(null, null, "name_desc"),
        new CategoryQueryParameters(null, null, "description_asc"),
        new CategoryQueryParameters(null, null, "description_desc"),
    };

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentNullException_WhenQueryParametersIsNull()
    {
        // Arrange
        var expectedException = new ArgumentNullException("queryParameters");
        CategoryQueryParameters queryParameters = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.GetAsync(queryParameters));
    }

    [Theory, MemberData(nameof(FilterQueryParameters))]
    public async Task GetAsync_ShouldFilterCategories(CategoryQueryParameters queryParameters)
    {
        // Arrange
        var categories = GetTestCategories();
        _mockDbContext.Setup(context => context.Categories)
            .ReturnsDbSet(categories);
        var expected = GetExpectedCategories(queryParameters);

        // Act
        var actual = await _service.GetAsync(queryParameters);

        // Assert
        Assert.NotEmpty(actual);

        foreach(var expectedCategory in expected)
        {
            var actualCategory = actual.FirstOrDefault(x => x.Id == expectedCategory.Id);
            
            Assert.NotNull(actualCategory);
            Assert.Equal(expectedCategory.Name, actualCategory.Name);
            Assert.Equal(expectedCategory.Description, actualCategory.Description);
        }
    }

    [Theory, MemberData(nameof(SortQueryParameters))]
    public async Task GetAsync_ShouldSortCategories(CategoryQueryParameters queryParameters)
    {
        // Arrange
        var categories = GetTestCategories();
        _mockDbContext.Setup(context => context.Categories)
            .ReturnsDbSet(categories);
        var expected = SortCategories(queryParameters.SortBy);

        // Act
        var actual = await _service.GetAsync(queryParameters);

        // Assert
        Assert.NotEmpty(actual);
        Assert.Equal(expected.Count, actual.Count);

        for(int i = 0; i < expected.Count; i++)
        {
            var expectedCategory = expected[i];
            var actualCategory = actual[i];

            Assert.Equal(expectedCategory.Id, actualCategory.Id);
            // Optional
            Assert.Equal(expectedCategory.Name, actualCategory.Name);
            Assert.Equal(expectedCategory.Description, actualCategory.Description);
            Assert.Equal(expectedCategory.Type, actualCategory.Type);
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_ShouldUpdateOwnerIdOfEntityFromCurrentUserId()
    {
        // Arrange
        var request = _fixture.Create<CreateCategoryRequest>();
        var expectedEntity = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            OwnerId = _ownerId,
            Owner = null!,
            CreatedBy = null!,
        };

        _mockDbContext.Setup(context => context.Categories.Add(It.IsAny<Category>()));
        _mockDbContext.Setup(context => context.SaveChangesAsync(CancellationToken.None));

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        _mockDbContext.Verify(
            context => context.Categories.Add(It.Is<Category>(cat => cat.OwnerId == _ownerId)), 
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCurrentUserService_Once()
    {
        // Arrange
        var request = _fixture.Create<CreateCategoryRequest>();

        _mockDbContext.Setup(context => context.Categories.Add(It.IsAny<Category>()));
        _mockDbContext.Setup(context => context.SaveChangesAsync(CancellationToken.None));

        // Act
        _ = await _service.CreateAsync(request);
        
        // Assert
        _mockCurrentUserService.Verify(service => service.GetUserId(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallAdd_Once()
    {
        // Arrange
        var request = _fixture.Create<CreateCategoryRequest>();

        _mockDbContext.Setup(context => context.Categories.Add(It.IsAny<Category>()));
        _mockDbContext.Setup(context => context.SaveChangesAsync(CancellationToken.None));

        // Act
        _ = await _service.CreateAsync(request);

        // Assert
        _mockDbContext.Verify(context => context.Categories.Add(It.IsAny<Category>()), Times.Once);
        _mockDbContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    private static List<Category> GetTestCategories() =>
        [
            new Category 
            { 
                Id = 1,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Electronics", 
                Description = "Gadgets and devices", 
                Type = CategoryType.Income, 
                OwnerId = _ownerId
            },
            new Category 
            {
                Id = 2,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Furniture", 
                Description = "Home and office furniture", 
                Type = CategoryType.Income, 
                OwnerId = _ownerId
            },
            new Category 
            {
                Id = 3,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Clothing", 
                Description = "Men's and women's wear", 
                Type = CategoryType.Income, 
                OwnerId = _ownerId},
            new Category 
            {
                Id = 4,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Books", 
                Description = "Educational and entertainment books", 
                Type = CategoryType.Expense, 
                OwnerId = _ownerId
            },
            new Category 
            {
                Id = 5,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Sports", 
                Description = "Sports equipment and apparel", 
                Type = CategoryType.Expense, 
                OwnerId = _ownerId
            },
            new Category 
            {
                Id = 6,
                CreatedBy = "John",
                Owner = new IdentityUser<Guid>(),
                Name = "Food & Beverages", 
                Description = "Groceries and drinks", 
                Type = CategoryType.Expense, 
                OwnerId = _ownerId
            }
        ];

    private static List<Category> GetExpectedCategories(CategoryQueryParameters queryParameters)
    {
        var query = GetTestCategories().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(queryParameters.Search))
        {
            query = query.Where(x => x.Name.Contains(queryParameters.Search) 
                || (x.Description != null && x.Description.Contains(queryParameters.Search)));
        }

        if (queryParameters.Type.HasValue)
        {
            query = query.Where(x => x.Type == queryParameters.Type.Value);
        }

        return query.ToList();
    }

    private static List<Category> SortCategories(string? sortBy)
    {
        var query = GetTestCategories().AsEnumerable();
        var result = sortBy switch
        {
            "name_desc" => query.OrderByDescending(x => x.Name),
            "description_asc" => query.OrderBy(x => x.Description),
            "description_desc" => query.OrderByDescending(x => x.Description),
            _ => query.OrderBy(x => x.Name)
        };

        return [.. result];
    }
}
