using System;
using System.Collections.Generic;

namespace B2b.Web.Models
{
    public class FileExplorerViewModel
    {
        public string CurrentPath { get; set; } = string.Empty;
        public List<FolderItem> Folders { get; set; } = new List<FolderItem>();
        public List<FileItem> Files { get; set; } = new List<FileItem>();
    }

    public class FolderItem
    {
        public string Name { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }

    public class FileItem
    {
        public string Name { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime LastModified { get; set; }

        // Boyutu kullanıcı dostu formata çeviren yardımcı property (örn: 1.5 MB)
        public string FormattedSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = SizeInBytes;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}