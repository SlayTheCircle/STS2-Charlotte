using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 不攻自破(普通,0 费技能):获得 3 点格挡,给予 1 层易伤。本回合内每打出过 1 张攻击牌,
/// 就额外获得 3 点格挡。升级:基础格挡 5。计算三件套照 Stack,面板与实战一致。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SelfDefeating : CharlotteCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(3m),
        new CalculationExtraVar(3m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => card.Owner.Creature.GetPower<CombatTrackerPower>()?.AttacksThisTurn ?? 0),
        new PowerVar<VulnerablePower>(1m),
    };

    public SelfDefeating()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, base.DynamicVars["VulnerablePower"].IntValue, base.Owner.Creature, this);
        }
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), base.DynamicVars.CalculatedBlock.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(2m);
    }
}
