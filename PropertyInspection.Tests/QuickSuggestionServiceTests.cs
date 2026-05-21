using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PropertyInspection.Application.IServices;
using PropertyInspection.Application.Mapping;
using PropertyInspection.Application.Services;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PropertyInspection.Tests
{
    public class QuickSuggestionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITenantAgencyResolver> _mockTenantResolver;
        private readonly IMapper _mapper;
        private readonly QuickSuggestionService _service;

        private readonly Guid _agencyId = Guid.NewGuid();

        public QuickSuggestionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTenantResolver = new Mock<ITenantAgencyResolver>();

            // Use the real AutoMapper mapping configuration to verify profiles are correct
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _mockTenantResolver.Setup(r => r.ResolveAgencyId(It.IsAny<Guid?>())).Returns(_agencyId);

            _service = new QuickSuggestionService(
                _mockUnitOfWork.Object,
                _mockTenantResolver.Object,
                _mapper
            );
        }

        [Fact]
        public async Task GetSuggestionsAsync_ShouldReturnPagedSuggestions_WhenValidRequest()
        {
            // Arrange
            var type = QuickSuggestionType.Routine;
            var suggestionsList = new List<QuickSuggestion>
            {
                new QuickSuggestion { Id = Guid.NewGuid(), AgencyId = _agencyId, Type = type, Text = "Phrase A", Shortcut = "PA" },
                new QuickSuggestion { Id = Guid.NewGuid(), AgencyId = _agencyId, Type = type, Text = "Phrase B", Shortcut = "PB" }
            };

            var mockRepo = new Mock<IGenericRepository<QuickSuggestion>>();
            mockRepo.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<QuickSuggestion, bool>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IQueryable<QuickSuggestion>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IOrderedQueryable<QuickSuggestion>>>()
            )).ReturnsAsync((suggestionsList, suggestionsList.Count));

            _mockUnitOfWork.Setup(u => u.QuickSuggestions).Returns(mockRepo.Object);

            // Act
            var result = await _service.GetSuggestionsAsync(type, null, "textasc", 1, 10);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TotalCount.Should().Be(2);
            result.Data!.Data.Should().HaveCount(2);
            result.Data!.Data[0].Text.Should().Be("Phrase A");
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSuggestion_WhenUniqueAndValid()
        {
            // Arrange
            var request = new CreateQuickSuggestionRequest
            {
                Type = QuickSuggestionType.EntryExit,
                Text = "Unique entry phrase",
                Shortcut = "UEP"
            };

            var mockRepo = new Mock<IGenericRepository<QuickSuggestion>>();
            // Setup text duplicate check to return null (no duplicate)
            mockRepo.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<QuickSuggestion, bool>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IQueryable<QuickSuggestion>>>()
            )).ReturnsAsync((QuickSuggestion?)null);

            _mockUnitOfWork.Setup(u => u.QuickSuggestions).Returns(mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Text.Should().Be("Unique entry phrase");
            result.Data!.Shortcut.Should().Be("UEP");
            mockRepo.Verify(r => r.AddAsync(It.IsAny<QuickSuggestion>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnConflict_WhenTextDuplicateExists()
        {
            // Arrange
            var request = new CreateQuickSuggestionRequest
            {
                Type = QuickSuggestionType.EntryExit,
                Text = "Existing phrase",
                Shortcut = "EP"
            };

            var existingSuggestion = new QuickSuggestion
            {
                Id = Guid.NewGuid(),
                AgencyId = _agencyId,
                Type = QuickSuggestionType.EntryExit,
                Text = "Existing phrase",
                Shortcut = "OTHER"
            };

            var mockRepo = new Mock<IGenericRepository<QuickSuggestion>>();
            mockRepo.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<QuickSuggestion, bool>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IQueryable<QuickSuggestion>>>()
            )).ReturnsAsync(existingSuggestion);

            _mockUnitOfWork.Setup(u => u.QuickSuggestions).Returns(mockRepo.Object);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ServiceErrorCodes.Conflict);
            result.Message.Should().Contain("already exists");
            mockRepo.Verify(r => r.AddAsync(It.IsAny<QuickSuggestion>()), Times.Never);
        }

        [Fact]
        public async Task PreviewImportAsync_ShouldValidateRowsCorrectly()
        {
            // Arrange
            var csvContent = "Text,Shortcut\r\nClean filter,CF\r\nExisting text,ET\r\nPhrase with duplicate shortcut,CF\r\n";
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            using var stream = new MemoryStream(bytes);

            var existingSuggestions = new List<QuickSuggestion>
            {
                new QuickSuggestion { Id = Guid.NewGuid(), AgencyId = _agencyId, Type = QuickSuggestionType.Routine, Text = "Existing text", Shortcut = "ET" }
            };

            var mockRepo = new Mock<IGenericRepository<QuickSuggestion>>();
            mockRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<QuickSuggestion, bool>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IQueryable<QuickSuggestion>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IOrderedQueryable<QuickSuggestion>>>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync(existingSuggestions);

            _mockUnitOfWork.Setup(u => u.QuickSuggestions).Returns(mockRepo.Object);

            // Act
            var result = await _service.PreviewImportAsync(QuickSuggestionType.Routine, stream, "test.csv");

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TotalRecords.Should().Be(3);

            // Row 1: Valid
            result.Data.Rows[0].IsValid.Should().BeTrue();
            result.Data.Rows[0].Text.Should().Be("Clean filter");

            // Row 2: Invalid (Exists in Database)
            result.Data.Rows[1].IsValid.Should().BeFalse();
            result.Data.Rows[1].ValidationErrors.Should().Contain("Suggestion phrase already exists in this library.");

            // Row 3: Invalid (Duplicate Shortcut in File)
            result.Data.Rows[2].IsValid.Should().BeFalse();
            result.Data.Rows[2].ValidationErrors.Should().Contain("Duplicate shortcut found in this import file.");
        }

        [Fact]
        public async Task ExportToCsvAsync_ShouldReturnCsvBytes()
        {
            // Arrange
            var type = QuickSuggestionType.Routine;
            var suggestionsList = new List<QuickSuggestion>
            {
                new QuickSuggestion { Id = Guid.NewGuid(), AgencyId = _agencyId, Type = type, Text = "Clean filter, HVAC", Shortcut = "CF" },
                new QuickSuggestion { Id = Guid.NewGuid(), AgencyId = _agencyId, Type = type, Text = "Standard Suggestion", Shortcut = "SS" }
            };

            var mockRepo = new Mock<IGenericRepository<QuickSuggestion>>();
            mockRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<QuickSuggestion, bool>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IQueryable<QuickSuggestion>>>(),
                It.IsAny<Func<IQueryable<QuickSuggestion>, IOrderedQueryable<QuickSuggestion>>>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync(suggestionsList);

            _mockUnitOfWork.Setup(u => u.QuickSuggestions).Returns(mockRepo.Object);

            // Act
            var result = await _service.ExportToCsvAsync(type);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            var csvContent = Encoding.UTF8.GetString(result.Data!);
            csvContent.Should().Contain("Text,Shortcut");
            csvContent.Should().Contain("\"Clean filter, HVAC\",CF");
            csvContent.Should().Contain("Standard Suggestion,SS");
        }
    }
}
