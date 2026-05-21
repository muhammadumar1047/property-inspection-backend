using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class QuickSuggestionService : IQuickSuggestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public QuickSuggestionService(
            IUnitOfWork unitOfWork,
            ITenantAgencyResolver tenantAgencyResolver,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedResult<QuickSuggestionResponse>>> GetSuggestionsAsync(
            QuickSuggestionType type, string? search, string? sortBy, int page, int pageSize, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                // Build ordering query
                Func<IQueryable<QuickSuggestion>, IOrderedQueryable<QuickSuggestion>> orderBy = q => q.OrderBy(s => s.Text);
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    switch (sortBy.ToLowerInvariant())
                    {
                        case "textdesc":
                        case "z-a":
                            orderBy = q => q.OrderByDescending(s => s.Text);
                            break;
                        case "textasc":
                        case "a-z":
                            orderBy = q => q.OrderBy(s => s.Text);
                            break;
                        case "shortcutasc":
                            orderBy = q => q.OrderBy(s => s.Shortcut);
                            break;
                        case "shortcutdesc":
                            orderBy = q => q.OrderByDescending(s => s.Shortcut);
                            break;
                        case "createdatdesc":
                            orderBy = q => q.OrderByDescending(s => s.CreatedAt);
                            break;
                        case "createdatasc":
                            orderBy = q => q.OrderBy(s => s.CreatedAt);
                            break;
                    }
                }

                var (suggestions, totalCount) = await _unitOfWork.QuickSuggestions.GetPagedAsync(
                    pageNumber: page,
                    pageSize: pageSize,
                    predicate: s => s.AgencyId == resolvedAgencyId &&
                                    s.Type == type &&
                                    !s.IsDeleted &&
                                    (string.IsNullOrWhiteSpace(search) || 
                                     s.Text.Contains(search) || 
                                     (s.Shortcut != null && s.Shortcut.Contains(search))),
                    orderBy: orderBy
                );

                var responseDtos = _mapper.Map<List<QuickSuggestionResponse>>(suggestions);

                return new ServiceResponse<PagedResult<QuickSuggestionResponse>>
                {
                    Success = true,
                    Message = "Suggestions retrieved successfully",
                    Data = new PagedResult<QuickSuggestionResponse>
                    {
                        Data = responseDtos,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<QuickSuggestionResponse>>
                {
                    Success = false,
                    Message = $"Error retrieving suggestions: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<QuickSuggestionResponse>> GetByIdAsync(Guid id, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var suggestion = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                    s => s.Id == id && s.AgencyId == resolvedAgencyId && !s.IsDeleted);

                if (suggestion == null)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var response = _mapper.Map<QuickSuggestionResponse>(suggestion);
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = true,
                    Message = "Suggestion retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = false,
                    Message = $"Error retrieving suggestion: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<QuickSuggestionResponse>> CreateAsync(CreateQuickSuggestionRequest request)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);

                // Trim text & shortcut
                var text = request.Text?.Trim();
                var shortcut = request.Shortcut?.Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase cannot be empty",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                if (text.Length > 1000)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase exceeds 1000 characters limit",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                if (shortcut != null && shortcut.Length > 50)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Shortcut exceeds 50 characters limit",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                // Check text uniqueness
                var duplicateText = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                    s => s.AgencyId == resolvedAgencyId && 
                         s.Type == request.Type && 
                         s.Text.ToLower() == text.ToLower() && 
                         !s.IsDeleted);

                if (duplicateText != null)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase already exists in this library",
                        ErrorCode = ServiceErrorCodes.Conflict
                    };
                }

                // Check shortcut uniqueness
                if (!string.IsNullOrEmpty(shortcut))
                {
                    var duplicateShortcut = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                        s => s.AgencyId == resolvedAgencyId && 
                             s.Type == request.Type && 
                             s.Shortcut != null && 
                             s.Shortcut.ToLower() == shortcut.ToLower() && 
                             !s.IsDeleted);

                    if (duplicateShortcut != null)
                    {
                        return new ServiceResponse<QuickSuggestionResponse>
                        {
                            Success = false,
                            Message = $"Shortcut '{shortcut}' is already assigned to another suggestion",
                            ErrorCode = ServiceErrorCodes.Conflict
                        };
                    }
                }

                var entity = new QuickSuggestion
                {
                    AgencyId = resolvedAgencyId,
                    Type = request.Type,
                    Text = text,
                    Shortcut = string.IsNullOrWhiteSpace(shortcut) ? null : shortcut,
                    IsActive = true
                };

                await _unitOfWork.QuickSuggestions.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<QuickSuggestionResponse>(entity);
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = true,
                    Message = "Suggestion created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = false,
                    Message = $"Error creating suggestion: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<QuickSuggestionResponse>> UpdateAsync(Guid id, UpdateQuickSuggestionRequest request)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);

                var suggestion = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                    s => s.Id == id && s.AgencyId == resolvedAgencyId && !s.IsDeleted);

                if (suggestion == null)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var text = request.Text?.Trim();
                var shortcut = request.Shortcut?.Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase cannot be empty",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                if (text.Length > 1000)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase exceeds 1000 characters limit",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                if (shortcut != null && shortcut.Length > 50)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Shortcut exceeds 50 characters limit",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                // Check text uniqueness excluding self
                var duplicateText = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                    s => s.Id != id &&
                         s.AgencyId == resolvedAgencyId && 
                         s.Type == suggestion.Type && 
                         s.Text.ToLower() == text.ToLower() && 
                         !s.IsDeleted);

                if (duplicateText != null)
                {
                    return new ServiceResponse<QuickSuggestionResponse>
                    {
                        Success = false,
                        Message = "Suggestion phrase already exists in this library",
                        ErrorCode = ServiceErrorCodes.Conflict
                    };
                }

                // Check shortcut uniqueness excluding self
                if (!string.IsNullOrEmpty(shortcut))
                {
                    var duplicateShortcut = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                        s => s.Id != id &&
                             s.AgencyId == resolvedAgencyId && 
                             s.Type == suggestion.Type && 
                             s.Shortcut != null && 
                             s.Shortcut.ToLower() == shortcut.ToLower() && 
                             !s.IsDeleted);

                    if (duplicateShortcut != null)
                    {
                        return new ServiceResponse<QuickSuggestionResponse>
                        {
                            Success = false,
                            Message = $"Shortcut '{shortcut}' is already assigned to another suggestion",
                            ErrorCode = ServiceErrorCodes.Conflict
                        };
                    }
                }

                suggestion.Text = text;
                suggestion.Shortcut = string.IsNullOrWhiteSpace(shortcut) ? null : shortcut;
                suggestion.IsActive = request.IsActive;

                await _unitOfWork.QuickSuggestions.UpdateAsync(suggestion);
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<QuickSuggestionResponse>(suggestion);
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = true,
                    Message = "Suggestion updated successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<QuickSuggestionResponse>
                {
                    Success = false,
                    Message = $"Error updating suggestion: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var suggestion = await _unitOfWork.QuickSuggestions.FirstOrDefaultAsync(
                    s => s.Id == id && s.AgencyId == resolvedAgencyId && !s.IsDeleted);

                if (suggestion == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Suggestion not found",
                        ErrorCode = ServiceErrorCodes.NotFound,
                        Data = false
                    };
                }

                suggestion.IsDeleted = true;
                suggestion.DeletedAt = DateTime.UtcNow;
                
                await _unitOfWork.QuickSuggestions.UpdateAsync(suggestion);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Suggestion deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting suggestion: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError,
                    Data = false
                };
            }
        }

        public async Task<ServiceResponse<QuickSuggestionSettingsResponse>> GetSettingsAsync(Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var settings = await _unitOfWork.QuickSuggestionSettings.FirstOrDefaultAsync(
                    s => s.AgencyId == resolvedAgencyId && !s.IsDeleted);

                if (settings == null)
                {
                    settings = new QuickSuggestionSettings
                    {
                        AgencyId = resolvedAgencyId,
                        IsEntryExitEnabled = true,
                        IsRoutineEnabled = true,
                        CombineDictionaries = true
                    };
                    await _unitOfWork.QuickSuggestionSettings.AddAsync(settings);
                    await _unitOfWork.CommitAsync();
                }

                var response = _mapper.Map<QuickSuggestionSettingsResponse>(settings);
                return new ServiceResponse<QuickSuggestionSettingsResponse>
                {
                    Success = true,
                    Message = "Settings retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<QuickSuggestionSettingsResponse>
                {
                    Success = false,
                    Message = $"Error retrieving settings: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<QuickSuggestionSettingsResponse>> UpdateSettingsAsync(UpdateQuickSuggestionSettingsRequest request)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);
                var settings = await _unitOfWork.QuickSuggestionSettings.FirstOrDefaultAsync(
                    s => s.AgencyId == resolvedAgencyId && !s.IsDeleted);

                if (settings == null)
                {
                    settings = new QuickSuggestionSettings
                    {
                        AgencyId = resolvedAgencyId,
                        IsEntryExitEnabled = request.IsEntryExitEnabled,
                        IsRoutineEnabled = request.IsRoutineEnabled,
                        CombineDictionaries = request.CombineDictionaries
                    };
                    await _unitOfWork.QuickSuggestionSettings.AddAsync(settings);
                }
                else
                {
                    settings.IsEntryExitEnabled = request.IsEntryExitEnabled;
                    settings.IsRoutineEnabled = request.IsRoutineEnabled;
                    settings.CombineDictionaries = request.CombineDictionaries;
                    await _unitOfWork.QuickSuggestionSettings.UpdateAsync(settings);
                }

                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<QuickSuggestionSettingsResponse>(settings);
                return new ServiceResponse<QuickSuggestionSettingsResponse>
                {
                    Success = true,
                    Message = "Settings updated successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<QuickSuggestionSettingsResponse>
                {
                    Success = false,
                    Message = $"Error updating settings: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<ImportPreviewResult>> PreviewImportAsync(
            QuickSuggestionType type, Stream fileStream, string fileName, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                // Read lines
                using var reader = new StreamReader(fileStream);
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }

                // Parse columns mapping from header if present
                bool hasHeader = false;
                int textIndex = 0;
                int shortcutIndex = 1;

                if (lines.Count > 0)
                {
                    var firstLineFields = ParseCsvLine(lines[0]);
                    var textHeaderIdx = firstLineFields.FindIndex(f => f.Equals("text", StringComparison.OrdinalIgnoreCase) || 
                                                                      f.Equals("phrase", StringComparison.OrdinalIgnoreCase) || 
                                                                      f.Equals("suggestion", StringComparison.OrdinalIgnoreCase));
                    var shortcutHeaderIdx = firstLineFields.FindIndex(f => f.Equals("shortcut", StringComparison.OrdinalIgnoreCase) || 
                                                                          f.Equals("code", StringComparison.OrdinalIgnoreCase) || 
                                                                          f.Equals("trigger", StringComparison.OrdinalIgnoreCase));
                    if (textHeaderIdx >= 0 || shortcutHeaderIdx >= 0)
                    {
                        hasHeader = true;
                        if (textHeaderIdx >= 0) textIndex = textHeaderIdx;
                        if (shortcutHeaderIdx >= 0) shortcutIndex = shortcutHeaderIdx;
                    }
                }

                var importRows = new List<QuickSuggestionImportRow>();
                int startRow = hasHeader ? 1 : 0;

                // Load existing suggestions for duplicate validation
                var existingSuggestions = await _unitOfWork.QuickSuggestions.GetAsync(
                    s => s.AgencyId == resolvedAgencyId && s.Type == type && !s.IsDeleted
                );
                var existingTexts = existingSuggestions.Select(s => s.Text.Trim().ToLowerInvariant()).ToHashSet();
                var existingShortcuts = existingSuggestions
                    .Where(s => !string.IsNullOrEmpty(s.Shortcut))
                    .Select(s => s.Shortcut!.Trim().ToLowerInvariant())
                    .ToHashSet();

                var processedTexts = new HashSet<string>();
                var processedShortcuts = new HashSet<string>();

                for (int i = startRow; i < lines.Count; i++)
                {
                    var fields = ParseCsvLine(lines[i]);
                    var rowNumber = i + 1; // 1-based index in file

                    var rowText = fields.Count > textIndex ? fields[textIndex]?.Trim() : string.Empty;
                    var rowShortcut = fields.Count > shortcutIndex ? fields[shortcutIndex]?.Trim() : string.Empty;

                    var importRow = new QuickSuggestionImportRow
                    {
                        RowNumber = rowNumber,
                        Text = rowText ?? string.Empty,
                        Shortcut = string.IsNullOrWhiteSpace(rowShortcut) ? null : rowShortcut
                    };

                    // 1. Text Empty check
                    if (string.IsNullOrWhiteSpace(rowText))
                    {
                        importRow.ValidationErrors.Add("Suggestion phrase text is empty.");
                    }
                    else
                    {
                        // 2. Length check
                        if (rowText.Length > 1000)
                        {
                            importRow.ValidationErrors.Add("Suggestion phrase exceeds 1000 characters limit.");
                        }

                        // 3. Duplicate check within file
                        var textLower = rowText.ToLowerInvariant();
                        if (processedTexts.Contains(textLower))
                        {
                            importRow.ValidationErrors.Add("Duplicate suggestion phrase found in this import file.");
                        }
                        else
                        {
                            processedTexts.Add(textLower);
                        }

                        // 4. Duplicate check in database
                        if (existingTexts.Contains(textLower))
                        {
                            importRow.ValidationErrors.Add("Suggestion phrase already exists in this library.");
                        }
                    }

                    // Shortcut checks
                    if (!string.IsNullOrWhiteSpace(rowShortcut))
                    {
                        if (rowShortcut.Length > 50)
                        {
                            importRow.ValidationErrors.Add("Shortcut exceeds 50 characters limit.");
                        }

                        var shortcutLower = rowShortcut.ToLowerInvariant();

                        // Duplicate shortcut within file
                        if (processedShortcuts.Contains(shortcutLower))
                        {
                            importRow.ValidationErrors.Add("Duplicate shortcut found in this import file.");
                        }
                        else
                        {
                            processedShortcuts.Add(shortcutLower);
                        }

                        // Duplicate shortcut in database
                        if (existingShortcuts.Contains(shortcutLower))
                        {
                            importRow.ValidationErrors.Add($"Shortcut '{rowShortcut}' is already assigned to another suggestion.");
                        }
                    }

                    importRow.IsValid = !importRow.ValidationErrors.Any();
                    importRows.Add(importRow);
                }

                var previewResult = new ImportPreviewResult
                {
                    TotalRecords = importRows.Count,
                    ValidCount = importRows.Count(r => r.IsValid),
                    InvalidCount = importRows.Count(r => !r.IsValid),
                    Rows = importRows
                };

                return new ServiceResponse<ImportPreviewResult>
                {
                    Success = true,
                    Message = "Import file parsed successfully",
                    Data = previewResult
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ImportPreviewResult>
                {
                    Success = false,
                    Message = $"Failed to preview import: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<int>> CommitImportAsync(CommitImportRequest request)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);

                // Double check constraints inside transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                int importedCount = 0;

                // Reload active database suggestions to avoid duplicate inserts during committing
                var existingSuggestions = await _unitOfWork.QuickSuggestions.GetAsync(
                    s => s.AgencyId == resolvedAgencyId && s.Type == request.Type && !s.IsDeleted
                );
                var existingTexts = existingSuggestions.Select(s => s.Text.Trim().ToLowerInvariant()).ToHashSet();
                var existingShortcuts = existingSuggestions
                    .Where(s => !string.IsNullOrEmpty(s.Shortcut))
                    .Select(s => s.Shortcut!.Trim().ToLowerInvariant())
                    .ToHashSet();

                foreach (var record in request.Records)
                {
                    var text = record.Text?.Trim();
                    var shortcut = record.Shortcut?.Trim();

                    if (string.IsNullOrWhiteSpace(text) || text.Length > 1000)
                        continue; // Skip invalid entries

                    if (shortcut != null && shortcut.Length > 50)
                        continue;

                    var textLower = text.ToLowerInvariant();
                    if (existingTexts.Contains(textLower))
                        continue; // Skip duplicate text

                    if (!string.IsNullOrEmpty(shortcut))
                    {
                        var shortcutLower = shortcut.ToLowerInvariant();
                        if (existingShortcuts.Contains(shortcutLower))
                            continue; // Skip duplicate shortcut

                        existingShortcuts.Add(shortcutLower);
                    }

                    existingTexts.Add(textLower);

                    var entity = new QuickSuggestion
                    {
                        AgencyId = resolvedAgencyId,
                        Type = request.Type,
                        Text = text,
                        Shortcut = string.IsNullOrWhiteSpace(shortcut) ? null : shortcut,
                        IsActive = true
                    };

                    await _unitOfWork.QuickSuggestions.AddAsync(entity);
                    importedCount++;
                }

                if (importedCount > 0)
                {
                    await _unitOfWork.CommitAsync();
                }

                await transaction.CommitAsync();

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = $"Imported {importedCount} suggestions successfully",
                    Data = importedCount
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Failed to commit import: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<byte[]>> ExportToCsvAsync(QuickSuggestionType type, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                // Fetch sorted active suggestions
                var suggestions = await _unitOfWork.QuickSuggestions.GetAsync(
                    s => s.AgencyId == resolvedAgencyId && s.Type == type && !s.IsDeleted,
                    orderBy: q => q.OrderBy(s => s.Text)
                );

                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Text,Shortcut");

                foreach (var suggestion in suggestions)
                {
                    csvBuilder.AppendLine($"{EscapeCsvField(suggestion.Text)},{EscapeCsvField(suggestion.Shortcut)}");
                }

                var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

                return new ServiceResponse<byte[]>
                {
                    Success = true,
                    Message = "CSV generated successfully",
                    Data = bytes
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>
                {
                    Success = false,
                    Message = $"Failed to export CSV: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentField = new StringBuilder();
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    // Check if double quote is an escaped quote (e.g. "")
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.ToString().Trim());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
            
            result.Add(currentField.ToString().Trim());
            return result;
        }

        private static string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field)) return string.Empty;
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
