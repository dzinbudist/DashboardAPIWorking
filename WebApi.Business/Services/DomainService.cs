using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Business.DTOs.Domains;
using WebApi.Data.Data;
using WebApi.Data.Entities;

namespace WebApi.Business.Services
{
    public interface IDomainService
    {
        DomainModelDto GetById(int id);
        IEnumerable<DomainModelDto> GetAllNotDeleted();
        void Create(DomainForCreationDto domain);
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
        public void Create(DomainForCreationDto domain)
        {
            var domainEntity = _mapper.Map<DomainModel>(domain);

            // dbr default poto:
            //domainEntity.Created_By = MiscFunctions.GetCurentUser(this.User);


            _context.Domains.Add(domainEntity);
            //if(newModel.Id != default) // jei per postmana, bando atsiust ID. Padaryta del grazaus response, nes bet kokiu atveju i DB neideda i Identity column.
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public IEnumerable<DomainModelDto> GetAllNotDeleted()
        {
            var domains = _context.Domains.Where(x => x.Deleted == false).ToList();
            var domainsDto = _mapper.Map<IEnumerable<DomainModelDto>>(domains);
            return domainsDto;
        }

        public DomainModelDto GetById(int id)
        {
            var domain = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted==false);
            var domainDto = _mapper.Map<DomainModelDto>(domain);
            return domainDto;
        }

        public object PseudoDelete(int id)
        {

            var domainModel = _context.Domains.Find(id);
            if (domainModel == null)
            {
                return null;
            }
            //DomainModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            domainModel.Date_Modified = DateTime.Now;
            domainModel.Deleted = true;
            _context.SaveChanges(); // kazkaip veikia, bet keista, nes mes kitam objektui pridedam viska.
            return new { message = $"Domain with {id} id deleted" };
        }

        public object Update(int id, DomainForUpdateDto domain)
        {
            var updatedModel = _context.Domains.Find(id);
            if (updatedModel == null)
            {
                return null;
            }

            //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);    
            updatedModel.Date_Modified = DateTime.Now;
            _mapper.Map(domain, updatedModel);
            _context.Domains.Update(updatedModel);
            _context.SaveChanges();
            return new { message = $"Updated domain with {id} id" };
        }
    }
}
