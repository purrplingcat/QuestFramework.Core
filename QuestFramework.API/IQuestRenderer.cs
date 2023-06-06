using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestRenderer
    {
        int Priority { get; }
        bool ShouldRenderQuest(ICustomQuest quest);
        void Draw(SpriteBatch spriteBatch, ICustomQuest quest, IQuestMenu menu);
    }
}
