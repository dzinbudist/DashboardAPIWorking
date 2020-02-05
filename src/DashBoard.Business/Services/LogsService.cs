using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DashBoard.Business.DTOs.Logs;
using DashBoard.Data.Data;

namespace DashBoard.Business.Services
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
            return !logsDto.Any() ? null : logsDto;
        }

        public IEnumerable<LogModelDto> GetLogsByDomainId(int id)
        {
            var logs = _context.Logs.Where(x => x.Domain_Id == id).ToList();
            var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
            return !logsDto.Any() ? null : logsDto;
        }
    }
}
