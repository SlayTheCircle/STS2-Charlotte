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
/// 跟进调查(罕见,1 费技能):本回合内,当你打出[新闻]时,抽 1 张牌。升级:抽 2 张。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FollowUpInvestigation : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<FollowUpPower>(1m),
    };

    public FollowUpInvestigation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FollowUpPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FollowUpPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FollowUpPower"].UpgradeValueBy(1m);
    }
}
