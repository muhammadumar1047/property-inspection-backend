using PropertyInspection.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Shared.DTOs
{
    public class QuickSuggestionResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public QuickSuggestionType Type { get; set; }
        public string Text { get; set; } = null!;
        public string? Shortcut { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateQuickSuggestionRequest
    {
        public Guid? AgencyId { get; set; }

        [Required]
        public QuickSuggestionType Type { get; set; }

        [Required, StringLength(1000)]
        public string Text { get; set; } = null!;

        [StringLength(50)]
        public string? Shortcut { get; set; }
    }

    public class UpdateQuickSuggestionRequest
    {
        public Guid? AgencyId { get; set; }

        [Required, StringLength(1000)]
        public string Text { get; set; } = null!;

        [StringLength(50)]
        public string? Shortcut { get; set; }

        public bool IsActive { get; set; }
    }

    public class QuickSuggestionSettingsResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public bool IsEntryExitEnabled { get; set; }
        public bool IsRoutineEnabled { get; set; }
        public bool CombineDictionaries { get; set; }
    }

    public class UpdateQuickSuggestionSettingsRequest
    {
        public Guid? AgencyId { get; set; }
        public bool IsEntryExitEnabled { get; set; }
        public bool IsRoutineEnabled { get; set; }
        public bool CombineDictionaries { get; set; }
    }

    public class QuickSuggestionImportRow
    {
        public int RowNumber { get; set; }
        public string Text { get; set; } = null!;
        public string? Shortcut { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }

    public class ImportPreviewResult
    {
        public int TotalRecords { get; set; }
        public int ValidCount { get; set; }
        public int InvalidCount { get; set; }
        public List<QuickSuggestionImportRow> Rows { get; set; } = new();
    }

    public class CommitImportRequest
    {
        public Guid? AgencyId { get; set; }
        [Required]
        public QuickSuggestionType Type { get; set; }
        [Required]
        public List<CreateQuickSuggestionRequest> Records { get; set; } = new();
    }
}
