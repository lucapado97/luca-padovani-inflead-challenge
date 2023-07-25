using Challenge.LocalDbContext;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Filters
{
    public class UserFilter
    {
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public IQueryable<User> ApplyFilter(IQueryable<User> dbSet)
        {
            if (!string.IsNullOrEmpty(this.Gender))
                dbSet = dbSet.Where(u => u.Gender == this.Gender);
            if (!string.IsNullOrEmpty(this.Email))
                dbSet = dbSet.Where(u => u.Email == this.Email);
            if (!string.IsNullOrEmpty(this.Username))
                dbSet = dbSet.Where(u => u.Username == this.Username);
            return dbSet;
        }
    }
}
