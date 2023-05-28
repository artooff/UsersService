using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using UsersService.Application.Common;
using UsersService.Application.DTO;
using UsersService.Application.Exceptions;
using UsersService.Application.Interfaces;
using UsersService.Domain.Models;
using UsersService.Infrastructure.Persistance;

namespace UsersService.Infrastructure.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDbContext _context;

        public UsersRepository(UsersDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddUser(User user)
        {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

            return users;
        }
        
        public async Task<User> GetUser(string login)
        {
            var user = await _context.Users.Where(x => x.Login == login).SingleOrDefaultAsync();
            return user;
        }

        public async Task UpdateLogin(UpdateLoginDto model, UserWithPermissions currentUser)
        {
            var userToUpdate = await _context.Users
            .FirstOrDefaultAsync(user => user.Login == model.Login);

            if (userToUpdate == null)
            {
                throw new NotFoundException(model.Login);
            }
            if (currentUser.IsAdmin || !userToUpdate.RevokedOn.HasValue)
            {
                userToUpdate.Login = model.NewLogin;
                userToUpdate.ModifiedBy = model.ModifiedBy;
                userToUpdate.ModifiedOn = model.ModifiedOn;

                await _context.SaveChangesAsync();
            }
            else
                throw new AccessDeniedException(model.Login);
        }


        public async Task UpdatePassword(UpdatePasswordDto model, UserWithPermissions currentUser)
        {
            var userToUpdate = await _context.Users
            .FirstOrDefaultAsync(user => user.Login == model.Login);

            if (userToUpdate == null)
            {
                throw new NotFoundException(model.Login);
            }
            if (currentUser.IsAdmin || !userToUpdate.RevokedOn.HasValue)
            {
                userToUpdate.Password = model.NewPassword;
                userToUpdate.ModifiedBy = model.ModifiedBy;
                userToUpdate.ModifiedOn = model.ModifiedOn;

                await _context.SaveChangesAsync();
            }
            else
                throw new AccessDeniedException(model.Login);
        }

        public async Task UpdateDetails(string login, JsonPatchDocument model, UserWithPermissions currentUser)
        {
            var userToUpdate = await _context.Users
            .FirstOrDefaultAsync(user => user.Login == login);

            if (userToUpdate == null)
            {
                throw new NotFoundException(login);
            }
            if (currentUser.IsAdmin || !userToUpdate.RevokedOn.HasValue)
            {
                model.ApplyTo(userToUpdate);
                userToUpdate.ModifiedBy = currentUser.Login;
                userToUpdate.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            else
                throw new AccessDeniedException(login);
        }

        public async Task<int> DeleteUserFull(string login)
        {
            var deleted = await _context.Users.Where(user => user.Login == login).ExecuteDeleteAsync();
            return deleted;
        }

        public async Task<int> DeleteUserSoft(string login, string revokedBy)
        {
            var revoked = await _context.Users.
                Where(user => user.Login == login).
                ExecuteUpdateAsync(e =>
                e.SetProperty(user => user.RevokedBy, revokedBy)
                .SetProperty(user => user.RevokedOn, DateTime.UtcNow));
            return revoked;
        }

        public async Task<int> RestoreUser(string login)
        {
            string revokedBy = null;
            DateTime? revokedOn = null;

            var restored = await _context.Users.
                Where(user => user.Login == login).
                ExecuteUpdateAsync(e =>
                e.SetProperty(user => user.RevokedBy, revokedBy)
                .SetProperty(user => user.RevokedOn, revokedOn));
            return restored;
        }

        public async Task<bool> AdminExists()
        {
            var admin = await _context.Users.Where(user => user.IsAdmin).FirstOrDefaultAsync();
            return admin != null;
        }
    }
}
