using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.CustomerReviews.Data.Repositories;

namespace VirtoCommerce.CustomerReviews.Data.PostgreSql
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<CustomerReviewsDbContext>
    {
        public CustomerReviewsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CustomerReviewsDbContext>();
            var connectionString = args.Any() ? args[0] : "User ID = postgres; Password = password; Host = localhost; Port = 5432; Database = virtocommerce3;";

            builder.UseNpgsql(
                connectionString,
                db => db.MigrationsAssembly(typeof(PostgreSqlDbContextFactory).Assembly.GetName().Name));

            return new CustomerReviewsDbContext(builder.Options);
        }
    }
}
