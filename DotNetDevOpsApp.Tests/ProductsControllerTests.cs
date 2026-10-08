using DotNetDevOpsApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DotNetDevOpsApp.Tests;

public class ProductsControllerTests
{
    [Fact]
    public void GetProducts_ReturnsOk()
    {
        var controller = new ProductsController();

        var result = controller.GetProducts();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetProduct_InvalidId_ReturnsNotFound()
    {
        var controller = new ProductsController();

        var result = controller.GetProduct(99999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void GetProduct_ValidId_ReturnsOk()
    {
        var controller = new ProductsController();

        var result = controller.GetProduct(1);

        Assert.IsType<OkObjectResult>(result);
    }
}
