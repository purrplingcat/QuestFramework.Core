using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using QuestFramework.Internal;

namespace QuestFramework.Menus
{
    public class CustomQuestLog : QuestLog
    {
        private class PriorityComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                // Sestupně řazení
                return y.CompareTo(x);
            }
        }

        private static readonly Dictionary<IRendererSelector, Func<IQuestRenderer>> _rendererMap = new Dictionary<IRendererSelector, Func<IQuestRenderer>>();
        private readonly IQuestRenderer _defaultRenderer = new DefaultQuestRenderer();
        private IQuestRenderer? _renderer;
        private IQuest? _previousQuest;

        public CustomQuestLog()
        {
            Renderers = _rendererMap.ToDictionary(p => p.Key, p => p.Value());
            SortedSelectors = new SortedList<int, IRendererSelector>(
                CreateSortedSelectors(_rendererMap.Keys),
                new PriorityComparer());
        }

        public CustomQuestLog(Dictionary<IRendererSelector, IQuestRenderer> renderers)
        {
            Renderers = renderers;
            SortedSelectors = new SortedList<int, IRendererSelector>(
                CreateSortedSelectors(renderers.Keys),
                new PriorityComparer());
        }

        private static IReflectionHelper Reflection => QuestCoreMod.Reflection;

        public List<List<IQuest>> Pages => pages;
        
        public int CurrentPage => currentPage;
        
        public float ContentHeight
        {
            get => _contentHeight; 
            set => _contentHeight = value;
        }

        public float ScissorRectHeight
        {
            get => _scissorRectHeight; 
            set => _scissorRectHeight = value;
        }
        
        protected Dictionary<IRendererSelector, IQuestRenderer> Renderers { get; }

        protected SortedList<int, IRendererSelector> SortedSelectors { get; }

        private string HoverText => Reflection
            .GetField<string>(this, "hoverText")
            .GetValue();

        public static void RegisterRenderer(IRendererSelector selector, Func<IQuestRenderer> rendererFactory)
        {
            _rendererMap[selector] = rendererFactory;
        }

        protected virtual IQuestRenderer? ChooseRenderer(IQuest quest)
        {
            foreach (var entry in SortedSelectors)
            {
                var selector = entry.Value;
                var renderer = Renderers[selector];

                if (selector.ShouldUseRenderer(quest))
                {
                    return renderer;
                }
            }

            return null;
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
                _renderer?.Cleanup();
                _renderer = null;

                if (_shownQuest != null)
                {
                    _renderer = ChooseRenderer(_shownQuest) ?? _defaultRenderer;
                    _renderer.Initialize(_shownQuest, this);
                }
            }
        }

        protected override void cleanupBeforeExit()
        {
            _renderer?.Cleanup();
            base.cleanupBeforeExit();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            _renderer?.Update(time);
            UpdateInternal();
        }

        public override void draw(SpriteBatch b)
        {
            if (questPage == -1 || _renderer == null)
            {
                base.draw(b);
                return;
            }

            DrawEarly(b);
            _renderer.Draw(b);
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
                drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }

        private void DrawEarly(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"), xPositionOnScreen + width / 2, yPositionOnScreen - 64);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);
        }

        internal static void HookOnQuestLog(IDisplayEvents events)
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

        private static Dictionary<int, IRendererSelector> CreateSortedSelectors(IEnumerable<IRendererSelector> selectors)
        {
            return selectors.ToDictionary(s => s.Priority, s => s);
        }

        public IQuest? GetCurrentQuest()
        {
            if (questPage != -1)
                return _shownQuest;

            return null;
        }
    }
}
