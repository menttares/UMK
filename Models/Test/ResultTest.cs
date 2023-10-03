using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models;


public class ResultTest
{
    public int Id { get; set; }
    public string NameStydent { get; set; }
    public string NameGroup { get; set; }
    public int TestId { get; set; }
    public Test Test { get; set; }
    public int Mark { get; set; }
}
