using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Validators.Base
{
    public class Validator<T> :  IValidator<T> where T : class
    {
        public virtual async Task<(bool, string)> Validate(T entity)
        {
            return await Task.FromResult((false, string.Empty));
        }
    }
}

