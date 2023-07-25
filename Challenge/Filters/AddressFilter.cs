using Challenge.LocalDbContext;

namespace Challenge.Filters
{
    public class AddressFilter
    {
        public string uuid { get; set; } = string.Empty;

        public IQueryable<Address> ApplyFilter(IQueryable<Address> dbSet)
        {
            if (!string.IsNullOrEmpty(this.uuid))
                dbSet = dbSet.Where(a => a.Id == this.uuid);
            return dbSet;
        }
    }
}
