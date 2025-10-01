using Committee.Core.Common;

namespace Committee.Core.Entities
{
    public class CommitteeDocument : EntityBase
    {
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }          // PDF, Word, Excel, etc.
        public string FilePath { get; set; }          // Storage path
        public int Version { get; set; }              // Version tracking
    }
}
