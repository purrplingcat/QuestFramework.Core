using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API.Exceptions
{
    public class QuestException : Exception
    {
        public QuestException(string? message) : base(message)
        {
        }
    }

    public class QuestCreationException : QuestException 
    {
        public QuestCreationException(string questId) : base($"Unable to create quest with ID '´{questId}'")
        {
        }

        public QuestCreationException(string questId, string reason) : base($"Unable to create quest with ID '´{questId}': {reason}")
        {
        }
    }
}
