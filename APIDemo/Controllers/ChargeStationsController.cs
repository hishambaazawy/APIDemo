using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiData.Entities;
using ApiData.Entities.Data;
using Microsoft.AspNetCore.Authorization;
using APIDemo.Services;
using ApiData.Shared.Model;
using ApiData.Shared;

namespace APIDemo.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("api/public/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,User")]
    public class ChargeStationsController : ControllerBase
    {
        private readonly ILogger<ChargeStationsController> logger;
        private readonly LogsService logsService;
        private readonly SmartChargingContext _context;
        private readonly MyHelper helper;
        public ChargeStationsController(SmartChargingContext context, LogsService logs, ILogger<ChargeStationsController> llogger, MyHelper _helper) 
        {
            _context = context;
            logger = llogger;
            logsService = logs;
            helper = _helper;
        }
        [HttpGet("GetChargeStations")]
        public async Task<GenericResponse<IEnumerable<ChargeStation>>> GetChargeStations()
        {
            return new GenericResponse<IEnumerable<ChargeStation>>() { Success = true, Response = await _context.ChargeStations.ToListAsync(), Message = "Success", StatusCode = StatusCodeEnum.Success };
        }
        [HttpGet("GetChargeStationWithFullDetails")]
        public async Task<GenericResponse<IEnumerable<ChargeStation>>> GetChargeStationWithFullDetails()
        {
            return new GenericResponse<IEnumerable<ChargeStation>>() { Success = true, Response = await _context.ChargeStations.Include(x => x.Connectors).ToListAsync(), Message = "Success", StatusCode = StatusCodeEnum.Success };
        }

        [HttpGet("GetChargeStationById")]
        public async Task<GenericResponse<ChargeStation>> GetChargeStationById(int id)
        {
            var TatgetChargeStation = await _context.ChargeStations.FindAsync(id);
            if (TatgetChargeStation == null)
            {
                return new GenericResponse<ChargeStation>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = TatgetChargeStation };
        }

        [HttpGet("GetChargeStationWithFullDetailsById")]
        public async Task<GenericResponse<ChargeStation>> GetChargeStationWithFullDetailsById(int id)
        {
            var result = await _context.ChargeStations.Include(x => x.Connectors).FirstOrDefaultAsync(x => x.GroupId == id);
            if (result == null)
            {
                return new GenericResponse<ChargeStation>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            else
            {
                return new GenericResponse<ChargeStation>() { Success = true, Response = result, Message = "Success", StatusCode = StatusCodeEnum.Success };
            }

        }

        [HttpPost("AddChargeStation")]
        public async Task<GenericResponse<ChargeStation>> AddChargeStation([FromBody] ChargeStationModel newStation)
        {
            if (newStation != null)
            {
                if (string.IsNullOrEmpty(newStation.Name) || await _context.Groups.FindAsync(newStation.GroupId)==null)
                {
                    return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                }
                else
                {
                    var station = new ChargeStation() {GroupId=newStation.GroupId, Name = newStation.Name, CreationDate = DateTime.Now, CreatedBy = User.Identity.Name ?? "*" };
                    await _context.ChargeStations.AddAsync(station);
                    await _context.SaveChangesAsync();
                    return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = station };
                }
            }
            else
            {
                return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }

        [HttpPost("AddChargeStationWithConnector")]
        public async Task<GenericResponse<ChargeStation>> AddChargeStationWithConnector([FromBody] ChargeStationModel newStation)
        {
            if (newStation != null)
            {
                var group = await _context.Groups.FindAsync(newStation.GroupId);
                if (string.IsNullOrEmpty(newStation.Name) || group == null || newStation.Connector==null || newStation.Connector==null)
                {
                    return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                }
                else
                {
                    if (newStation.Connector.MaxCurrentInAmps <= 0 || (group.CapacityInAmps>=(await helper.GetGroupConnectorAmps(group.GroupId) + newStation.Connector.MaxCurrentInAmps)) ) 
                    {
                        return new GenericResponse<ChargeStation>() { Message = "Invalid Connector", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                    }
                    var station = new ChargeStation() { GroupId = newStation.GroupId, Name = newStation.Name, CreationDate = DateTime.Now, CreatedBy = User.Identity.Name ?? "*" };
                    station.Connectors.Add(new Connector() { MaxCurrentInAmps = newStation.Connector.MaxCurrentInAmps, CreatedBy = station.CreatedBy, CreationDate = station.CreationDate, Reference = newStation.Connector.Reference });
                    await _context.ChargeStations.AddAsync(station);
                    await _context.SaveChangesAsync();
                    return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = station };
                }
            }
            else
            {
                return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }

        [HttpPut("UpdateChargeStation")]
        public async Task<GenericResponse<ChargeStation>> UpdateChargeStation(int id, ChargeStationModel stationInput)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var station = await _context.ChargeStations.Include(x => x.Connectors).Include(x=>x.Group).FirstOrDefaultAsync(x => x.ChargeStationId == id);
                var group= await _context.Groups.FindAsync(stationInput.GroupId);
                if (station == null || group==null || string.IsNullOrEmpty(stationInput.Name))
                {
                    return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }
                else
                {
                    if (station.GroupId == stationInput.GroupId)
                    {
                        station.Name = stationInput.Name;
                        _context.ChargeStations.Update(station);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = station };
                    }
                    else if (station.GroupId != stationInput.GroupId && station.Connectors.Count() == 0)
                    {
                        station.Name = stationInput.Name;
                        station.GroupId = stationInput.GroupId;
                        _context.ChargeStations.Update(station);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = station };
                    }
                    else 
                    {
                        var sumGroupStationConnectors =  await helper.GetGroupConnectorAmps(stationInput.GroupId);
                        var sumCurrentStationConnector = station.Connectors.Select(x => x.MaxCurrentInAmps).Sum();
                        if ((group.CapacityInAmps - (sumGroupStationConnectors + sumCurrentStationConnector)) >= 0)
                        {
                            station.Name = stationInput.Name;
                            station.GroupId = stationInput.GroupId;
                            _context.ChargeStations.Update(station);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return new GenericResponse<ChargeStation>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = station };

                        }
                        else
                        {
                            return new GenericResponse<ChargeStation>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                logger.LogError("Error Update Charge Station", exp);
                logsService.TraceError(exp);
                return new GenericResponse<ChargeStation>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }

        [HttpDelete("DeleteChargeStation")]
        public async Task<GenericResponse<bool>> DeleteChargeStation(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var station = await _context.ChargeStations.Include(x => x.Connectors).FirstOrDefaultAsync(x =>x.ChargeStationId == id);
                if (station == null)
                {
                    return new GenericResponse<bool>() { Message = "Invalid Id", Success = false, StatusCode = StatusCodeEnum.Warning, Response = false };
                }
                else
                {
                    _context.Connectors.RemoveRange(station.Connectors);
                    _context.ChargeStations.Remove(station);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new GenericResponse<bool>() { Message = "Deleted", Success = true, StatusCode = StatusCodeEnum.Success, Response = true };
                }
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                logger.LogError("Error Delete ChargeStation", exp);
                logsService.TraceError(exp);
                return new GenericResponse<bool>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = false };
            }

        }

    }
}
