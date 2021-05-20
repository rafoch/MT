using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MT.Core.Interfaces
{
    /// <summary>
    /// Interface to provide sql connection
    /// </summary>
    public interface IOnConfiguringDbContextOptionsBuilderProvider
    {
        /// <summary>
        /// Prepares proper connection to database
        /// </summary>
        /// <param name="optionsBuilder"><see cref="DbContextOptionsBuilder"/></param>
        /// <param name="sqlConnection"><see cref="SqlConnection"/></param>
        /// <returns><see cref="DbContextOptionsBuilder"/></returns>
        DbContextOptionsBuilder Provide(DbContextOptionsBuilder optionsBuilder, SqlConnection sqlConnection);
    }
}