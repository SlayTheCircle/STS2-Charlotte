using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 野外调查(罕见,1 费技能):恢复等同于你拥有的[聚焦]层数的生命值(自身聚焦)。消耗。升级:费用 0。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FieldInvestigation : CharlotteCardBase
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

    public FieldInvestigation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int focus = base.Owner.Creature.GetPowerAmount<LensFocusPower>();
        if (focus > 0)
        {
            await CreatureCmd.Heal(base.Owner.Creature, focus);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
