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
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 时效核验(罕见,1 费技能):获得 4 点格挡。本回合内你每消耗过 1 张牌,就再获得 2 点格挡。升级:基础 7。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class DeadlineCheck : CharlotteCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(4m),
        new CalculationExtraVar(2m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => card.Owner.Creature.GetPower<CombatTrackerPower>()?.ExhaustsThisTurn ?? 0),
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
