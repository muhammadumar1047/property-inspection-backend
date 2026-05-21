using System;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IEmailTemplateProcessor
    {
        Task<string> ProcessTemplateAsync(string templateBody, Guid inspectionId);
    }
}
