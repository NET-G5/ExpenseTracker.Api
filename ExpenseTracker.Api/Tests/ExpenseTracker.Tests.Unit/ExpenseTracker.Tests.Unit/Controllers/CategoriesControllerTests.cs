using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using ExpenseTracker.Api.Controllers;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseTracker.Tests.Unit.Controllers;

public class CategoriesControllerTests : ControllerTestsBase
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<ITransferService> _mockTransferService;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mockTransferService = new Mock<ITransferService>();
        _controller = new CategoriesController(_mockCategoryService.Object, _mockTransferService.Object);
    }

    [Fact]
    public async Task GetAsync_ShouldCallServiceGetAsync_Once()
    {
        // Arrange
        var queryParamters = _fixture.Create<CategoryQueryParameters>();

        _mockCategoryService.Setup(x => x.GetAsync(It.IsAny<CategoryQueryParameters>()));

        // Act
        _ = await _controller.GetAsync(queryParamters);

        // Assert
        _mockCategoryService.Verify(service => service.GetAsync(It.IsAny<CategoryQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldPassQueryParameters_ToService()
    {
        // Arrange
        var queryParamters = _fixture.Create<CategoryQueryParameters>();

        _mockCategoryService.Setup(x => x.GetAsync(queryParamters));

        // Act
        _ = await _controller.GetAsync(queryParamters);

        // Assert
        _mockCategoryService.Verify(service => service.GetAsync(queryParamters), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCategories_WhenServiceReturnsCategories()
    {
        // Arrange
        var queryParamters = _fixture.Create<CategoryQueryParameters>();
        var expectedCategories = _fixture.CreateMany<CategoryDto>(10).ToList();

        _mockCategoryService.Setup(x => x.GetAsync(queryParamters))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _controller.GetAsync(queryParamters);
        var okResult = result.Result as OkObjectResult;
        var actual = okResult?.Value;

        // Assert
        Assert.Equivalent(expectedCategories, actual);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEmpty_WhenServiceReturnsEmpty()
    {
        // Arrange
        var queryParamters = _fixture.Create<CategoryQueryParameters>();
        var expectedCategories = _fixture.CreateMany<CategoryDto>(0).ToList();

        _mockCategoryService.Setup(x => x.GetAsync(queryParamters))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _controller.GetAsync(queryParamters);
        var okResult = result.Result as OkObjectResult;
        var actual = okResult?.Value;

        // Assert
        Assert.Equivalent(expectedCategories, actual);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var queryParamters = _fixture.Create<CategoryQueryParameters>();
        var exception = _fixture.Create<Exception>();

        _mockCategoryService.Setup(x => x.GetAsync(queryParamters))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAsync(queryParamters));
    }

    [Fact]
    public async Task PutAsync_ShouldReturnBadRequest_WhenRouteIdDoesNotMatchWithBodyId()
    {
        // Arrange
        var routeId = _fixture.Create<int>();
        var request = _fixture.Build<UpdateCategoryRequest>()
            .With(x => x.Id, routeId + 1)
            .Create();
        var expectedMessage = $"Route parameter ID: {routeId} does not match with body parameter: {request.Id}";

        // Act
        var result = await _controller.PutAsync(routeId, request);
        var badResult = result as BadRequestObjectResult;
        var actual = badResult?.Value;

        // Assert
        Assert.Equal(expectedMessage, actual);
    }
}
