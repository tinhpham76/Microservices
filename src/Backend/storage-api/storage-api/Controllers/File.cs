using System;

namespace storage_api.Controllers
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }

        public long FileSize { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
