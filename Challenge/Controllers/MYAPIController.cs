using Challenge.Filters;
using Challenge.LocalDbContext;
using Challenge.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Challenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MYAPIController : ControllerBase
    {
        private readonly IOptions<SQLiteDbContextOptions<SQLiteDbContext>> _dbContextOptions;
        private readonly RandomDataManager _dataManager;
        public MYAPIController(IOptions<SQLiteDbContextOptions<SQLiteDbContext>> dbContextOptions, RandomDataManager dataManager)
        {
            _dbContextOptions = dbContextOptions;
            _dataManager = dataManager;
        }

        // POST: MYAPIController/SearchUser
        [HttpPost(Name = "SearchUser")]
        public async Task<IActionResult> SearchUser([FromBody] UserFilter filter)
        {
            //check on local db, if no result call EXAPI
            await using var db = new SQLiteDbContext(_dbContextOptions.Value.DbContextOptions.Options);

            IQueryable<User> uList = db.Users;
            if (filter != null)
                uList = filter.ApplyFilter(uList);
            if (!uList.Any())
            {
                await _dataManager.UpdateLocalDataAsync();
                uList = db.Users;
                if (filter != null)
                    uList = filter.ApplyFilter(uList);
            }
            return new JsonResult(uList.ToList());
        }

    }
}
