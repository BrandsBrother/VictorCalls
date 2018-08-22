using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace LeadWomb.Model
{
    public class Document
    {
        public int DocumentID { get; set; }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        //public File ProjectFile { get; set; }
    }
}
