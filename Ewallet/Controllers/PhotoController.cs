﻿using AutoMapper;
using CloudinaryDotNet.Actions;
using Ewallet.Commons;
using Ewallet.Dtos.Photo;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public PhotoController(IMapper mapper, IPhotoService photoService)
        {
            _mapper = mapper;
            _photoService = photoService;
        }


        // this route is only open to logged-in users
        // api/Photos/add-photo?userId=d274fa2f-201a-41f9-8a89-f17c4f07544d
        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto([FromForm] UploadPhotoDto model, string userId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var file = model.Photo;

            if (file.Length > 0)
            {
                var uploadStatus = await _photoService.UploadPhotoAsync(model, userId);

                if (uploadStatus.Item1)
                {
                    var res = await _photoService.AddPhotoAsync(model, userId);
                    if (!res.Item1)
                    {
                        ModelState.AddModelError("Failed", "Could not add photo to database");
                        return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Failed to add to database", ModelState, null));
                    }

                    return Ok(Util.BuildResponse<object>(true, "Uploaded successfully", null, new { res.Item2.PublicId, res.Item2.Url }));
                }

                ModelState.AddModelError("Failed", "File could not be uploaded to cloudinary");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Failed to upload", ModelState, null));

            }

            ModelState.AddModelError("Invalid", "File size must not be empty");
            return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "File is empty", ModelState, null));

        }

        [HttpGet("get-user-photos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPhotos(string userId)
        {
            var photos = await _photoService.GetUserPhotosAsync(userId);
            if (photos == null)
            {
                ModelState.AddModelError("Not found", "No result found for photos");
                return NotFound(Util.BuildResponse<ImageUploadResult>(false, "Result is empty", ModelState, null));
            }

            // map result
            var listOfUsers = new List<GetPhotoDto>();
            foreach (var photo in photos)
            {
                var photosToReturn = _mapper.Map<GetPhotoDto>(photo);
                listOfUsers.Add(photosToReturn);

            }

            return Ok(Util.BuildResponse<List<GetPhotoDto>>(true, "List of user's photos", null, listOfUsers));
        }

        [HttpGet("get-user-main-photo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserMainPhoto(string userId)
        {
            var photo = await _photoService.GetUserMainPhotoAsync(userId);
            if (photo == null)
            {
                ModelState.AddModelError("Not found", "No result found for main photo");
                return NotFound(Util.BuildResponse<ImageUploadResult>(false, "Result is empty", ModelState, null));
            }

            // map result
            var photosToReturn = _mapper.Map<GetPhotoDto>(photo);


            return Ok(Util.BuildResponse<GetPhotoDto>(true, "User's main photo", null, photosToReturn));
        }


        [HttpPatch("set-main-photo/{publicId}")]
        public async Task<IActionResult> SetMainPhoto(string userId, string publicId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _photoService.SetMainPhotoAsync(userId, publicId);
            if (!res.Item1)
            {
                ModelState.AddModelError("Failed", "Could not set main photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Set main failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Main photo is set sucessfully!", null, res.Item2));

        }

        [HttpPatch("unset-main-photo")]
        public async Task<IActionResult> UnsetMainPhoto(string userId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _photoService.UnSetMainPhotoAsync(userId);
            if (!res)
            {
                ModelState.AddModelError("Failed", "Could not unset main photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Unset failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Unset main photo is sucessful!", null, ""));

        }

        [HttpDelete("delete-photo/{publicId}")]
        public async Task<IActionResult> DeletePhoto(string userId, string publicId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _photoService.DeletePhotoAsync(publicId);
            if (!res)
            {
                ModelState.AddModelError("Failed", "Could not delete photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Delete failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Photo deleted sucessful!", null, ""));

        }
    }
}
