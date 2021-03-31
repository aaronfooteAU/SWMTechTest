using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using SWMTechTest.Common.Services;
using SWMTechTest.Extensions;
using SWMTechTest.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWMTechTest.Controllers
{
    [Route("/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IUsersService _usersService;
        public ILogger<UsersController> _logger;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all users one page at a time")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([SwaggerParameter(Description = "Page number, >=1", Required = true)] int pageNumber,
                                                                          [SwaggerParameter(Description = "Page size, >=1", Required = true)] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning($"Invalid request to GetAllUsers, pageNumber <= 0 || pageSize <= 0.  Received pageNumber: '{pageNumber}, pageSize: '{pageSize}'");
                return BadRequest($"pageNumber must be >= 1 and pageSize must be >=0. Received pageNumber: '{pageNumber}, pageSize: '{pageSize}'");
            }
            try
            {
                var users = await _usersService.GetAllUsers(pageNumber, pageSize).ConfigureAwait(false);
                return Ok(users.ToDto());
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured in GetAllUsers, Message: '{e.Message}'", e);
                throw;
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [SwaggerOperation(Summary = "Gets user by Id")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserDto>> GetUserById([SwaggerParameter(Description = "Users Id", Required = true)] int id)
        {
            try
            {
                var user = await _usersService.GetUserById(id).ConfigureAwait(false);
                if (user == null)
                    return NotFound();

                return Ok(user.ToDto());
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured in GetUserById, Message: '{e.Message}'", e);
                throw;
            }
        }

        [HttpGet]
        [Route("search")]
        [SwaggerOperation(Summary = "Gets users matching a supplied age")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByAge([SwaggerParameter(Description = "Users age", Required = true)] int age)
        {
            try
            {
                var users = await _usersService.GetUsersByAge(age).ConfigureAwait(false);
                return Ok(users.ToDto());
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured in GetUsersByAge, Message: '{e.Message}'", e);
                throw;
            }
        }
    }
}