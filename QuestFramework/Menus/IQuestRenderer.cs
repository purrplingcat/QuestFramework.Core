using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Quests;

namespace QuestFramework.Menus
{
    public interface IQuestRenderer
    {
        void Draw(SpriteBatch spriteBatch);
        void Initialize(IQuest quest, CustomQuestLog menu);
        void Cleanup();
        void Update(GameTime time);
    }
}
