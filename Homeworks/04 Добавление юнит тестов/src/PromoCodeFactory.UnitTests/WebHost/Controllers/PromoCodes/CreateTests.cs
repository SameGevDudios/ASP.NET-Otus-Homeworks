using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Soenneker.Utils.AutoBogus;
using System.Linq.Expressions;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PromoCodesController _promoCodesController;

    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();

        _promoCodesController = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            _customerPromoCodesRepositoryMock.Object,
            _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.PartnerId, partnerId)
            .Generate();
        var partner = CreatePartner(partnerId);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        // Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Preference not found");
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var request = new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.PartnerId, partnerId)
            .RuleFor(r => r.PreferenceId, preferenceId)
            .Generate();

        var partner = CreatePartner(partnerId);
        var preference = new AutoFaker<Preference>()
            .RuleFor(p => p.Id, preferenceId)
            .RuleFor(p => p.Customers, new List<Customer>())
            .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        // Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("No active limit");
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var request = new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.PartnerId, partnerId)
            .RuleFor(r => r.PreferenceId, preferenceId)
            .Generate();

        var partner = CreatePartner(partnerId);
        var limit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
            .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(10))
            .RuleFor(l => l.Limit, 5)
            .RuleFor(l => l.IssuedCount, 5)
            .RuleFor(l => l.Partner, partner)
            .Generate();
        partner.PartnerLimits.Add(limit);

        var preference = new AutoFaker<Preference>()
            .RuleFor(p => p.Id, preferenceId)
            .RuleFor(p => p.Customers, new List<Customer>())
            .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        // Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Limit exceeded");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var request = new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.PartnerId, partnerId)
            .RuleFor(r => r.PreferenceId, preferenceId)
            .Generate();

        var partner = CreatePartner(partnerId);
        var initialIssuedCount = 2;
        var limit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
            .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(10))
            .RuleFor(l => l.Limit, 5)
            .RuleFor(l => l.IssuedCount, initialIssuedCount)
            .RuleFor(l => l.Partner, partner)
            .Generate();
        partner.PartnerLimits.Add(limit);

        var preference = new AutoFaker<Preference>()
            .RuleFor(p => p.Id, preferenceId)
            .RuleFor(p => p.Customers, new List<Customer>())
            .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        _promoCodesRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        limit.IssuedCount.Should().Be(initialIssuedCount + 1);

        _promoCodesRepositoryMock.Verify(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()), Times.Once);
        _partnersRepositoryMock.Verify(r => r.Update(partner, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Partner CreatePartner(Guid partnerId)
    {
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, Guid.NewGuid())
            .Generate();

        var employee = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, Guid.NewGuid())
            .RuleFor(e => e.Role, role)
            .Generate();

        return new AutoFaker<Partner>()
            .RuleFor(p => p.Id, partnerId)
            .RuleFor(p => p.IsActive, true)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>())
            .Generate();
    }
}
