using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MT.Core.Model;

namespace MT.Core.Validators
{
    /// <summary>
    /// dsa
    /// </summary>
    /// <typeparam name="TTenant"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class TenantValidator<TTenant, TKey>
     where TTenant : Tenant<TKey> 
     where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Validates provided tenant before it is added to the database
        /// </summary>
        /// <param name="tenant"><see cref="Tenant{TKey}"/></param>
        /// <returns><see cref="ValidationResult"/></returns>
        public ValidationResult Validate(TTenant tenant)
        {
            if (tenant is null)
            {
                return new ValidationResult("Tenant object is missing", new List<string>() {nameof(Tenant<TKey>)});
            }
            if (string.IsNullOrWhiteSpace(tenant.Password))
            {
                return new ValidationResult("Password is missing", new List<string>() {nameof(Tenant<TKey>.Password)});
            }

            if (string.IsNullOrWhiteSpace(tenant.Server))
            {
                return new ValidationResult("Server is missing", new List<string>() {nameof(Tenant<TKey>.Server)});
            }

            if (string.IsNullOrWhiteSpace(tenant.Database))
            {
                return new ValidationResult("Database is missing", new List<string>() { nameof(Tenant<TKey>.Database) });
            }

            return ValidationResult.Success;
        }
    }
}