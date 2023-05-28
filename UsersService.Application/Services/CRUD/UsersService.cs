using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UsersService.Application.Common;
using UsersService.Application.DTO;
using UsersService.Application.Exceptions;
using UsersService.Application.Interfaces;
using UsersService.Domain.Models;

namespace UsersService.Application.Services.CRUD
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IValidator<AddUserDto> _userValidator;
        private readonly IValidator<UpdateLoginDto> _updateLoginValidator;
        private readonly IValidator<UpdatePasswordDto> _updatePasswordValidator;
        private readonly IValidator<UpdateDetailsDto> _updateDetailsValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IUsersRepository usersRepository,
            IPasswordHashService passwordHashService,
            IValidator<AddUserDto> userValidator,
            IValidator<UpdateLoginDto> updateLoginValidator,
            IValidator<UpdatePasswordDto> updatePasswordValidator,
            IValidator<UpdateDetailsDto> updateDetailsValidator,
            IMapper mapper,
            ILogger<UsersService> logger)
        {
            _usersRepository = usersRepository;
            _passwordHashService = passwordHashService;
            _userValidator = userValidator;
            _updateLoginValidator = updateLoginValidator;
            _updatePasswordValidator = updatePasswordValidator;
            _updateDetailsValidator = updateDetailsValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<GetUserDto>>> GetActiveUsers()
        {
            var users = (await _usersRepository.GetUsers())
                .Where(users => !users.RevokedOn.HasValue);
            return new Result<List<GetUserDto>> { IsSuccess = true, Value = _mapper.Map<List<GetUserDto>>(users) };
        }

        public async Task<Result<List<GetUserDto>>> GetUsersByAge(int age)
        {
            var users = (await _usersRepository.GetUsers())
                .Where(user => user.Birthday.HasValue && (DateTime.UtcNow - user.Birthday.Value)
                        .TotalDays / 365.25 > age);
            return new Result<List<GetUserDto>> { IsSuccess = true, Value = _mapper.Map<List<GetUserDto>>(users) };
        }

        public async Task<Result<GetUserDetailsDto>> GetUserByLogin(string login)
        {
            var user = await _usersRepository.GetUser(login);
            if (user != null)
            {
                return new Result<GetUserDetailsDto> { IsSuccess = true, Value = _mapper.Map<GetUserDetailsDto>(user) };

            }
            var ex = new NotFoundException(login);
            _logger.LogError(ex, "An error occurred while getting user by login: {Login}", login);
            return new Result<GetUserDetailsDto> { IsSuccess = false, Exception = ex };

        }

        public async Task<Result<GetUserDetailsDto>> GetCurrentUser(string login)
        {
            var user = await _usersRepository.GetUser(login);
            if (user == null)
            {
                var ex = new NotFoundException(login);
                _logger.LogError(ex, "An error occurred while getting user by login: {Login}", login);
                return new Result<GetUserDetailsDto> { IsSuccess = false, Exception = ex };
            }
            if (user.RevokedOn.HasValue)
            {
                var ex = new AccessDeniedException(login);
                _logger.LogError(ex, "An error occurred while getting user by login: {Login}", login);
                return new Result<GetUserDetailsDto> { IsSuccess = false, Exception = ex };
            }
            return new Result<GetUserDetailsDto> { IsSuccess = true, Value = _mapper.Map<GetUserDetailsDto>(user) };
        }

        public async Task<Result<string>> AddUser(AddUserDto userModel, string currentUserLogin)
        {
            try
            {
                _userValidator.Validate(userModel);
                var user = _mapper.Map<User>(userModel);
                ConfigureAdd(user, currentUserLogin);
                await _usersRepository.AddUser(user);
                return new Result<string> {IsSuccess = true, Value = userModel.Login};
            }
            catch (DbUpdateException)
            {
                var ex = new LoginExistsException(userModel.Login);
                _logger.LogError(ex, "An error occurred while adding user: {Login}", userModel.Login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user: {Login}", userModel.Login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
        }

        private void ConfigureAdd(User user, string currentUserLogin)
        {
            user.CreatedOn = DateTime.UtcNow;
            user.CreatedBy = currentUserLogin;
            user.Password = _passwordHashService.HashPassword(user.Password);
        }

        public async Task<Result<string>> UpdatePassword(string login, string newPassword, UserWithPermissions currentUser)
        {
            var updateModel = new UpdatePasswordDto
            {
                Login = login,
                NewPassword = newPassword,
                ModifiedBy = currentUser.Login,
                ModifiedOn = DateTime.UtcNow
            };

            var validationResult = await _updatePasswordValidator.ValidateAsync(updateModel);
            if (!validationResult.IsValid)
            {
                var ex = new ValidationException(validationResult.Errors);
                _logger.LogError(ex, "An error occurred while updating user password: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }

            updateModel.NewPassword = _passwordHashService.HashPassword(updateModel.NewPassword);

            try
            {
                await _usersRepository.UpdatePassword(updateModel, currentUser);
                return new Result<string> { IsSuccess = true, Value = login };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user login: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
        }

        public async Task<Result<string>> UpdateLogin(string login, string newLogin, UserWithPermissions currentUser)
        {
            var updateModel = new UpdateLoginDto
            {
                Login = login,
                NewLogin = newLogin,
                ModifiedBy = currentUser.Login,
                ModifiedOn = DateTime.UtcNow
            };

            var validationResult = await _updateLoginValidator.ValidateAsync(updateModel);
            if (!validationResult.IsValid)
            {   
                var ex = new ValidationException(validationResult.Errors);
                _logger.LogError(ex, "An error occurred while updating user login: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }

            try
            {
                await _usersRepository.UpdateLogin(updateModel, currentUser);
                return new Result<string> { IsSuccess = true, Value = newLogin };
            }
            catch (DbUpdateException)
            {
                var ex = new LoginExistsException(newLogin);
                _logger.LogError(ex, "An error occurred while updating user login: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user login: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
        }

        public async Task<Result<string>> UpdateDetails(string login, JsonPatchDocument patchDocument, UserWithPermissions currentUser)
        {

            var model = new UpdateDetailsDto();
            patchDocument.ApplyTo(model);

            var validationResult = await _updateDetailsValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {   
                var ex = new ValidationException(validationResult.Errors);
                _logger.LogError(ex, "An error occurred while updating user details: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }

            try
            {
                await _usersRepository.UpdateDetails(login, patchDocument, currentUser);
                return new Result<string> { IsSuccess = true, Value = login };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user details: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
        }

        public async Task<Result<string>> DeleteUserSoft(string login, string revokedBy)
        {
            var revokedrecords = await _usersRepository.DeleteUserSoft(login, revokedBy);

            if (revokedrecords == 0)
            {
                var ex = new NotFoundException(login);
                _logger.LogError(ex, "An error occurred while softly deleting user: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
            return new Result<string> { IsSuccess = true, Value = login };
        }

        public async Task<Result<string>> DeleteUserFull(string login)
        {
            var deletedRecords = await _usersRepository.DeleteUserFull(login);

            if (deletedRecords == 0)
            {
                var ex = new NotFoundException(login);
                _logger.LogError(ex, "An error occurred while fully deleting user: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
            return new Result<string> { IsSuccess = true, Value = login };
        }

        public async Task<Result<string>> RestoreUser(string login)
        {
            var restoredRecords = await _usersRepository.RestoreUser(login);

            if (restoredRecords == 0)
            {
                var ex = new NotFoundException(login);
                _logger.LogError(ex, "An error occurred while restoring user: {Login}", login);
                return new Result<string> { IsSuccess = false, Exception = ex };
            }
            return new Result<string> { IsSuccess = true, Value = login };
        }
    }
}
