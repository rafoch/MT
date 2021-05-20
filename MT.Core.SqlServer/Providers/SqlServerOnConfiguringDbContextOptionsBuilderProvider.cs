using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Interfaces;

namespace MT.Core.SqlServer.Providers
{
    /// <inheritdoc />
    public class SqlServerOnConfiguringDbContextOptionsBuilderProvider : IOnConfiguringDbContextOptionsBuilderProvider
    {
        /// <inheritdoc />
        public DbContextOptionsBuilder Provide(DbContextOptionsBuilder optionsBuilder, SqlConnection sqlConnection)
        {
            optionsBuilder.UseSqlServer(sqlConnection);
            return optionsBuilder;
        }
    }
}