using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestRenderer
    {
        void Draw(SpriteBatch spriteBatch, ICustomQuest quest, IQuestMenu menu);
    }
}
