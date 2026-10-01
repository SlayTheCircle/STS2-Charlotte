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
/// 吸引视线(罕见,1 费技能):给予自身 3 层[聚焦],抽 2 张牌。升级:抽 3 张。
/// 自我聚焦=资源(谨遵事实转格挡/野外调查转治疗),代价是触发时自伤——设计核心联动位。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class DrawAttention : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<LensFocusPower>(3m),
        new CardsVar(2),
    };

    public DrawAttention()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LensFocusPower>(choiceContext, base.Owner.Creature, base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
