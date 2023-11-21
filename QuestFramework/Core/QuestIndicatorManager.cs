using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp || Game1.currentLocation == null) { return; }

            e.SpriteBatch.Begin();
            foreach (var npc in Game1.currentLocation.characters)
            {
                if (npc == null || npc.IsInvisible || !Utility.isOnScreen(npc.Position, 64))
                {
                    continue;
                }

                if (!Indicators.TryGetValue(npc.Name, out QuestIndicator? indicator) || indicator == null)
                {
                    continue;
                }

                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                var position = Game1.GlobalToLocal(Game1.viewport, new Vector2(npc.Position.X, npc.Position.Y + yOffset));
                var scale = 4f + Math.Max(0f, 0.25f - yOffset / 8f);
                var origin = new Vector2(1f, 4f);

                switch (indicator.CurrentMark)
                {
                    case QuestMark.Default:
                    case QuestMark.Exclamation:
                        e.SpriteBatch.Draw(Game1.mouseCursors, position, 
                            new Rectangle(395, 497, 3, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.ExclamationBlue:
                        e.SpriteBatch.Draw(Game1.mouseCursors, position,
                            new Rectangle(398, 497, 3, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.ExclamationBig:
                        position.Y -= 14;
                        e.SpriteBatch.Draw(Game1.mouseCursors, position,
                            new Rectangle(403, 496, 5, 14), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                    case QuestMark.Question:
                        e.SpriteBatch.Draw(
                            Game1.mouseCursors2, position,
                            new Rectangle(114, 53, 6, 10), Color.White, 0f, origin, scale, SpriteEffects.None, 1f
                        );
                        break;
                }
            }
            e.SpriteBatch.End();
        }
    }
}
