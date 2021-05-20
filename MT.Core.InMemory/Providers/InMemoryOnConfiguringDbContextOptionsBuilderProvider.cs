using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Interfaces;

namespace MT.Core.InMemory.Providers
{
    /// <inheritdoc />
    public class InMemoryOnConfiguringDbContextOptionsBuilderProvider : IOnConfiguringDbContextOptionsBuilderProvider
    {
        /// <inheritdoc />
        public DbContextOptionsBuilder Provide(DbContextOptionsBuilder optionsBuilder, SqlConnection sqlConnection)
        {
            optionsBuilder.UseInMemoryDatabase(sqlConnection.ConnectionString);
            return optionsBuilder;
        }
    }
}