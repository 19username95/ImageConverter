using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ImagesConverter.Models
{
    public class StandardApiDbContext : DbContext
    {
        public DbSet<ImageFile> ImageFiles { get; set; }
    }
}