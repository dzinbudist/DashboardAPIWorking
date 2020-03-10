using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DashBoard.Business.CustomExceptions;
using DashBoard.Business.DTOs.Users;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;

namespace DashBoard.Business.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<UserModelDto> GetAll(string userId);
        UserModelDto GetById(int id, string userId);
        User Create(RegisterModelDto model, string password, string userId);
        string Update(int id, UpdateModelDto model, string userId);
        string Delete(int id, string userId);
        Task<User> GetUserModel(string userId);
        Task<bool> UpdateUserRefreshToken(User user, string refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        private readonly string validPasswordPattern =
            "^(?=\\S*[a-z])(?=\\S*[A-Z])(?=\\S*\\d)(?=\\S*[\\W_])\\S{10,128}$";
            //senesnis regex: "^(?=\\S*[a-z])(?=\\S*[A-Z])(?=\\S*\\d)(?=\\S*[!\"#$%&'()*+,./:;<=>?@[\\\\\\]^_`{|}~-])\\S{10,128}$";
        public UserService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<UserModelDto> GetAll(string userId)
        {
            var userMakingThisRequest = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
            var teamKey = userMakingThisRequest.Team_Key;
            var users = _context.Users.Where(x => x.Team_Key == teamKey);
            var usersDto = _mapper.Map<IList<UserModelDto>>(users);
            return usersDto;
        }

        public UserModelDto GetById(int id, string userId)
        {
            var userMakingThisRequest = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
            var teamKey = userMakingThisRequest.Team_Key;
            var user = _context.Users.FirstOrDefault(c => c.Id == id && c.Team_Key == teamKey);  //you can find other team IDs. I didn't change it here, because you need to reconfig JWT token.
            
            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserModelDto>(user);
            return userDto;
        }
        public User Create(RegisterModelDto model, string password, string userId)
        {
            
            var user = _mapper.Map<User>(model); //map model to entity. Ar created by admin nusifiltruos cia ?

            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
            if (password != model.ConfirmPassword)
            {
                throw new AppException("Passwords do not match");
            }
            //check if password is strong enough
            if (!Regex.IsMatch(password, validPasswordPattern))
            {
                throw new AppException("Passwords must be 10 to 128 characters long and contain: upper case (A - Z), lower case (a - z), number(0 - 9) and special character(e.g. !@#$%^&*)");
            }

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            if (model.CreatedByAdmin)
            {
                //admin thats making this request
                var admin = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
                user.Created_By = admin.Id;
                user.Modified_By = admin.Id;
                user.Role = Role.User;
                user.Team_Key = admin.Team_Key;
            }
            else
            {
                user.Role = Role.Admin;
                user.Team_Key = Guid.NewGuid();
            }
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Date_Modified = DateTime.Now;
            user.Date_Created = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();

            return user; // method returns entity model, but it's not used in controller. So no need for DTO here.
        }
        public string Update(int id, UpdateModelDto model, string userId)
        {

            //cia nera password checkinngo !!!

            //user that's making this update

            var userMakingThisUpdate = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
            var teamKey = userMakingThisUpdate.Team_Key;

            //find excising user to update in Database
            var user = _context.Users.FirstOrDefault(x => x.Id == id && x.Team_Key == teamKey);

            if (user == null)
                throw new AppException("User not found");
            
            //check if user doesn't try to update other users.
            if (!userMakingThisUpdate.Role.Equals("Admin") && userMakingThisUpdate.Id != user.Id)
            {
                throw new AppException("You can't update this user");
            }

            // map model to entity
            var password = model.Password; //get user sent password, before mapping.
            var modelWithNewParams = _mapper.Map<User>(model);

            //password validation
            if (password != model.ConfirmPassword)
            {
                throw new AppException("Passwords do not match");
            }
            //check if password is strong enough
            if (!Regex.IsMatch(password, validPasswordPattern))
            {
                throw new AppException("Passwords must be 10 to 128 characters long and contain: upper case (A - Z), lower case (a - z), number(0 - 9) and special character(e.g. !@#$%^&*)");
            }

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(modelWithNewParams.Username) && modelWithNewParams.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == modelWithNewParams.Username))
                    throw new AppException("Username " + modelWithNewParams.Username + " is already taken");

                user.Username = modelWithNewParams.Username;
            }

            // update email if it has changed
            if (!string.IsNullOrWhiteSpace(modelWithNewParams.UserEmail) && modelWithNewParams.UserEmail != user.UserEmail)
            {
                // throw error if the new email is already taken
                if (_context.Users.Any(x => x.UserEmail == modelWithNewParams.UserEmail))
                    throw new AppException("Email " + modelWithNewParams.UserEmail + " is already taken");

                user.UserEmail = modelWithNewParams.UserEmail;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(modelWithNewParams.FirstName))
                user.FirstName = modelWithNewParams.FirstName;

            if (!string.IsNullOrWhiteSpace(modelWithNewParams.LastName))
                user.LastName = modelWithNewParams.LastName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // update role if provided and check if user is admin.
            if (userMakingThisUpdate.Role.Equals("Admin") && model.Role != user.Role)
            {
                if (model.Role != "Admin" && model.Role != "User")
                {
                    throw new AppException("No such role: " + model.Role);
                }
                else if (userMakingThisUpdate.Id == user.Id)
                {
                    if (userMakingThisUpdate.Role == Role.Admin)
                    {
                        user.Role = Role.Admin;
                        return "notAllowed";
                    }
                }
                else
                {
                    user.Role = model.Role;
                }                
            }

            user.Date_Modified = DateTime.Now;
            user.Modified_By = userMakingThisUpdate.Id;

            _context.Users.Update(user);
            _context.SaveChanges();
            return "ok";
        }

        public string Delete(int id, string userId)
        {
            var userMakingThisDelete = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
            var teamKey = userMakingThisDelete.Team_Key;
            //check if there is such a user in team with such Id.
            var user = _context.Users.FirstOrDefault(x => x.Id == id && x.Team_Key == teamKey);

            if (user != null)
            {
                if (userMakingThisDelete.Id == user.Id)
                {
                    return "notAllowed";
                }
                else
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return "ok";
                }
            }
            return "notFound";
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public async Task<User> GetUserModel(string userId)
        {
            var user = _context.Users.First(c => c.Id == Convert.ToInt32(userId));

            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<bool> UpdateUserRefreshToken(User user, string refreshToken)
        {
            try
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenValidDate = DateTime.Now.AddDays(3);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}