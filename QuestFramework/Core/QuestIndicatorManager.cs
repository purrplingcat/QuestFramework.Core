using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace QuestFramework.Core
{
    internal class QuestIndicatorManager
    {
        private readonly PerScreen<Dictionary<string, QuestIndicator>> _indicators;

        public Dictionary<string, QuestIndicator> Indicators => _indicators.Value;

        public QuestIndicatorManager(IDisplayEvents display)
        {
            _indicators = new(() => new());
            display.RenderedWorld += OnRenderedWorld;
        }

        public QuestIndicator GetIndicator(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!Indicators.TryGetValue(name, out var indicator))
            {
                indicator = new QuestIndicator();
                Indicators[name] = indicator;
            }

            return indicator;
        }

        public QuestIndicator GetIndicator(NPC npc)
        {
            return GetIndicator(npc.Name);
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp || Game1.currentLocation == null) { return; }

            foreach (var npc in Game1.currentLocation.characters)
            {
                if (npc == null || npc.IsInvisible || npc.IsEmoting || !Utility.isOnScreen(npc.Position, 64))
                {
                    continue;
                }

                if (!Indicators.TryGetValue(npc.Name, out QuestIndicator? indicator) || indicator == null || !indicator.Visible)
                {
                    continue;
                }

                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                var position = npc.getLocalPosition(Game1.viewport);
                var scale = 4f + Math.Max(0f, 0.25f - yOffset / 8f);
                var origin = Vector2.Zero;

                position.X += npc.Sprite.SpriteWidth * 2;
                position.Y -= npc.Sprite.SpriteHeight * 4 - (npc.Gender == 1 ? 4 : 0);

                switch (indicator.CurrentMark)
                {
                    case QuestMark.Default:
                    case QuestMark.Exclamation:
                        position.X -= 6;
                        position.Y += 8;
                        e.SpriteBatch.Draw(Game1.mouseCursors, position, 
                            new Rectangle(395, 497, 3, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.ExclamationBlue:
                        position.X -= 6;
                        position.Y += 8;
                        e.SpriteBatch.Draw(Game1.mouseCursors, position,
                            new Rectangle(398, 497, 3, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.ExclamationGreen:
                        position.X -= 6;
                        position.Y += 8;
                        e.SpriteBatch.Draw(Game1.mouseCursors2, position,
                            new Rectangle(220, 160, 3, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.ExclamationBig:
                        position.X -= 10;
                        position.Y -= 14;
                        e.SpriteBatch.Draw(Game1.mouseCursors, position,
                            new Rectangle(403, 496, 5, 14), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.Question:
                        position.X -= 12;
                        position.Y += 10;
                        e.SpriteBatch.Draw(
                            Game1.mouseCursors2, position,
                            new Rectangle(114, 53, 6, 10), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.Arrow:
                        position.X -= 24;
                        position.Y -= 15;
                        e.SpriteBatch.Draw(
                            Game1.mouseCursors, position + new Vector2(0, yOffset),
                            new Rectangle(148, 208, 11, 15), Color.White, 0f, origin, 4f, SpriteEffects.FlipVertically, 1f
                        );
                        break;
                }
            }
        }
    }
}
