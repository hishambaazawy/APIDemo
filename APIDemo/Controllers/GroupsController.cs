using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiData.Entities;
using ApiData.Entities.Data;
using APIDemo.Services;
using ApiData.Shared;
using Microsoft.Extensions.Options;
using ApiData.Shared.Model;
using Microsoft.AspNetCore.Authorization;

namespace APIDemo.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("api/public/v{version:apiVersion}/[controller]")]
    [Authorize(Roles ="Admin,User")]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> logger;
        private readonly LogsService logsService;
        private readonly SmartChargingContext _context;

        public GroupsController(SmartChargingContext context, LogsService logs, ILogger<GroupsController> llogger)
        {
            _context = context;
            logger = llogger;
            logsService = logs;
        }

        [HttpGet("GetGroups")]
        public async Task<ActionResult<GenericResponse<IEnumerable<Group>>>> GetGroups()
        {
            return new GenericResponse<IEnumerable<Group>>() { Success=true, Response= await _context.Groups.ToListAsync(), Message= "Success", StatusCode= StatusCodeEnum.Success };
        }
        [HttpGet("GetGroupsWithFullDetails")]
        public async Task<ActionResult<GenericResponse<IEnumerable<Group>>>> GetGroupsWithFullDetails()
        {
            return new GenericResponse<IEnumerable<Group>>() { Success = true, Response = await _context.Groups.Include(x=>x.ChargeStations).ThenInclude(x=>x.Connectors).ToListAsync(), Message = "Success", StatusCode = StatusCodeEnum.Success };
        }
       
        [HttpGet("GetGroupById")]
        public async Task<ActionResult<GenericResponse<Group>>> GetGroupById(int id)
        {
            var Tatgetgroup = await _context.Groups.FindAsync(id);
            if (Tatgetgroup == null)
            {
                return new GenericResponse<Group>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            return new GenericResponse<Group>() { Message= "Success", Success=true, StatusCode= StatusCodeEnum.Success, Response = Tatgetgroup };
        }

        [HttpGet("GetGroupWithFullDetails")]
        public async Task<ActionResult<GenericResponse<Group>>> GetGroupWithFullDetails(int id)
        {
            var result = await _context.Groups.Include(x => x.ChargeStations).ThenInclude(x => x.Connectors).FirstOrDefaultAsync(x => x.GroupId == id);
            if (result == null)
            {
                return new GenericResponse<Group>() { Message = "Not Found", Success = true, StatusCode = StatusCodeEnum.Warning, Response = null };
            }
            else
            {
                return new GenericResponse<Group>() { Success = true, Response = result, Message = "Success", StatusCode = StatusCodeEnum.Success };
            }
           
        }

        [HttpPost("AddGroup")]
        public async Task<GenericResponse<Group>> AddGroup([FromBody] GroupModel NewGroup)
        {
            if (NewGroup != null)
            {
                if (string.IsNullOrEmpty(NewGroup.Name) || NewGroup.CapacityInAmps <= 0)
                {
                    return new GenericResponse<Group>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                }
                else
                {
                    var group=new Group() { Name=NewGroup.Name, CapacityInAmps=NewGroup.CapacityInAmps };   
                    await _context.Groups.AddAsync(group); 
                    await _context.SaveChangesAsync();
                    return new GenericResponse<Group>() { Message = "Success", Success = true, StatusCode = StatusCodeEnum.Success, Response = group };
                }
            }
            else
            {
                return new GenericResponse<Group>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }

        [HttpPut("UpdateGroup")]
        public async Task<GenericResponse<Group>> UpdateGroup(int id, GroupModel groupInput)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var group = await _context.Groups.Include(x => x.ChargeStations).ThenInclude(x => x.Connectors).FirstOrDefaultAsync(x => x.GroupId == id);
                if (group == null)
                {
                    return new GenericResponse<Group>() { Message = "Invalid Id", Success = false, StatusCode = StatusCodeEnum.Warning, Response = null };
                }
                else
                {
                    if (string.IsNullOrEmpty(groupInput.Name) || groupInput.CapacityInAmps <= 0)
                    {
                        return new GenericResponse<Group>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                    }
                    int SumMaxAmps = 0;
                    foreach(var charge in  group.ChargeStations) 
                    {
                        SumMaxAmps += charge.Connectors.Sum(x => x.MaxCurrentInAmps);
                    }
                    if (groupInput.CapacityInAmps >= SumMaxAmps)
                    {
                        group.Name=groupInput.Name;
                        group.CapacityInAmps = groupInput.CapacityInAmps;
                        _context.Groups.Update(group);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new GenericResponse<Group>() { Message = "Success", Success =true, StatusCode = StatusCodeEnum.Success, Response = group };
                    }
                    else
                    {
                        return new GenericResponse<Group>() { Message = "Invalid Input", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
                    }
                }
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                logger.LogError("Error Update Group", exp);
                logsService.TraceError(exp);
                return new GenericResponse<Group>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = null };
            }
        }

        
        [HttpDelete("DeleteGroup")]
        public async Task<GenericResponse<bool>> DeleteGroup(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var group = await _context.Groups.Include(x => x.ChargeStations).ThenInclude(x => x.Connectors).FirstOrDefaultAsync(x => x.GroupId == id);
                if (group == null)
                {
                    return new GenericResponse<bool>() { Message = "Invalid Id", Success = false, StatusCode = StatusCodeEnum.Warning, Response = false };
                }
                else
                {
                    foreach (var chargeStation in group.ChargeStations)
                    {
                        _context.Connectors.RemoveRange(chargeStation.Connectors);
                    }
                    _context.ChargeStations.RemoveRange(group.ChargeStations);
                    _context.Groups.Remove(group);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new GenericResponse<bool>() { Message = "Deleted", Success = true, StatusCode = StatusCodeEnum.Success, Response = true };
                }
            }
            catch (Exception exp)
            {
               await transaction.RollbackAsync();
                logger.LogError("Error Delete Group",exp);
                logsService.TraceError(exp);    
                return new GenericResponse<bool>() { Message = "System Error", Success = false, StatusCode = StatusCodeEnum.Error, Response = false };
            }
            
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
