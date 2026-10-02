using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 时效核验(罕见,1 费技能):获得 4 点格挡。本回合内你每消耗过 1 张牌,就再获得 2 点格挡。升级:基础 7。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class DeadlineCheck : CharlotteCardBase
{
    // 「消耗」为文案中的动作/牌堆引用(非本卡自身关键词),挂词条悬停而非 CanonicalKeywords 横幅(BurningPact 同款,悬浮扫描 2026-10-03)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(4m),
        new CalculationExtraVar(2m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => PlayCount.ExhaustsThisTurn(card.Owner)),
    };

    public DeadlineCheck()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), base.DynamicVars.CalculatedBlock.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(3m);
    }
}
