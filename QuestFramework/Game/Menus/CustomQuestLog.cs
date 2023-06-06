using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.API;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;

namespace QuestFramework.Game.Menus
{
    internal class CustomQuestLog : QuestLog, IQuestMenu
    {
        private IQuest? _previousQuest;

        public static SortedSet<IQuestRenderer> Renderers { get; }

        private IQuestRenderer? _renderer;

        private string HoverText => QuestFrameworkMod.Reflection
            .GetField<string>(this, "hoverText")
            .GetValue();

        static CustomQuestLog()
        {
            Renderers = new SortedSet<IQuestRenderer>(
                Comparer<IQuestRenderer>.Create(
                    (a, b) => -1 * a.Priority.CompareTo(b.Priority)
                )
            );
        }

        protected override IList<IQuest> GetAllQuests()
        {
            var quests = new List<IQuest>();
            var manager = Game1.player.GetQuestManager();

            if (manager != null)
            {
                // Add QF quests
                for (int i = manager.Quests.Count - 1; i >= 0; i--)
                {
                    var quest = manager.Quests[i];
                    if (quest != null && !quest.IsHidden())
                    {
                        quests.Add(quest);
                    }
                }
            }

            // Forward vanilla quests
            quests.AddRange(base.GetAllQuests());

            return quests;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            UpdateInternal();
        }

        private void UpdateInternal()
        {
            if (_shownQuest != _previousQuest)
            {
                _previousQuest = _shownQuest;
                _renderer = null;

                if (_shownQuest is not ICustomQuest quest)
                    return;

                foreach (var renderer in Renderers)
                {
                    if (renderer.ShouldRenderQuest(quest))
                    {
                        _renderer = renderer;
                        break;
                    }
                }
                
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            UpdateInternal();
        }

        public override void draw(SpriteBatch b)
        {
            if (questPage == -1 || _shownQuest is not ICustomQuest quest || _renderer == null)
            {
                base.draw(b);
                return;
            }

            DrawEarly(b);
            _renderer.Draw(b, quest, this);
            DrawLate(b);
        }

        private void DrawLate(SpriteBatch b)
        {
            if (NeedsScroll())
            {
                upArrow.draw(b);
                downArrow.draw(b);
                scrollBar.draw(b);
            }
            if (currentPage < pages.Count - 1 && questPage == -1)
            {
                forwardButton.draw(b);
            }
            if (currentPage > 0 || questPage != -1)
            {
                backButton.draw(b);
            }
            if (upperRightCloseButton != null && shouldDrawCloseButton())
            {
                upperRightCloseButton.draw(b);
            }
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
            if (HoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }

        private void DrawEarly(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"), xPositionOnScreen + width / 2, yPositionOnScreen - 64);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);
        }

        public static void HookOnMenu(IDisplayEvents events)
        {
            events.MenuChanged += (_, e) =>
            {
                if (!Context.IsWorldReady) { return; }
                if (e.NewMenu is QuestLog and not CustomQuestLog)
                {
                    Game1.activeClickableMenu = new CustomQuestLog();
                    Logger.Trace("QuestLog menu is overriden by CustomQuestLog menu");
                }
            };
        }
    }
}
