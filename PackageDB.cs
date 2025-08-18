using Microsoft.EntityFrameworkCore;
using PackageModels;

class PackageDB : DbContext
{
    public PackageDB(DbContextOptions<PackageDB> options)
        : base(options) { }

    public DbSet<Package> Packages => Set<Package>();
}