using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DashBoard.Business.Services
{
    public interface IDomainService
    {
        DomainModelDto GetById(int id, string userId);
        IEnumerable<DomainModelDto> GetAllNotDeleted(string userId);
        Task<DomainModelDto> Create(DomainForCreationDto domain, string userId);
        object Update(int id, DomainForUpdateDto domain, string userId);
        object PseudoDelete(int id, string userId);
    }
    public class DomainService : IDomainService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public DomainService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //random uses of async methods. No idea when it's appropriate and when it's not
        public async Task<DomainModelDto> Create(DomainForCreationDto domain, string userId)
        {
            var domainEntity = _mapper.Map<DomainModel>(domain);
            var loggedInUser = await GetUserById(userId);

            domainEntity.Created_By = loggedInUser.Id;

            domainEntity.Team_Key = loggedInUser.Team_Key;

            _context.Domains.Add(domainEntity);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }

            var domainDto = _mapper.Map<DomainModelDto>(domainEntity);
            return domainDto;
        }

        public IEnumerable<DomainModelDto> GetAllNotDeleted(string userId)
        {
            try
            {
                var user = GetUserById(userId);
                var teamKey = user.Result.Team_Key; //what's the difference if i use async method and await for Task<> here?
                //gets domains that are not deleted and belongs to user team.
                var domains = _context.Domains.Where(x => x.Deleted == false && x.Team_Key == teamKey).ToList();
                if (!domains.Any())
                {
                    return null;
                }
                var domainsDto = _mapper.Map<IEnumerable<DomainModelDto>>(domains);
                return domainsDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public DomainModelDto GetById(int id, string userId)
        {
            var user = GetUserById(userId);
            var teamKey = user.Result.Team_Key;
            var domain = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted==false && x.Team_Key == teamKey);
            if (domain == null)
            {
                return null;
            }
            var domainDto = _mapper.Map<DomainModelDto>(domain);
            return domainDto;
        }

        public object PseudoDelete(int id, string userId)
        {
            var user = GetUserById(userId);

            var teamKey = user.Result.Team_Key;

            var domainModel = _context.Domains.FirstOrDefault(x=>x.Id ==id && x.Deleted==false && x.Team_Key == teamKey);
            if (domainModel == null)
            {
                return null;
            }
            //DomainModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            domainModel.Date_Modified = DateTime.Now;
            domainModel.Deleted = true;
            _context.SaveChanges(); 
            return new { message = $"Domain with {id} id deleted" }; //client doesn't see this. It's just for something to return.

        }

        public object Update(int id, DomainForUpdateDto domain, string userId)
        {   
            var user = GetUserById(userId);
            var teamKey = user.Result.Team_Key;
            var updatedModel = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted==false && x.Team_Key == teamKey);
            if (updatedModel == null)
            {
                return null;
            }

            updatedModel.Modified_By = user.Result.Id; 
            updatedModel.Date_Modified = DateTime.Now;
            _mapper.Map(domain, updatedModel);
            _context.Domains.Update(updatedModel);
            _context.SaveChanges();
            return new { message = $"Updated domain with {id} id" }; //client doesn't see this. It's just for something to return.
        }

        //private helper methods.
        private async Task<User> GetUserById(string userId)
        {
            //this takes string, because controller passes identity name(id) from claim, which is string.
            var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
            return user;
        }
    }
}
