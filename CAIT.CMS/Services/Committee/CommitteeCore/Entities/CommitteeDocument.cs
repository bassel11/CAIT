using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Entities
{
    public class CommitteeDocument
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }          // PDF, Word, Excel, etc.
        public string FilePath { get; set; }          // Storage path
        public int Version { get; set; }              // Version tracking
    }
}
