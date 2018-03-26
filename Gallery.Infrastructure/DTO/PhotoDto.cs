using System;

namespace Gallery.Infrastructure.DTO
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
