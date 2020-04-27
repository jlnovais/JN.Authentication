using System.Threading.Tasks;
using JN.Authentication.HelperClasses;

namespace JN.Authentication.Interfaces
{
    public interface IApiKeyValidationService
    {
        Task<ValidationResult> ValidateApiKey(string apiKey);
    }
}
