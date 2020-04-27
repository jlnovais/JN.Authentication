using System.Threading.Tasks;
using JN.Authentication.HelperClasses;

namespace JN.Authentication.Interfaces
{
    public interface IBasicValidationService
    {
        Task<ValidationResult> ValidateUser(string username, string password, string resourceName);
    }
}
