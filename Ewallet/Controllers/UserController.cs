using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.User;
using Ewallet.Helpers;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IWalletService _walletService;

        public UserController(IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWalletService walletService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _walletService = walletService;
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(AddUserDto model)
        {
            // if user already exist return early
            var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmailUser != null)
            {
                ModelState.AddModelError("Invalid", $"User with email: {model.Email} already exists");
                return BadRequest(Util.BuildResponse<object>(false, "User already exists!", ModelState, null));
            }

            // map data from model to user
            var user = _mapper.Map<User>(model);
            //user.Address.Street = model.Street;
            //user.Address.State = model.State;
            //user.Address.Country = model.Country;


            var response = await _userManager.CreateAsync(user, model.Password);

            if (!response.Succeeded)
            {
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<string>(false, "Failed to add user!", ModelState, null));
            }

            var res = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (!res.Succeeded)
            {
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<string>(false, "Failed to add user role!", ModelState, null));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("ConfirmEmail", "User", new { Email = user.Email, Token = token }, Request.Scheme);  // this is the url to send

            // next thing TODO here is to send an email to this new user to the email provided using a notification service you should build

            string confirmationMessage = $"<p>Hi {user.FirstName},</p>" +
                $"<p>Before you can login to your account, you need to verify your email. <a href='{url}'>Click here to verify.</a></p><br/>" +
                $"<p>Best regards,<br/>Merlin</p>" +
                $"<p>Can't see the link for verifying your email? Here it is:</p>" +
                $"<p><a href='{url}'>{url}</a></p>" +
                $"<p><b>Note:</b> You are receiving this email because it's important and it's not something that you can unsubscribe from.</p>";
            
            var message = new Message(new List<EmailConfiguration>{ new EmailConfiguration { DisplayName = user.FirstName, From = "no_reply@gmail.com", Address = user.Email } }, "Email Confirmation", confirmationMessage);
            
            var mailResponse = await _emailSender.SendEmailAsync(message);

            if (!mailResponse) await _userManager.DeleteAsync(user);

            // map data to dto
            var userRoles = await _userManager.GetRolesAsync(user);
            var details = _mapper.Map<GetUserDto>(user);
            details.Roles = userRoles;

            // the confirmation link is added to this response object for testing purpose since at this point it is not being sent via mail
            return Ok(Util.BuildResponse(true, "New user added!", null, new { details, ConfimationLink = url }));


        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetUsers(int page, int perPage)
        {
            List<GetUserDto> listOfUsers = new List<GetUserDto>();

            var users = await _userManager.Users.ToListAsync();

            if (users != null)
            {
                var pagedList = PageList<User>.Paginate(users, page, perPage);
                foreach (var user in pagedList.Data)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var mapped = _mapper.Map<GetUserDto>(user);
                    mapped.Roles = userRoles;
                    listOfUsers.Add(mapped);
                }

                var res = new PaginatedListDto<GetUserDto>
                {
                    MetaData = pagedList.MetaData,
                    Data = listOfUsers
                };

                return Ok(Util.BuildResponse(true, "List of users", null, res));
            }
            else
            {
                ModelState.AddModelError("Notfound", "There was no record for users found!");
                var res = Util.BuildResponse<List<GetUserDto>>(false, "No results found!", ModelState, null);
                return NotFound(res);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-user-by-email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            GetUserDto getUser = new GetUserDto();

            User user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var mapped = _mapper.Map<GetUserDto>(user);
                mapped.Roles = userRoles;
                getUser = mapped;

                var res = Util.BuildResponse(true, "User details", null, getUser);
                return Ok(res);
            }
            else
            {
                ModelState.AddModelError("Notfound", $"There was no record found for user with email {user.Email}");
                return NotFound(Util.BuildResponse<List<GetUserDto>>(false, "No result found!", ModelState, null));
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-user-by-id/")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            GetUserDto getUser = new GetUserDto();

            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var mapped = _mapper.Map<GetUserDto>(user);
                mapped.Roles = userRoles;
                getUser = mapped;

                var res = Util.BuildResponse(true, "User details", null, getUser);
                return Ok(res);
            }
            else
            {
                ModelState.AddModelError("Notfound", $"There was no record found for user with Id {user.Id}");
                return NotFound(Util.BuildResponse<List<GetUserDto>>(false, "No result found!", ModelState, null));
            }
        }

        [Authorize]
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto model)
        {
            // check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            string role = currentUser.FindFirst(ClaimTypes.Role).Value;
            if (!role.Equals("Admin") && !id.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to edit another user's details");
                return BadRequest(Util.BuildResponse<string>(false, "Access Denied!", ModelState, ""));
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", $"User with id: {id} was not found");
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }            
            else
            {   
                if (ModelState.IsValid)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;

                    var userRoles = await _userManager.GetRolesAsync(user);

                    IdentityResult res = await _userManager.UpdateAsync(user);

                    if (!res.Succeeded)
                    {
                        foreach (var err in res.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                        return BadRequest(Util.BuildResponse<object>(false, "Failed to update user", ModelState, null));
                    }

                    var mapped = _mapper.Map<GetUserDto>(user);
                    mapped.Roles = userRoles;
                    GetUserDto updatedUser = mapped;

                    return Ok(Util.BuildResponse<object>(true, "User updated successfully!", null, updatedUser));
                }
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to update user", ModelState, ""));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var res = await _userManager.DeleteAsync(user);

                if (!res.Succeeded)
                {
                    foreach (var err in res.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                }

                return Ok(Util.BuildResponse<object>(true, "User deleted successfully!", null, ""));
            }

            ModelState.AddModelError("Notfound", "There was no record for users found!");
            return NotFound(Util.BuildResponse<string>(false, $"User with id {id} not found", ModelState, null));
        }

        [Authorize(Roles = "Admin")]
        [Route("assign-role-to-user/{id:guid}/roles")]
        [HttpPut]
        public async Task<ActionResult> AssignRolesToUser(string id, [FromBody] string[] rolesToAssign)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", $"User with id: {id} was not found");
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesNotExists = rolesToAssign.Except(_roleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {
                ModelState.AddModelError("NotFound", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }

            if (currentRoles.Equals("Elite"))
            {
                await _walletService.MergeWallets(user.Id);
            }

            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                foreach (var err in removeResult.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<object>(false, "Failed to remove user roles", ModelState, null));
            }

            IdentityResult addResult = await _userManager.AddToRolesAsync(user, rolesToAssign);

            if (!addResult.Succeeded)
            {
                foreach (var err in removeResult.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<object>(false, "Failed to add user roles", ModelState, null));
            }

            var mapped = _mapper.Map<GetUserDto>(user);
            mapped.Roles = currentRoles;
            GetUserDto updatedUser = mapped;

            return Ok(Util.BuildResponse<object>(true, "User roles added successfully!", null, updatedUser));
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError("Invalid", "UserId and token is required");
                return BadRequest(Util.BuildResponse<object>(false, "UserId or token is empty!", ModelState, null));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("NotFound", $"User with email: {email} was not found");
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }

            var res = await _userManager.ConfirmEmailAsync(user, token);
            if (!res.Succeeded)
            {
                foreach (var err in res.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<object>(false, "Failed to confirm email", ModelState, null));
            }

            return Ok(Util.BuildResponse<object>(true, "Email confirmation suceeded!", null, user));
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(currentUserId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult res = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!res.Succeeded)
            {
                return BadRequest(Util.BuildResponse<object>(false, "Failed to change password", ModelState, null));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var mapped = _mapper.Map<GetUserDto>(user);
            mapped.Roles = userRoles;
            GetUserDto updatedUser = mapped;

            return Ok(Util.BuildResponse<object>(true, "Password changed sucessfully!", null, updatedUser));
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "User", new { email = model.Email, token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)) }, Request.Scheme);

                    string confirmationMessage = $"<p>Hi {user.FirstName},</p>" +
                    $"<p>We understand you have a lot going on on your mind, feel free to reset your password. <a href='{url}'>Click here to reset password.</a></p><br/>" +
                    $"<p>Best regards,<br/>Merlin</p>" +
                    $"<p>Can't see the link for resetting your password? Here it is:</p>" +
                    $"<p><a href='{url}'>{url}</a></p>" +
                    $"<p><b>Note:</b> You are receiving this email because it's important and it's not something that you can unsubscribe from.</p>";

                    var message = new Message(new List<EmailConfiguration> { new EmailConfiguration { DisplayName = user.FirstName, From = "no_reply@gmail.com", Address = user.Email } }, "Forgot Password", confirmationMessage);
                    await _emailSender.SendEmailAsync(message);

                    // the confirmation link is added to this response object for testing purpose since at this point it is not being sent via mail
                    return Ok(Util.BuildResponse(true, "Password reset link sent!", null, new { PaswordResetLink = url }));
                }

                ModelState.AddModelError("Invalid", $"User with email: {model.Email} does not exists");
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }

            ModelState.AddModelError("Failed", "Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to reset password", ModelState, ""));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if(model.Token == null || model.Email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
                return BadRequest(Util.BuildResponse<string>(false, "Unable to reset password", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    User user = await _userManager.FindByEmailAsync(model.Email);
                    if(user != null)
                    {
                        var res = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token)), model.Password);
                        if (!res.Succeeded)
                        {
                            foreach(var err in res.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }

                            return BadRequest(Util.BuildResponse<string>(false, "Failed to reset password!", ModelState, null));
                        }

                        var userRoles = await _userManager.GetRolesAsync(user);
                        var mapped = _mapper.Map<GetUserDto>(user);
                        mapped.Roles = userRoles;
                        GetUserDto updatedUser = mapped;

                        return Ok(Util.BuildResponse<object>(true, "Password reset sucessfully!", null, updatedUser));
                    }
                }

                ModelState.AddModelError("Failed", "Invalid payload");
                return BadRequest(Util.BuildResponse<string>(false, "Unable to reset password", ModelState, ""));
            }
        }
        
    }
}
