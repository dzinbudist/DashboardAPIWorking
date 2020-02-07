using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;

namespace DashBoard.Business.Services
{
    public interface IDomainService
    {
        DomainModelDto GetById(int id);
        IEnumerable<DomainModelDto> GetAllNotDeleted();
        DomainModelDto Create(DomainForCreationDto domain);
        object Update(int id, DomainForUpdateDto domain);
        object PseudoDelete(int id);
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
        public DomainModelDto Create(DomainForCreationDto domain)
        {
            var domainEntity = _mapper.Map<DomainModel>(domain);

            // dbr default poto:
            //domainEntity.Created_By = MiscFunctions.GetCurentUser(this.User);

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

        public IEnumerable<DomainModelDto> GetAllNotDeleted()
        {
            try
            {
                var domains = _context.Domains.Where(x => x.Deleted == false).ToList();
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

        public DomainModelDto GetById(int id)
        {
            var domain = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted==false);
            if (domain == null)
            {
                return null;
            }
            var domainDto = _mapper.Map<DomainModelDto>(domain);
            return domainDto;
        }

        public object PseudoDelete(int id)
        {

            var domainModel = _context.Domains.FirstOrDefault(x=>x.Id ==id && x.Deleted==false);
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

        public object Update(int id, DomainForUpdateDto domain)
        {
            var updatedModel = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted==false);
            if (updatedModel == null)
            {
                return null;
            }

            //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);    
            updatedModel.Date_Modified = DateTime.Now;
            _mapper.Map(domain, updatedModel);
            _context.Domains.Update(updatedModel);
            _context.SaveChanges();
            return new { message = $"Updated domain with {id} id" }; //client doesn't see this. It's just for something to return.
        }
    }
}
