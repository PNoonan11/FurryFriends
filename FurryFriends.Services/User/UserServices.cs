using FurryFriends.Data;
using FurryFriends.Models.Users;
using FurryFriends.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FurryFriends.Models.User;
using System.Linq;

namespace FurryFriends.Services.User
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _DbContext;
        public UserServices(ApplicationDbContext DbContext)
        {
            _DbContext = DbContext;
        }
        private readonly int _userId;

        public async Task<bool> RegisterUserAsync(UserCreate model)
        {
            if (await GetUserByEmailAsync(model.Email) != null || await GetUserByUsernameAsync(model.Username) != null)
                return false;

            var entity = new UserEntity
            {
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var passwordHasher = new PasswordHasher<UserEntity>();
            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _DbContext.User.Add(entity);
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;

        }

        private async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _DbContext.User.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
        }

        private async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return await _DbContext.User.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
        }

        // private async Task<bool> VerifyValidId (int id)
        // {
        //     var result = await _DbContext.User.FirstOrDefaultAsync(x => x.Id == id);

        //     return result != null ? true : false; 
        // }

        public async Task<bool> CreatePetProfileAsync(CreatePetProfile model)
        {
            // if (! await VerifyValidId(model.OwnerId)) return false;
            _DbContext.Profiles.Add(new ProfileEntity
            {
                Name = model.Name,
                Age = model.Age,
                PetType = model.PetType,
                BreedId = model.BreedId,
                CityID = model.CityID,
                Bio = model.Bio,
                Size = model.Size
            });
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;
        }
        
        public async Task<List<PetProfile>> GetAllProfiles()
        {
            List<PetProfile> entity = await _DbContext.Profiles
                .Select(r => new PetProfile()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Age = r.Age,
                    PetType = ((PetTypes)r.PetType).ToString(),
                    BreedId = ((Breeds)r.BreedId).ToString(),
                    CityID = ((CityNames)r.CityID).ToString(),
                    Bio = r.Bio,
                    Size = ((PetSizes)r.Size).ToString()
                })
                .ToListAsync();
            return entity;
        }

        public List<PetProfile> GetProfileByLocation(int CityID)
        {
            var petList = _DbContext.Profiles
                .Where (x => (int)x.CityID == CityID)
                .Select(
                    x => new PetProfile
                    {
                        Name = x.Name,
                        Age = x.Age,
                        PetType = ((PetTypes)x.PetType).ToString(),
                        BreedId = ((Breeds)x.BreedId).ToString(),
                        CityID = ((CityNames)x.CityID).ToString(),
                        Bio = x.Bio,
                        Size = ((PetSizes)x.Size).ToString()

                    }
                );
            return petList.ToList();
        }

        public List<PetProfile> GetProfileByAnimalType(int PetType)
        {
            var petList = _DbContext.Profiles
                .Where (x => (int)x.PetType == PetType)
                .Select(
                    x => new PetProfile
                    {
                        Name = x.Name,
                        Age = x.Age,
                        PetType = ((PetTypes)x.PetType).ToString(),
                        BreedId = ((Breeds)x.BreedId).ToString(),
                        CityID = ((CityNames)x.CityID).ToString(),
                        Bio = x.Bio,
                        Size = ((PetSizes)x.Size).ToString()

                    }
                );
            return petList.ToList();
        }

        public List<PetProfile> GetProfileByBreed(int BreedId)
        {
            var petList = _DbContext.Profiles
                .Where (x => (int)x.BreedId == BreedId)
                .Select(
                    x => new PetProfile
                    {
                        Name = x.Name,
                        Age = x.Age,
                        PetType = ((PetTypes)x.PetType).ToString(),
                        BreedId = ((Breeds)x.BreedId).ToString(),
                        CityID = ((CityNames)x.CityID).ToString(),
                        Bio = x.Bio,
                        Size = ((PetSizes)x.Size).ToString()

                    }
                );
            return petList.ToList();
        }

        public List<PetProfile> GetProfileBySize(int size)
        {
            var petList = _DbContext.Profiles
                .Where(x => (int)x.Size == size)
                .Select(
                    x => new PetProfile
                    {
                        Name = x.Name,
                        Age = x.Age,
                        PetType = ((PetTypes)x.PetType).ToString(),
                        BreedId = ((Breeds)x.BreedId).ToString(),
                        CityID = ((CityNames)x.CityID).ToString(),
                        Bio = x.Bio,
                        Size = ((PetSizes)x.Size).ToString()
                    }
                );

            return petList.ToList();
        }

        public List<ProfileEntity> GetProfileByAgeRange(int UpperAge, int LowerAge)
        {
            var petList = _DbContext.Profiles
                .Where(x => x.Age >= LowerAge && x.Age <= UpperAge)
                .Select(
                    x => new ProfileEntity
                    {
                        Name = x.Name,
                        Age = x.Age,
                        PetType = x.PetType,
                        BreedId = x.BreedId,
                        CityID = x.CityID,
                        Bio = x.Bio,
                        Size = x.Size
                    }
                );
            return petList.ToList();
        }

        public async Task<bool> UpdateAProfile(ProfileUpdate request)
        {
            var entity = await _DbContext.Profiles.FindAsync(request.Id);
            if (entity?.Id != _userId)
                return false;

            var profileUpdate = new ProfileUpdate
            {
                Name = entity.Name,
                Age = entity.Age,
                PetType = (int)entity.PetType,
                BreedId = (int)entity.BreedId,
                CityID = (int)entity.CityID,
                Bio = entity.Bio,
                Size = (int)entity.Size
            };

            var numberOfChanges = await _DbContext.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<bool> DeleteAUser(int Id)
        {
            var entity = await _DbContext.User.FindAsync(Id);
            if (entity?.Id != _userId)
                return false;
            var userToDelete = await _DbContext.User.FindAsync(Id);

            _DbContext.User.Remove(userToDelete);
            return await _DbContext.SaveChangesAsync() == 1;
        }
    }
}