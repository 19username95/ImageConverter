using ImagesConverter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ImagesConverter.Services
{
    public class ImagesDbService
    {
        private readonly StandardApiDbContext _context;

        public ImagesDbService(StandardApiDbContext context)
        {
            this._context = context;
        }

        public Task<List<ImageFile>> LoadImagesAsync(IEnumerable<int> ids = null)
        {
            return (ids == null || !ids.Any()) ?
                _context.ImageFiles.ToListAsync() :
                _context.ImageFiles.Where(_ => ids.Contains(_.Id)).ToListAsync();
        }

        public async Task AddImageAsync(ImageFile entity)
        {
            _context.ImageFiles.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}