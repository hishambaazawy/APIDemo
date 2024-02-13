using ApiData.Entities;
using APIDemo.Controllers;
using Microsoft.EntityFrameworkCore;

namespace APIDemo.Services
{
    public class MyHelper
    {
        private readonly ILogger<MyHelper> logger;
        private readonly LogsService logsService;
        private readonly SmartChargingContext _context;
        public MyHelper(SmartChargingContext context, LogsService logs, ILogger<MyHelper> llogger)
        {
            _context = context;
            logger = llogger;
            logsService = logs;
        }

        public async Task<int> GetGroupConnectorAmps(int groupId)
        {
            try
            {
                var sumGroupStationConnectors = 0;
                var listStation = await _context.ChargeStations.Include(x => x.Connectors).Where(x => x.GroupId == groupId).ToListAsync();
                foreach (var chargeStation in listStation)
                {
                    sumGroupStationConnectors += chargeStation.Connectors.Select(x => x.MaxCurrentInAmps).Sum();
                }
                return sumGroupStationConnectors;
            }
            catch (Exception exp)
            {
                logger.LogError(exp.Message,exp);
                logsService.TraceError(exp);
                return 0;
            }
        }
    }
}
