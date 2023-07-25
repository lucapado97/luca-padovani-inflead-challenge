using Challenge.LocalDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System.IO;

namespace Challenge.Managers
{
    public class Address
    {
        public string city { get; set; }
        public string street_name { get; set; }
        public string street_address { get; set; }
        public string zip_code { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public Coordinates coordinates { get; set; }
    }

    public class Coordinates
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class CreditCard
    {
        public string cc_number { get; set; }
    }

    public class Employment
    {
        public string title { get; set; }
        public string key_skill { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public string uid { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public string gender { get; set; }
        public string phone_number { get; set; }
        public string social_insurance_number { get; set; }
        public string date_of_birth { get; set; }
        public Employment employment { get; set; }
        public Address address { get; set; }
        public CreditCard credit_card { get; set; }
        public Subscription subscription { get; set; }
    }

    public class Subscription
    {
        public string plan { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        public string term { get; set; }
    }

    public class RandomDataManager
    {
        private readonly IOptions<SQLiteDbContextOptions<SQLiteDbContext>> _dbContextOptions;
        public RandomDataManager(IOptions<SQLiteDbContextOptions<SQLiteDbContext>> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<bool> UpdateLocalDataAsync()
        {
            await using var db = new SQLiteDbContext(_dbContextOptions.Value.DbContextOptions.Options);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://random-data-api.com/api/users/random_user?size=10");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IList<Root>>();
                if (result == null)
                    return false;
                foreach (var record in result)
                {
                    //se record non presente a db lo salvo
                    if(!db.Users.Where(u => u.Id.Equals(record.uid)).Any())
                    {
                        var addressToSave = new LocalDbContext.Address()
                        {
                            Id = Guid.NewGuid().ToString(),
                            City = record.address.city,
                            CreationDate = DateTime.Now,
                            State = record.address.state,
                            Street = $"{record.address.street_name} {record.address.street_address}",
                            ZipCode = record.address.zip_code
                        };
                        var userToSave = new User()
                        {
                            Id = record.uid,
                            CreationDate = DateTime.Now,
                            Email = record.email,
                            Employment = record.employment.title,
                            FullName = $"{record.first_name} {record.last_name}",
                            Gender = record.gender,
                            KeySkill = record.employment.key_skill,
                            PhoneNumber = record.phone_number,
                            ProfilePicUrl = record.avatar,
                            Username = record.username,
                            AddressId = addressToSave.Id
                        };
                        db.Users.Add(userToSave);
                        db.Addresses.Add(addressToSave);
                    }
                }
                await db.SaveChangesAsync();
            }
            else
                return false;
            return true;
        }


}
}
