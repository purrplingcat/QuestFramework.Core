using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley;
using StardewValley.Quests;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.SpecialOrders;
using Microsoft.Xna.Framework;

namespace QuestFramework.Menus
{
    public class DefaultQuestRenderer : IQuestRenderer
    {
        private IQuest? _quest;
        private CustomQuestLog? _questMenu;
        private List<string> _objectiveText = new();

        protected IQuest Quest => _quest ?? throw new ArgumentNullException(nameof(Quest));
        protected CustomQuestLog Menu => _questMenu ?? throw new ArgumentNullException(nameof(Menu));
        protected bool Initialized { get; private set; }

        public virtual void Initialize(IQuest quest, CustomQuestLog questMenu)
        {
            _quest = quest;
            _questMenu = questMenu;
            _objectiveText = quest.GetObjectiveDescriptions();
            Initialized = true;
        }

        public virtual void Draw(SpriteBatch b)
        {
            float yPos = 0;
            Rectangle scissor_rect = default;
            Rectangle cached_scissor_rect = b.GraphicsDevice.ScissorRectangle;

            // Draw title
            DrawTitle(b);

            // Enter the scrissor mode to draw description and objectives scrollable
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
            {
                ScissorTestEnable = true
            });

            // Draw description
            DrawDescription(b, ref scissor_rect, ref yPos);

            // Draw reward box if quest is completed
            if (Quest.ShouldDisplayAsComplete())
            {
                // We draw reward in normal mode
                b.End();
                b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                DrawReward(b);
                return;
            }

            // Draw objectives
            for (int j = 0; j < _objectiveText.Count; j++)
            {
                yPos = DrawObjective(b, scissor_rect, yPos, j);
            }

