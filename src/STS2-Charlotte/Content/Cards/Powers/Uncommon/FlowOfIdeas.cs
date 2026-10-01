using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 文思泉涌(罕见,1 费能力):每当你没有手牌时,抽 1 张牌。升级:费用 0。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FlowOfIdeas : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<FlowOfIdeasPower>(1m) };

    public FlowOfIdeas()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlowOfIdeasPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FlowOfIdeasPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
