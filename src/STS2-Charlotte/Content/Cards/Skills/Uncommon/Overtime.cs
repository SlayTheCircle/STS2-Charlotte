using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 加班加点(罕见,1 费技能):消耗 1 张牌,获得 2 点能量。升级:3 点能量。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Overtime : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new EnergyVar(2) };

    public Overtime()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        CardModel? victim = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 0, 1),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this)).FirstOrDefault();
        if (victim != null)
        {
            await CardCmd.Exhaust(choiceContext, victim);
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
