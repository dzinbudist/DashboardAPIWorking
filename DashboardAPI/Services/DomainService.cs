using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IDomainService
    {
        DomainModel GetById(int id);
        IEnumerable<DomainModel> GetAllNotDeleted();
        DomainModel Create(DomainModel domainModel);
        DomainModel Update(int id, DomainModel domainModel);
        DomainModel PseudoDelete(int id);
    }
    public class DomainService : IDomainService
    {
        private DataContext _context;
        public DomainService(DataContext context)
        {
            _context = context;
        }
        public DomainModel Create(DomainModel domainModel)
        {
            DomainModel newModel = domainModel;
            newModel.Created_By = default; // dbr default poto:
            //newModel.Created_By = MiscFunctions.GetCurentUser(this.User);
            newModel.Modified_By = default;
            newModel.Last_Fail = default;
            newModel.Date_Created = DateTime.Now;
            newModel.Date_Modified = DateTime.Now;

            _context.Domains.Add(newModel);
            //if(newModel.Id != default) // jei per postmana, bando atsiust ID. Padaryta del grazaus response, nes bet kokiu atveju i DB neideda i Identity column.
            try
            {
               _context.SaveChanges();
            }
            catch(Exception)
            {
                return null; 
            }
            
            return newModel;
        }

        public IEnumerable<DomainModel> GetAllNotDeleted()
        {
            return _context.Domains.Where(x => x.Deleted == false).ToList();
        }

        public DomainModel GetById(int id)
        {
            DomainModel domainModel = _context.Domains.Find(id);
            return domainModel;
        }

        public DomainModel PseudoDelete(int id)
        {

            DomainModel domainModel = _context.Domains.Find(id);
            if(domainModel == null)
            {
                return null;
            }
            //DomainModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            domainModel.Date_Modified = DateTime.Now;
            domainModel.Deleted = true;
            _context.SaveChanges(); // kazkaip veikia, bet keista, nes mes kitam objektui pridedam viska.
            return domainModel;
        }

        public DomainModel Update(int id, DomainModel domainModel)
        {
            var updatedModel = _context.Domains.Find(id);
            //galima su automapper nugetu graziai surasyti sintaxe.
            if (updatedModel.Service_Name != domainModel.Service_Name)
            {
                updatedModel.Service_Name = domainModel.Service_Name;
            }
            if (updatedModel.Url != domainModel.Url)
            {
                updatedModel.Url = domainModel.Url;
            }
            if (updatedModel.Service_Type != domainModel.Service_Type)
            {
                updatedModel.Service_Type = domainModel.Service_Type;
            }
            if (updatedModel.Method != domainModel.Method)
            {
                updatedModel.Method = domainModel.Method;
            }
            if (updatedModel.Basic_Auth != domainModel.Basic_Auth)
            {
                updatedModel.Basic_Auth = domainModel.Basic_Auth;
                if (updatedModel.Basic_Auth == false)
                {
                    updatedModel.Auth_User = null;
                    updatedModel.Auth_Password = null;
                }
                updatedModel.Auth_User = domainModel.Auth_User;
                updatedModel.Auth_Password = domainModel.Auth_Password;
            }
            if (updatedModel.Parameters != domainModel.Parameters)
            {
                updatedModel.Parameters = domainModel.Parameters;
            }
            if (updatedModel.Notification_Email != domainModel.Notification_Email)
            {
                updatedModel.Notification_Email = domainModel.Notification_Email;
            }
            if (updatedModel.Interval_Ms != domainModel.Interval_Ms)
            {
                updatedModel.Interval_Ms = domainModel.Interval_Ms;
            }
            if (updatedModel.Active != domainModel.Active)
            {
                updatedModel.Active = domainModel.Active;
            }
            //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);    
            updatedModel.Date_Modified = DateTime.Now;

            _context.Entry(updatedModel).State = EntityState.Modified; //ar reikia sitos eilutes?
            _context.Domains.Update(updatedModel);
            _context.SaveChanges();
            return updatedModel;
        }
    }
}
