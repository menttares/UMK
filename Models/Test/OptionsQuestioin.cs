using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace УМК.Models
{
    public class OptionsQuestioin
    {
        public int Id {get; set;}

        public int OptionId {get; set;}
        public Option Option {get; set;}

        public Questioin Questioin {get; set;}

        public int QuestioinId {get; set;}
        
        /// <summary>
        /// Правильный ответ
        /// </summary>
        public bool IsTry {get; set;}
    }
}