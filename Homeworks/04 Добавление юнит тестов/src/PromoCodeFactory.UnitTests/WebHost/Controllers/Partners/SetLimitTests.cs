using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Soenneker.Utils.AutoBogus;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _partnersController;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _partnersController = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();
        var partner = CreatePartner(partnerId, false);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var objectResult = (UnprocessableEntityObjectResult)result.Result!;
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Partner blocked");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>()
            .RuleFor(r => r.Limit, 100)
            .Generate();
        var partner = CreatePartner(partnerId, true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result.Result!;
        createdResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();

        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.Is<PartnerPromoCodeLimit>(l => l.Limit == request.Limit && l.Partner.Id == partnerId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var activeLimitId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();
        var partner = CreatePartner(partnerId, true);
        var activeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, activeLimitId)
            .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
            .RuleFor(l => l.Partner, partner)
            .Generate();
        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        activeLimit.CanceledAt.Should().NotBeNull();

        _partnersRepositoryMock.Verify(r => r.Update(partner, It.IsAny<CancellationToken>()), Times.Once);
        _partnerLimitsRepositoryMock.Verify(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();
        var partner = CreatePartner(partnerId, true);
        var activeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
            .RuleFor(l => l.Partner, partner)
            .Generate();
        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        // Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private static Partner CreatePartner(Guid partnerId, bool isActive)
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
            .RuleFor(p => p.IsActive, isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>())
            .Generate();
    }
}
