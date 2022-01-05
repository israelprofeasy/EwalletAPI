using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Role;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> Add(AddRoleDto model)
        {
            ResponseDto<List<GetRoleDto>> res = new ResponseDto<List<GetRoleDto>>();

            List<GetRoleDto> listOfRoles = new List<GetRoleDto>();
            if (ModelState.IsValid)
            {
                IdentityRole newRole = _mapper.Map<IdentityRole>(model);

                List<IdentityRole> roles = await _roleService.Add(newRole);

                if (roles.Count > 0)
                {
                    foreach (var role in roles)
                    {
                        listOfRoles.Add(_mapper.Map<GetRoleDto>(role));
                    }

                    res.Status = true;
                    res.Message = "Role added successfully!";
                    res.Data = listOfRoles;
                    return Ok(res);
                }
                else
                {
                    ModelState.AddModelError("Failed", $"Invalid payload");
                    return BadRequest(Util.BuildResponse<string>(false, "Unable to add new role", ModelState, ""));
                }                

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to add new role", ModelState, ""));

        }

        [HttpGet("get-roles")]
        public IActionResult Get()
        {
            ResponseDto<List<GetRoleDto>> res = new ResponseDto<List<GetRoleDto>>();
            List<GetRoleDto> listOfRoles = new List<GetRoleDto>();

            List<IdentityRole> roles = _roleService.GetRoles();

            if (roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    listOfRoles.Add(_mapper.Map<GetRoleDto>(role));
                }
                res.Status = true;
                res.Message = "List of roles";
                res.Data = listOfRoles;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve roles", ModelState, ""));
        }

        [HttpGet("get-role-by-id/{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            ResponseDto<GetRoleDto> res = new ResponseDto<GetRoleDto>();
            GetRoleDto model = new GetRoleDto();

            IdentityRole role = await _roleService.GetRoleById(id);

            if (role != null)
            {
                model = _mapper.Map<GetRoleDto>(role);

                res.Status = true;
                res.Message = "Role details";
                res.Data = model;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve role", ModelState, ""));
        }

        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> Update(string id, UpdateRoleDto model)
        {
            ResponseDto<GetRoleDto> res = new ResponseDto<GetRoleDto>();

            IdentityRole role = await _roleService.GetRoleById(id);

            if (role == null)
            {
                ModelState.AddModelError("Denied", $"Role does not exist");
                return BadRequest(Util.BuildResponse<string>(false, "Access Denied!", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {

                    IdentityRole updateRole = new IdentityRole
                    {
                        Name = model.Name
                    };


                    IdentityRole response = await _roleService.Update(id, updateRole);

                    if (response != null)
                    {
                        GetRoleDto updatedRole = _mapper.Map<GetRoleDto>(response);

                        res.Status = true;
                        res.Message = "Role updated";
                        res.Data = updatedRole;
                    }

                    return Ok(res);
                }
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to update role", ModelState, ""));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseDto<GetRoleDto> res = new ResponseDto<GetRoleDto>();
            bool status = await _roleService.Delete(id);

            if (status)
            {
                res.Status = true;
                res.Message = "Role Deleted";

                return Ok(res);

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to delete role", ModelState, ""));
        }
    }
}
