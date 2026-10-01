using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 暂时规避(罕见,2 费技能):获得 15 点格挡。在下个回合获得 1 点能量(EnergyNextTurnPower)。升级:20 格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class TemporaryEvade : CharlotteCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(15m, ValueProp.Move),
        new PowerVar<EnergyNextTurnPower>(1m),
    };

    public TemporaryEvade()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, base.Owner.Creature, base.DynamicVars["EnergyNextTurnPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(5m);
    }
}
