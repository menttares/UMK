using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models;

/// <summary>
/// Представление одной записи в разделе
/// </summary>
public class Knowledge
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description {get; set;}
    
    public bool IsAccess {get; set;} = true;
    public int TypeKnowledgeId {get; set;}

    public TypeKnowledge TypeKnowledge {get; set;}

}
