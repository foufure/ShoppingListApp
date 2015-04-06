using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public static class RepositoryNameProviderValidationExtensions
    {
        public static void RepositoryNameValidation<T>(this T repositoryNameProviderToValidate)
                where T : IRepositoryNameProvider
        {
            if (repositoryNameProviderToValidate == null)
            {
                throw new ArgumentNullException("Internal error: Items Repository could not be initialized because the repository name provider is null", (Exception)null);
            }

            if (repositoryNameProviderToValidate.RepositoryName == null)
            {
                throw new ArgumentNullException("Internal error: Items Repository could not be initialized because the repository name is null", (Exception)null);
            }

            if (string.IsNullOrEmpty(repositoryNameProviderToValidate.RepositoryName))
            {
                throw new ArgumentOutOfRangeException("Internal error: Items Repository could not be initialized because the repository name is an empty string", (Exception)null);
            }
        }
    }
}
