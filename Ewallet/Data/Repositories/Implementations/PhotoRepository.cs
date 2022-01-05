using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;

        public PhotoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Add<T>(T entity)
        {
            _context.Add(entity);
            return await SaveChanges();
        }

        public async Task<bool> Delete<T>(T entity)
        {
            _context.Remove(entity);
            return await SaveChanges();
        }

        public async Task<bool> Edit<T>(T entity)
        {
            _context.Update(entity);
            return await SaveChanges();
        }

        public async Task<Photo> GetPhotoByPublicId(string PublicId)
        {
            return await _context.Photos.Include(x => x.User).FirstOrDefaultAsync(x => x.PublicId == PublicId);
        }

        public async Task<List<Photo>> GetPhotos()
        {
            return await _context.Photos.ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosByUserId(string UserId)
        {
            return await _context.Photos.Where(x => x.UserId == UserId).ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
