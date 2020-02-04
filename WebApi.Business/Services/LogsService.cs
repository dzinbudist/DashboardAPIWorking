using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using WebApi.Business.DTOs.Logs;
using WebApi.Data.Data;

namespace WebApi.Business.Services
{

    public interface ILogsService
    {
        IEnumerable<LogModelDto> GetAllLogs();
        IEnumerable<LogModelDto> GetLogsByDomainId(int id);
    }
    public class LogsService : ILogsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LogsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<LogModelDto> GetAllLogs()
        {
            var logs = _context.Logs.ToList();
            var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
            return logsDto;
        }

        public IEnumerable<LogModelDto> GetLogsByDomainId(int id)
        {
            var logs = _context.Logs.Where(x => x.Domain_Id == id).ToList();
            var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
            return logsDto;
        }
    }
}
