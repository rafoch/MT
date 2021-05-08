using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Exceptions;
using MT.Core.Extensions;
using MT.Core.Model;

namespace MT.Core.Services
{
    /// <inheritdoc />
    public class TenantManager : TenantManager<Tenant, string>
    {
        /// <inheritdoc />
        public TenantManager(TenantCatalogDbContext<Tenant, string> dbContext) : base(dbContext)
        {
        }
    }

    /// <inheritdoc />
    public class TenantManager<TTenant> : TenantManager<TTenant, string>
        where TTenant : Tenant<string>
    {
        /// <inheritdoc />
        public TenantManager(TenantCatalogDbContext<TTenant, string> dbContext) : base(dbContext)
        {
        }
    }

    /// <summary>
    /// Tenant Manager allows you to manage <see cref="Tenant{TKey}"/> object in application.
    /// </summary>
    /// <typeparam name="TTenant">Object that inherits from <see cref="Tenant{TKey}"/></typeparam>
    /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
    public class TenantManager<TTenant, TKey>
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly TenantCatalogDbContext<TTenant, TKey> _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public TenantManager(
            TenantCatalogDbContext<TTenant, TKey> dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Register tenant in <see cref="TenantCatalogDbContext{TTenant,TKey}.Tenants"/> 
        /// </summary>
        /// <param name="tenant">Object that implements <see cref="Tenant{TKey}"/> class</param>
        /// <returns>Object that inherits from <see cref="Tenant{TKey}"/></returns>
        public TTenant AddTenant([NotNull] TTenant tenant)
        {
            if (tenant is null)
            {
                throw new TenantObjectIsMissingException();
            }

            if (string.IsNullOrWhiteSpace(tenant.Password))
            {
                throw new TenantDatabasePasswordIsMissingException();
            }
            var encryptPassword = EncryptionHelper.Encrypt(tenant.Password, tenant.ConcurrencyStamp);
            tenant.Password = encryptPassword;
            _dbContext.Tenants.Add(tenant);
            _dbContext.SaveChanges();
            return tenant;
        }

        /// <summary>
        /// Register tenant in <see cref="TenantCatalogDbContext{TTenant,TKey}.Tenants"/> 
        /// </summary>
        /// <param name="tenant">Object that implements <see cref="Tenant{TKey}"/> class</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns>Object that inherits from <see cref="Tenant{TKey}"/></returns>
        public async Task<TTenant> AddTenantAsync([NotNull] TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (tenant is null)
            {
                throw new TenantObjectIsMissingException();
            }

            if (string.IsNullOrWhiteSpace(tenant.Password))
            {
                throw new TenantDatabasePasswordIsMissingException();
            }

            var encryptPassword = EncryptionHelper.Encrypt(tenant.Password, tenant.ConcurrencyStamp);
            tenant.Password = encryptPassword;
            await _dbContext.Tenants.AddAsync(tenant, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return tenant;
        }

        /// <summary>
        /// Removes tenant with specified id from catalog database
        /// </summary>
        /// <param name="id"><see cref="Tenant{TKey}.Id"/></param>
        public void RemoveTenant(TKey id)
        {
            var tenant = _dbContext.Tenants.Filter(t => t.Id, id).FirstOrDefault();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            RemoveTenant(tenant);
        }

        /// <summary>
        /// Removes tenant from catalog database
        /// </summary>
        /// <param name="tenant">Object that inherits from <see cref="Tenant{TKey}"/></param>
        public void RemoveTenant(TTenant tenant)
        {
            _dbContext.Tenants.Remove(tenant);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes tenant with specified id from catalog database
        /// </summary>
        /// <param name="id"><see cref="Tenant{TKey}.Id"/></param>
        public async Task RemoveTenantAsync(TKey id)
        {
            var tenant = await _dbContext.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            await RemoveTenantAsync(tenant);
        }

        /// <summary>
        /// Removes tenant from catalog database
        /// </summary>
        /// <param name="tenant">Object that inherits from <see cref="Tenant{TKey}"/></param>
        public async Task RemoveTenantAsync(TTenant tenant)
        {
            _dbContext.Tenants.Remove(tenant);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gives a tenant object by specific id
        /// </summary>
        /// <param name="id"><see cref="Tenant{TKey}.Id"/></param>
        /// <returns>Object that inherits from <see cref="Tenant{TKey}"/></returns>
        public TTenant Get(TKey id) => _dbContext.Tenants.Filter(t => t.Id, id).FirstOrDefault();

        /// <summary>
        /// Gives a tenant object by specific id
        /// </summary>
        /// <param name="id"><see cref="Tenant{TKey}.Id"/></param>
        /// <returns>Object that inherits from <see cref="Tenant{TKey}"/></returns>
        public Task<TTenant> GetAsync(TKey id) => _dbContext.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();

        internal string GetTenantPassword(string tenantPassword, string concurrencyStamp)
        {
            var rawPassword = EncryptionHelper.Decrypt(tenantPassword, concurrencyStamp);
            return rawPassword;
        }

        private static class EncryptionHelper
        {
            internal static string Encrypt(string clearText, string key)
            {
                string EncryptionKey = key;
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            internal static string Decrypt(string cipherText, string key)
            {
                string EncryptionKey = key;
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
        }
    }
}