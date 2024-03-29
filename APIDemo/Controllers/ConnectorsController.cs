﻿using System;
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
using ApiData.Shared;
using ApiData.Shared.Model;
using static System.Collections.Specialized.BitVector32;

namespace APIDemo.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("api/public/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,User")]
    public class ConnectorsController : ControllerBase
    {
        private readonly ILogger<ConnectorsController> logger;
        private readonly LogsService logsService;
        private readonly SmartChargingContext _context;
        private readonly MyHelper helper;
        public ConnectorsController(SmartChargingContext context, LogsService logs, ILogger<ConnectorsController> llogger, MyHelper _helper)
        {
            _context = context;
            logger = llogger;
            logsService = logs;
            helper = _helper;
        }

        [HttpGet("GetConnectors")]
        public async Task<GenericResponse<IEnumerable<Connector>>> GetConnectors()
        {
            return new GenericResponse<IEnumerable<Connector>>() { Success = true, Response = await _context.Connectors.ToListAsync(), Message = "Success", StatusCode = StatusCodeEnum.Success };
        }
        [HttpGet("GetConnectorById")]
        public async Task<GenericResponse<Connector>> GetConnectorById(int ConnectorId, int ChargeStationId)
        {
            var TatgetConnector = await _context.Connectors.FindAsync(ConnectorId, ChargeStationId);
            if (TatgetConnector == null)
            {
                return new GenericResponse<Connector>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            return new GenericResponse<Connector>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = TatgetConnector };
        }
        [HttpGet("GetConnectorByChargeStationId")]
        public async Task<GenericResponse<IEnumerable<Connector>>> GetConnectorByChargeStationId(int ChargeStationId)
        {
            var TatgetConnector = await _context.Connectors.Where(x => x.ChargeStationId == ChargeStationId).ToListAsync();
            if (TatgetConnector == null)
            {
                return new GenericResponse<IEnumerable<Connector>>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            return new GenericResponse<IEnumerable<Connector>>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = TatgetConnector };
        }
        [HttpPost("AddConnector")]
        public async Task<GenericResponse<Connector>> AddConnector(ConnectorModel connectorInput)
        {
            try
            {
                if (connectorInput == null || connectorInput.MaxCurrentInAmps <= 0 || connectorInput.ChargeStationId <= 0)
                {
                    return new GenericResponse<Connector>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }
                var targetStation = await _context.ChargeStations.Where(x => x.ChargeStationId == connectorInput.ChargeStationId).Include(x => x.Connectors).Include(x => x.Group).AsNoTracking().FirstOrDefaultAsync();
                if (targetStation == null)
                {
                    return new GenericResponse<Connector>() { Message = "Invalid ChargeStation", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }
                else if (targetStation.Connectors.Count() >= 5)
                {
                    return new GenericResponse<Connector>() { Message = "ChargeStation Is Full", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }
                else
                {
                    if (await helper.GetGroupConnectorAmps(targetStation.GroupId) + connectorInput.MaxCurrentInAmps <= targetStation.Group.CapacityInAmps)
                    {
                        var ids = await _context.Connectors.Where(x => x.ChargeStationId == connectorInput.ChargeStationId).ToListAsync();
                        var Max = 1;
                        if(ids.Any()) {Max=ids.Select(x=>x.ConnectorId).Max() + 1;}
                        var connector = new Connector()
                        {
                            ConnectorId = Max,
                            ChargeStationId = targetStation.ChargeStationId,
                            MaxCurrentInAmps = connectorInput.MaxCurrentInAmps,
                            Reference = connectorInput.Reference,
                            CreatedBy = User.Identity.Name ?? "*",
                            CreationDate = DateTime.Now
                        };
                        await _context.Connectors.AddAsync(connector);
                        await _context.SaveChangesAsync();
                        return new GenericResponse<Connector>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = connector };

                    }
                    else
                    {
                        return new GenericResponse<Connector>() { Message = "Invalid Amps", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                    }
                }

            }
            catch (Exception exp)
            {
                logger.LogError("Error Add Connector", exp);
                logsService.TraceError(exp);
                return new GenericResponse<Connector>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }
        [HttpPut("UpdateConnector")]
        public async Task<GenericResponse<Connector>> UpdateConnector(int ConnectorId, int ChargeStationId, ConnectorModel connectorInput)
        {

            if (ConnectorId == 0 || ChargeStationId == 0 || connectorInput == null )
            {
                return new GenericResponse<Connector>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            var target = await _context.Connectors.Include(x=>x.ChargeStation).ThenInclude(x=>x.Group).FirstOrDefaultAsync(x => x.ConnectorId == ConnectorId && x.ChargeStationId == ChargeStationId);
            if (connectorInput.MaxCurrentInAmps <= 0 || target == null)
            {
                return new GenericResponse<Connector>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if ((await helper.GetGroupConnectorAmps(target.ChargeStation.GroupId) - target.MaxCurrentInAmps) + connectorInput.MaxCurrentInAmps <= target.ChargeStation.Group.CapacityInAmps)
                {
                    target.MaxCurrentInAmps = connectorInput.MaxCurrentInAmps;
                    target.Reference = connectorInput.Reference;
                    _context.Connectors.Update(target);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new GenericResponse<Connector>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = target };
                }
                else
                {
                    return new GenericResponse<Connector>() { Message = "Invalid Input (Amps value)", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }

            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                logger.LogError("Error Update Connector", exp);
                logsService.TraceError(exp);
                return new GenericResponse<Connector>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }
        [HttpDelete("DeleteConnector")]
        public async Task<GenericResponse<bool>> DeleteConnector(int ConnectorId, int ChargeStationId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var connector = await _context.Connectors.FirstOrDefaultAsync(x => x.ChargeStationId == ChargeStationId && x.ConnectorId == ConnectorId);
                if (connector == null)
                {
                    return new GenericResponse<bool>() { Message = "Invalid Id", Success = false, StatusCode = StatusCodeEnum.Warning, Response = false };
                }
                else
                {
                    _context.Connectors.Remove(connector);
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
