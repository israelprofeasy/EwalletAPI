﻿using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Dtos.Photo;
using Ewallet.Helpers;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IOptions<CloudinarySettings> config,
            IMapper mapper, UserManager<User> userManager,
            IPhotoRepository photoRepository)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _mapper = mapper;
            _userManager = userManager;
            _photoRepository = photoRepository;
        }


        public async Task<Tuple<bool, UploadPhotoDto>> UploadPhotoAsync(UploadPhotoDto model, string userId)
        {
            var uploadResult = new ImageUploadResult();

            using (var stream = model.Photo.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.Photo.FileName, stream),
                    Transformation = new Transformation().Width(300).Height(300).Gravity("face").Crop("fill")
                };

                uploadResult = await _cloudinary.UploadAsync(imageUploadParams);
            }

            var status = uploadResult.StatusCode.ToString();

            if (status.Equals("OK"))
            {
                model.PublicId = uploadResult.PublicId;
                model.Url = uploadResult.Url.ToString();
                return new Tuple<bool, UploadPhotoDto>(true, model);
            }

            return new Tuple<bool, UploadPhotoDto>(false, model);

        }

        public async Task<Tuple<bool, UploadPhotoDto>> AddPhotoAsync(UploadPhotoDto model, string userId)
        {
            var user = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == userId);

            var photo = _mapper.Map<Photo>(model);
            photo.UserId = userId;

            if (!user.Photos.Any(x => x.IsMain))
                photo.IsMain = true;

            // add photo to database
            var res = await _photoRepository.Add(photo);

            return new Tuple<bool, UploadPhotoDto>(res, model);
        }

        public async Task<List<Photo>> GetUserPhotosAsync(string userId)
        {
            var res = await _photoRepository.GetPhotosByUserId(userId);
            if (res != null)
                return res;

            return null;
        }

        public async Task<Photo> GetUserMainPhotoAsync(string userId)
        {
            var res = await _photoRepository.GetPhotosByUserId(userId);

            var mainPhoto = res.FirstOrDefault(x => x.IsMain == true);

            if (mainPhoto != null)
                return mainPhoto;

            return null;
        }

        public async Task<Tuple<bool, string>> SetMainPhotoAsync(string userId, string PublicId)
        {
            var photos = await _photoRepository.GetPhotosByUserId(userId);
            if (photos != null)
            {
                this.UnsetMain(photos);

                var newMain = photos.FirstOrDefault(x => x.PublicId == PublicId);
                newMain.IsMain = true;

                // update database
                var res = await _photoRepository.Edit(newMain);
                if (res)
                    return new Tuple<bool, string>(true, newMain.Url);
            }

            return new Tuple<bool, string>(false, "");
        }

        public async Task<bool> UnSetMainPhotoAsync(string userId)
        {
            var photos = await _photoRepository.GetPhotosByUserId(userId);
            if (photos != null)
            {
                this.UnsetMain(photos);

                // update database
                var res = await _photoRepository.SaveChanges();
                if (res)
                    return true;
            }

            return false;
        }

        private void UnsetMain(List<Photo> photos)
        {
            if (photos.Any(x => x.IsMain))
            {
                // get the main photo and unset it
                var main = photos.FirstOrDefault(x => x.IsMain == true);
                main.IsMain = false;
            }
        }

        public async Task<bool> DeletePhotoAsync(string PublicId)
        {
            DeletionParams destroyParams = new DeletionParams(PublicId)
            {
                ResourceType = ResourceType.Image
            };

            DeletionResult destroyResult = _cloudinary.Destroy(destroyParams);

            if (destroyResult.StatusCode.ToString().Equals("OK"))
            {
                var photo = await _photoRepository.GetPhotoByPublicId(PublicId);
                if (photo != null)
                {
                    var res = await _photoRepository.Delete(photo);
                    if (res)
                        return true;
                }
            }

            return false;
        }
    }
}
