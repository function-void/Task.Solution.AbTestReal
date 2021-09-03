﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_WebSolution.Context.Model;
using Task_WebSolution.Context.Repositories;
using Task_WebSolution.Context.Validators;
using Task_WebSolution.Models;

namespace Task_WebSolution.Controllers
{
    [Route("[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(UserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        // PATCH : users
        [HttpPatch]
        public async Task<ActionResult> UpdateDatesForUsersAsync([FromBody] List<UserDto> fullDataUsers)
        {
            if (ModelState.IsValid)
            {
                await _userRepository
                        .MultiSaveAsync(fullDataUsers
                            .Select(x => _mapper.Map<User>(x)));

                return NoContent();
            }

            return BadRequest(fullDataUsers);
        }

        // GET : users
        [HttpGet]
        public ActionResult<List<UserDto>> GetUsersWithoutDate()
        {
            return _userRepository.GetAll()
                .Where(x => x.DateRegistration is null && x.DateLastActivity is null)
                    .Take(10)
                        .Select(x => _mapper.Map<UserDto>(x))
                            .ToList();
        }

        // GET : users/full
        [HttpGet("full")]
        public ActionResult<List<UserDto>> GetUsersWithFullInfo()
        {
            return _userRepository.GetAll()
                .Where(x => x.DateRegistration is not null && x.DateLastActivity is not null)
                    .Select(x => _mapper.Map<UserDto>(x))
                        .ToList();
        }

        // DELETE : users/5
        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            var user = _userRepository.Get(id);

            return user is null 
                ? NotFound(user)
                : _userRepository.Remove(user)
                    ? StatusCode(StatusCodes.Status204NoContent)
                    : StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
