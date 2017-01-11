using NetworkService.Contracts.Models.Interfaces;

namespace NetworkService.Contracts.Models
{
    public class FileModel : IFile
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
    }
}
