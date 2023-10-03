using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models;

public class KnowledgeFiles
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public File File { get; set; }

    public int KnowledgeId {get; set;}
    
    public Knowledge Knowledge {get; set;}

}
