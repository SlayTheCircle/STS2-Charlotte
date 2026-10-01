using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
/// 特制镜头(罕见,1 费能力):每当你打出[留影纪念],获得 3 点格挡。升级:5 点。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class CustomLens : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<CustomLensPower>(3m) };

    public CustomLens()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CustomLensPower>(choiceContext, base.Owner.Creature, base.DynamicVars["CustomLensPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CustomLensPower"].UpgradeValueBy(2m);
    }
}
