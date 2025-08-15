using Microsoft.EntityFrameworkCore;

class PackageDB : DbContext
{
    public PackageDB(DbContextOptions<PackageDB> options)
        : base(options) { }

    public DbSet<PackageTest> Packages => Set<PackageTest>();
}