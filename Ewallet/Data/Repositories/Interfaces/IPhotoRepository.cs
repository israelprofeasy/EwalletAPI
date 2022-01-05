using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Interfaces
{
    public interface IPhotoRepository : ICRUDRepository
    {
        Task<List<Photo>> GetPhotos();
        Task<Photo> GetPhotoByPublicId(string PublicId);
        Task<List<Photo>> GetPhotosByUserId(string UserId);
    }
}
