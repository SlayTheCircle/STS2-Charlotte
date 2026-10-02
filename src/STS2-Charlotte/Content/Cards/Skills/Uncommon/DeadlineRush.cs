using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 奋笔疾书(罕见,1 费技能):抽满你的手牌。本回合内你无法再获得格挡(原版 NoBlockPower)。消耗。
/// 升级:移除无法获得格挡的负面(Mirror 2026-10-02 确认)。
/// 抽牌数 = 引擎手牌上限 - 当前手牌数(vanilla Scrawl 同款;结算中的本牌已在 Play 堆不占手位,
/// 原实现的「再 -1」固定少抽一张,2026-10-03 审阅 #9)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class DeadlineRush : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    public DeadlineRush()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int current = PileType.Hand.GetPile(base.Owner).Cards.Count;
        int toDraw = CardPile.MaxCardsInHand - current;
        if (toDraw > 0)
        {
            await CardPileCmd.Draw(choiceContext, toDraw, base.Owner);
        }
        if (!IsUpgraded)
        {
            await PowerCmd.Apply<NoBlockPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
        }
    }
}
