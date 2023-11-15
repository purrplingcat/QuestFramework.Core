using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestMetadata
    {
        public string QualifiedId { get; }
        public string LocalId { get; }
        public string TypeIdentifier { get; }
        public int Seed { get; }
    }
}
