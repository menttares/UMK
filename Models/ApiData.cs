using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models
{

    public record TestResponse(Test Test, List<QuestionsTest> Questions);

    public record QuestionsTest(Questioin Question, List<Option> Options);
}