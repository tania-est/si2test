﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using si2.bll.Dtos.Requests.Administration;
using si2.bll.Dtos.Results.Administration;
using si2.dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace si2.api.Controllers
{
    [ApiController]
    [Route("api/administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(ILogger<AdministrationController> logger,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        [HttpGet("users")]
        public IActionResult GetUsers(CancellationToken ct)
        {
            var Users = _userManager.Users;

            if (Users == null)
                return NotFound();

            return Ok(Users);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model, CancellationToken ct)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole() { Name = model.RoleName });

            if (result.Succeeded)
            {
                var roleToReturn = _roleManager.Roles.FirstOrDefault(c => c.Name == model.RoleName);
                return CreatedAtRoute("GetRole", new { id = roleToReturn.Id }, roleToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("roles/{id}", Name = "GetRole")]
        public async Task<IActionResult> GetRoleById([FromRoute]string id, CancellationToken ct)
        {
            var Role = await _roleManager.FindByIdAsync(id);

            if (Role == null)
                return NotFound();

            return Ok(Role);
        }

        [HttpGet("roles")]
        public IActionResult GetRoles(CancellationToken ct)
        {
            var Roles = _roleManager.Roles;

            if (Roles == null)
                return NotFound();

            return Ok(Roles);
        }


        [HttpPost("users/{userId}/claims")]
        public async Task<IActionResult> ManageUserClaims([FromRoute]string userId, [FromBody] UserClaimsDto model, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, existingClaims);

            if (!result.Succeeded)
                return BadRequest(); // TODO Bad request is not the best returned error 

            var claims = model.Claims.Select(c => new Claim(c.ClaimType, c.ClaimType));

            result = await _userManager.AddClaimsAsync(user, claims);
            if (!result.Succeeded)
                return BadRequest(); // TODO Bad request is not the best returned error 

            return CreatedAtRoute("GetUserClaims", new { userId = userId }, model);
        }

        [HttpGet("users/{userId}/claims", Name = "GetUserClaims")]
        public async Task<IActionResult> GetUserClaims([FromRoute]string userId, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var claims = existingClaims.Select(c => new UserClaimDto() { ClaimType = c.Type, IsSelected = true });

            var result = new UserClaimsDto()
            {
                Claims = claims.ToList()
            };

            return Ok(result);
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetRolesForUser([FromRoute]string userId, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AddRolesToUser([FromRoute]string userId, [FromBody]RolesDto addRoles, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId); 
            
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
            await _userManager.AddToRolesAsync(user, addRoles.Roles.ToArray());

            var finalRoles = await _userManager.GetRolesAsync(user);

            return Ok(finalRoles);
        }
    }
}
