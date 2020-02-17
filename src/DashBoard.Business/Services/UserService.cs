using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<UserModelDto> GetAll();
        UserModelDto GetById(int id);
        User Create(RegisterModelDto model, string password, string userId);
        void Update(int id, UpdateModelDto model, string userId);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
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

        public IEnumerable<UserModelDto> GetAll()
        {
            var users = _context.Users;
            var usersDto = _mapper.Map<IList<UserModelDto>>(users);
            return usersDto;
        }

        public UserModelDto GetById(int id)
        {
            var user = _context.Users.Find(id);
            var userDto = _mapper.Map<UserModelDto>(user);
            return userDto;
        }
        public User Create(RegisterModelDto model, string password, string userId)
        {
            var user = _mapper.Map<User>(model); //map model to entity. Ar created by admin nusifiltruos cia ?

            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            if (model.CreatedByAdmin)
            {
                var admin = _context.Users.First(c => c.Id == Convert.ToInt32(userId));
                user.Created_By = admin.Id;
                user.Modified_By = admin.Id;
                user.Role = Role.User;
                user.Team_Key = admin.Team_Key;
            }
            user.Role = Role.Admin; // Role.
            user.Team_Key = Guid.NewGuid();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Date_Modified = DateTime.Now;
            user.Date_Created = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();

            return user; // method returns entity model, but it's not used in controller. So no need for DTO here.
        }
        public void Update(int id, UpdateModelDto model, string userId)
        {
            // map model to entity
            var password = model.Password; //get user sent password, before mapping.
            var modelWithNewParams = _mapper.Map<User>(model);
            //find excising user
            var user = _context.Users.Find(id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(modelWithNewParams.Username) && modelWithNewParams.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == modelWithNewParams.Username))
                    throw new AppException("Username " + modelWithNewParams.Username + " is already taken");

                user.Username = modelWithNewParams.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(modelWithNewParams.FirstName))
                user.FirstName = modelWithNewParams.FirstName;

            if (!string.IsNullOrWhiteSpace(modelWithNewParams.LastName))
                user.LastName = modelWithNewParams.LastName;

            user.Date_Modified = DateTime.Now;
            if (model.UpdatedByAdmin)
            {
                var admin = _context.Users.First(c => c.Id == Convert.ToInt32(userId)); //pakeist
                user.Modified_By = admin.Id;
            }
            user.Modified_By = Convert.ToInt32(userId);

            //this is missing email update !!

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
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
    }
}