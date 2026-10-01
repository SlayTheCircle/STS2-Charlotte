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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 核实流程(罕见,1 费技能):给予 9 层[聚焦]。消耗。升级:13 层。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FactCheck : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Exhaust };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<LensFocusPower>(9m) };

    public FactCheck()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<LensFocusPower>(choiceContext, cardPlay.Target, base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["LensFocusPower"].UpgradeValueBy(4m);
    }
}
