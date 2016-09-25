using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameFile.Models
{
    class MyFileInfo
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public string LastWriteTime { get; set; }
    }
}
