using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 真实至上(稀有,1 费能力):你每打出1张牌，都会获得{TruthAbovePower:diff()}点[gold]格挡[/gold]。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class TruthAbove : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<TruthAbovePower>(1m) };

    public TruthAbove()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TruthAbovePower>(choiceContext, base.Owner.Creature, base.DynamicVars["TruthAbovePower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["TruthAbovePower"].UpgradeValueBy(1m);
    }

}
