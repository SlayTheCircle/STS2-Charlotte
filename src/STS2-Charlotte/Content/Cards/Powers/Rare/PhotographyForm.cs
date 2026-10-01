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
/// 摄影形态(稀有,3 费能力):你每消耗1张牌，对全体敌人造成{PhotographyFormPower:diff()}点伤害。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class PhotographyForm : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<PhotographyFormPower>(4m) };

    public PhotographyForm()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PhotographyFormPower>(choiceContext, base.Owner.Creature, base.DynamicVars["PhotographyFormPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PhotographyFormPower"].UpgradeValueBy(2m);
    }

}
