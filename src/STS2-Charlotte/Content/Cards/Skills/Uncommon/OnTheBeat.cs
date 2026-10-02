using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 紧跟时事(罕见,1 费技能):本回合中,当你的[新闻]被消耗时,生成这张牌的[留影纪念]。升级:本场战斗中。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class OnTheBeat : CharlotteCardBase
{
    // 「消耗」为文案中的动作/牌堆引用(非本卡自身关键词),挂词条悬停而非 CanonicalKeywords 横幅(BurningPact 同款,悬浮扫描 2026-10-03)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot); // 卡面提及[留影纪念](#63)
            return set;
        }
    }

    public OnTheBeat()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TopicalLinkPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
        if (IsUpgraded && base.Owner.Creature.GetPower<TopicalLinkPower>() is { } link)
        {
            link.Permanent = true;
        }
    }
}
