using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Exceptions;
using MT.Core.Extensions;
using MT.Core.Model;

namespace MT.Core.Services
{
    public class TenantManager : TenantManager<Tenant, string>
    {
        public TenantManager(TenantCatalogContext<Tenant, string> context,
            IDataProtectionProvider dataProtectionProvider) : base(context, dataProtectionProvider)
        {
        }
    }

    public class TenantManager<TTenant> : TenantManager<TTenant, string>
        where TTenant : Tenant<string>
    {
        public TenantManager(TenantCatalogContext<TTenant, string> context,
            IDataProtectionProvider dataProtectionProvider) : base(context, dataProtectionProvider)
        {
        }
    }

    public class TenantManager<TTenant, TKey>
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly TenantCatalogContext<TTenant, TKey> _context;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public TenantManager(
            TenantCatalogContext<TTenant, TKey> context,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public TTenant AddTenant(TTenant tenant)
        {
            _context.Tenants.Add(tenant);
            _context.SaveChanges();
            return tenant;
        }

        public async Task<TTenant> AddTenantAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            var encryptPassword = EncryptionHelper.Encrypt(tenant.Password, tenant.ConcurencyStamp);
            tenant.Password = encryptPassword;
            await _context.Tenants.AddAsync(tenant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return tenant;
        }

        public void RemoveTenant(TKey id)
        {
            var tenant = _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            RemoveTenant(tenant);
        }

        public void RemoveTenant(TTenant tenant)
        {
            _context.Tenants.Remove(tenant);
            _context.SaveChanges();
        }

        public async Task RemoveTenantAsync(TKey id)
        {
            var tenant = await _context.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            await RemoveTenantAsync(tenant);
        }

        public async Task RemoveTenantAsync(TTenant tenant)
        {
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
        }

        public TTenant Get(TKey id) => _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
        public Task<TTenant> GetAsync(TKey id) => _context.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();

        public string GetTenantPassword(string tenantPassword, string concurrencyStamp)
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