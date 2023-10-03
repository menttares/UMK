using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models;


public class TestQuestioins
{
    public int Id { get; set; }
    
    public Questioin Questioin {get; set;}

    public int QuestioinId {get; set;}

    public Test Test {get; set;}

    public int TestId {get; set;}

}
