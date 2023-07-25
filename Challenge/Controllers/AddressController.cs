using Challenge.Filters;
using Challenge.LocalDbContext;
using Challenge.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Challenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IOptions<SQLiteDbContextOptions<SQLiteDbContext>> _dbContextOptions;

        public AddressController(IOptions<SQLiteDbContextOptions<SQLiteDbContext>> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        // POST: AddressController/SearchAddress
        [HttpPost(Name = "SearchAddress")]
        public async Task<IActionResult> SearchAddress([FromBody] AddressFilter filter)
        {
            await using var db = new SQLiteDbContext(_dbContextOptions.Value.DbContextOptions.Options);

            IQueryable<LocalDbContext.Address> aList = db.Addresses;
            if (filter != null)
                aList = filter.ApplyFilter(aList);
            return new JsonResult(aList.ToList());
        }
    }
}
