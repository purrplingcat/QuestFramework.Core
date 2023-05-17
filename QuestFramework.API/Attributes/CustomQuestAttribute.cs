using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomQuestAttribute : Attribute
    {
        public string? Name { get; set; }

        public CustomQuestAttribute()
        {
        }

        public CustomQuestAttribute(string name) 
        {
            Name = name;
        }
    }
}