            // Draw quest footer in normal mode
            b.End();
            b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (Quest.CanBeCancelled())
            {
                Menu.cancelQuestButton.draw(b);
            }
            if (Menu.NeedsScroll())
            {
                if (Menu.scrollAmount > 0f)
                {
                    b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Top, scissor_rect.Width, 4), Color.Black * 0.15f);
                }
                if (Menu.scrollAmount < Menu.ContentHeight - Menu.ScissorRectHeight)
                {
                    b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Bottom - 4, scissor_rect.Width, 4), Color.Black * 0.15f);
                }
            }
        }

        protected virtual void DrawReward(SpriteBatch b)
        {
            SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), Menu.xPositionOnScreen + 32 + 4, Menu.rewardBox.bounds.Y + 21 + 4);
            Menu.rewardBox.draw(b);
            if (Menu.HasMoneyReward())
            {
                b.Draw(Game1.mouseCursors, new Vector2(Menu.rewardBox.bounds.X + 16, Menu.rewardBox.bounds.Y + 16 - Game1.dialogueButtonScale / 2f), new Rectangle(280, 410, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", Quest.GetMoneyReward()), Menu.xPositionOnScreen + 448, Menu.rewardBox.bounds.Y + 21 + 4);
            }
        }

        protected virtual void DrawDescription(SpriteBatch b, ref Rectangle scissor_rect, ref float yPos)
        {
            string description = Game1.parseText(Quest.GetDescription(), Game1.dialogueFont, Menu.width - 128);
            Vector2 description_size = Game1.dialogueFont.MeasureString(description);
            scissor_rect.X = Menu.xPositionOnScreen + 32;
            scissor_rect.Y = Menu.yPositionOnScreen + 96;
            scissor_rect.Height = Menu.yPositionOnScreen + Menu.height - 32 - scissor_rect.Y;
            scissor_rect.Width = Menu.width - 64;
            Menu.ScissorRectHeight = scissor_rect.Height;
            scissor_rect = Utility.ConstrainScissorRectToScreen(scissor_rect);
            
            Game1.graphics.GraphicsDevice.ScissorRectangle = scissor_rect;
            Utility.drawTextWithShadow(b, description, Game1.dialogueFont, new Vector2(Menu.xPositionOnScreen + 64, Menu.yPositionOnScreen - Menu.scrollAmount + 96f), Game1.textColor);
            yPos = Menu.yPositionOnScreen + 96 + description_size.Y + 32f - Menu.scrollAmount;
        }

        protected virtual void DrawTitle(SpriteBatch b)
        {
            SpriteText.drawStringHorizontallyCenteredAt(b, Quest.GetName(), Menu.xPositionOnScreen + Menu.width / 2 + ((Quest.IsTimedQuest() && Quest.GetDaysLeft() > 0) ? (Math.Max(32, SpriteText.getWidthOfString(Quest.GetName()) / 3) - 32) : 0), Menu.yPositionOnScreen + 32);
            if (Quest.IsTimedQuest() && Quest.GetDaysLeft() > 0)
            {
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(Menu.xPositionOnScreen + 32, Menu.yPositionOnScreen + 48 - 8), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f);
                Utility.drawTextWithShadow(b, Game1.parseText((Quest.GetDaysLeft() > 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", Quest.GetDaysLeft()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest_FinalDay"), Game1.dialogueFont, Menu.width - 128), Game1.dialogueFont, new Vector2(Menu.xPositionOnScreen + 80, Menu.yPositionOnScreen + 48 - 8), Game1.textColor);
            }
        }

        protected virtual float DrawObjective(SpriteBatch b, Rectangle scissor_rect, float yPos, int index)
        {
            bool completed = _objectiveText[index].StartsWith('~') || (Quest is SpecialOrder o && o.objectives[index].IsComplete());
            string objective_text = _objectiveText[index].StartsWith('~') ? _objectiveText[index][1..] : _objectiveText[index];
            string parsed_text = Game1.parseText(objective_text, width: Menu.width - 192, whichFont: Game1.dialogueFont);
            Color text_color = Game1.unselectedOptionColor;
            if (!completed)
            {
                text_color = Color.DarkBlue;
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(Menu.xPositionOnScreen + 96 + 8f * Game1.dialogueButtonScale / 10f, yPos), new Rectangle(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);
            }
            Utility.drawTextWithShadow(b, parsed_text, Game1.dialogueFont, new Vector2(Menu.xPositionOnScreen + 128, yPos - 8f), text_color);
            yPos += Game1.dialogueFont.MeasureString(parsed_text).Y;
            if (Quest is SpecialOrder order)
            {
                OrderObjective order_objective = order.objectives[index];
                if (order_objective.GetMaxCount() > 1 && order_objective.ShouldShowProgress())
                {
                    Color dark_bar_color = Color.DarkRed;
                    Color bar_color = Color.Red;
                    if (order_objective.GetCount() >= order_objective.GetMaxCount())
                    {
                        bar_color = Color.LimeGreen;
                        dark_bar_color = Color.Green;
                    }
                    int inset = 64;
                    int objective_count_draw_width = 160;
                    int notches = 4;
                    Rectangle bar_background_source = new(0, 224, 47, 12);
                    Rectangle bar_notch_source = new(47, 224, 1, 12);
                    int bar_horizontal_padding = 3;
                    int bar_vertical_padding = 3;
                    int slice_width = 5;
                    string objective_count_text = order_objective.GetCount() + "/" + order_objective.GetMaxCount();
                    int max_text_width = (int)Game1.dialogueFont.MeasureString(order_objective.GetMaxCount() + "/" + order_objective.GetMaxCount()).X;
                    int count_text_width = (int)Game1.dialogueFont.MeasureString(objective_count_text).X;
                    int text_draw_position = Menu.xPositionOnScreen + Menu.width - inset - count_text_width;
                    int max_text_draw_position = Menu.xPositionOnScreen + Menu.width - inset - max_text_width;
                    Utility.drawTextWithShadow(b, objective_count_text, Game1.dialogueFont, new Vector2(text_draw_position, yPos), Color.DarkBlue);
                    Rectangle bar_draw_position = new(Menu.xPositionOnScreen + inset, (int)yPos, Menu.width - inset * 2 - objective_count_draw_width, bar_background_source.Height * 4);
                    if (bar_draw_position.Right > max_text_draw_position - 16)
                    {
                        int adjustment = bar_draw_position.Right - (max_text_draw_position - 16);
                        bar_draw_position.Width -= adjustment;
                    }
                    b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X + slice_width * 4, bar_draw_position.Y, bar_draw_position.Width - 2 * slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X + slice_width, bar_background_source.Y, bar_background_source.Width - 2 * slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.Right - slice_width * 4, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.Right - slice_width, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    float quest_progress = (float)order_objective.GetCount() / order_objective.GetMaxCount();
                    if (order_objective.GetMaxCount() < notches)
                    {
                        notches = order_objective.GetMaxCount();
                    }
                    bar_draw_position.X += 4 * bar_horizontal_padding;
                    bar_draw_position.Width -= 4 * bar_horizontal_padding * 2;
                    for (int k = 1; k < notches; k++)
                    {
                        b.Draw(Game1.mouseCursors2, new Vector2(bar_draw_position.X + bar_draw_position.Width * (k / notches), bar_draw_position.Y), bar_notch_source, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
                    }
                    bar_draw_position.Y += 4 * bar_vertical_padding;
                    bar_draw_position.Height -= 4 * bar_vertical_padding * 2;
                    Rectangle rect = new Rectangle(bar_draw_position.X, bar_draw_position.Y, (int)(bar_draw_position.Width * quest_progress) - 4, bar_draw_position.Height);
                    b.Draw(Game1.staminaRect, rect, null, bar_color, 0f, Vector2.Zero, SpriteEffects.None, rect.Y / 10000f);
                    rect.X = rect.Right;
                    rect.Width = 4;
                    b.Draw(Game1.staminaRect, rect, null, dark_bar_color, 0f, Vector2.Zero, SpriteEffects.None, rect.Y / 10000f);
                    yPos += (bar_background_source.Height + 4) * 4;
                }
            }
            Menu.ContentHeight = yPos + Menu.scrollAmount - scissor_rect.Y;
            return yPos;
        }

        public void Cleanup()
        {
            _quest = null;
            _questMenu = null;
            _objectiveText.Clear();
        }

        public void Update(GameTime time)
        {
        }
    }
}