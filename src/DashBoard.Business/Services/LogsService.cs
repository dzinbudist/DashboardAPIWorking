using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DashBoard.Business.DTOs.Logs;
using DashBoard.Data.Data;

namespace DashBoard.Business.Services
{

    public interface ILogsService
    {
        IEnumerable<LogModelDto> GetAllLogs(string userId);
        IEnumerable<LogModelDto> GetLogsByDomainId(int id, string userId, int count);
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
        public IEnumerable<LogModelDto> GetAllLogs(string userId)
        {
            var userMakingThisRequest = _context.Users.Find(Convert.ToInt32(userId));
            var teamKey = userMakingThisRequest.Team_Key;
            var logs = _context.Logs.Where(x => x.Team_Key == teamKey).ToList();
            var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
            return logsDto.Any() ? logsDto : null;
        }

        public IEnumerable<LogModelDto> GetLogsByDomainId(int id, string userId, int count)
        {
            var userMakingThisRequest = _context.Users.Find(Convert.ToInt32(userId));
            var teamKey = userMakingThisRequest.Team_Key;

            if (count > 0)
            {
                var logs = _context.Logs.Where(x => x.Domain_Id == id && x.Team_Key == teamKey).OrderByDescending(x => x.Id).Take(count).ToList();
                var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
                return logsDto.Any() ? logsDto : null;
            }
            else
            {
                var logs = _context.Logs.Where(x => x.Domain_Id == id && x.Team_Key == teamKey).OrderByDescending(x => x.Id).ToList();
                var logsDto = _mapper.Map<IEnumerable<LogModelDto>>(logs);
                return logsDto.Any() ? logsDto : null;
            }
        }
    }
}
