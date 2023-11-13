using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QuestObjectiveAttribute : Attribute
    {
        public string? Name { get; set; }

        public QuestObjectiveAttribute() 
        { 
        }

        public QuestObjectiveAttribute(string name)
        {
            Name = name;
        }
    }
}
