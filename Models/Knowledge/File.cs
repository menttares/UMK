using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models;

public enum TypeFile
{
    doc,
    xlsx,
    txt,
    cs,
    none
}

public class File
{
    public int Id { get; set; }
    public string? Name {get; set;}
    public string? Type {get; set;}
    
}
