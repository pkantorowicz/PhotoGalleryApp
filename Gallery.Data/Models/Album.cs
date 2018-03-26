using System;
using System.Collections.Generic;

namespace Gallery.Data.Models
{
    public class Album : IEntityBase
    {
        public Album()
        {
            Photos = new List<Photo>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}
