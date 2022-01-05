using Ewallet.Dtos.Photo;
using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<Tuple<bool, UploadPhotoDto>> UploadPhotoAsync(UploadPhotoDto model, string userId);
        Task<Tuple<bool, UploadPhotoDto>> AddPhotoAsync(UploadPhotoDto model, string userId);
        Task<List<Photo>> GetUserPhotosAsync(string userId);
        Task<Photo> GetUserMainPhotoAsync(string userId);
        Task<Tuple<bool, string>> SetMainPhotoAsync(string userId, string PublicId);
        Task<bool> UnSetMainPhotoAsync(string userId);
        Task<bool> DeletePhotoAsync(string PublicId);
    }
}
