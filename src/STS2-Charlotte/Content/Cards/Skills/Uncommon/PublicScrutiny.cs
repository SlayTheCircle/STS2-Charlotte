using System;
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

/// <summary>众目睽睽(罕见,1 费技能,联机限定):5/9 聚焦；每名其他玩家首次以攻击牌造成未格挡伤害后抽 2。消耗。</summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class PublicScrutiny : CharlotteCardBase
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Exhaust };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<LensFocusPower>(5m),
        new PowerVar<PublicScrutinyPower>(2m),
    };

    public PublicScrutiny()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<LensFocusPower>(choiceContext, cardPlay.Target,
            base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<PublicScrutinyPower>(choiceContext, cardPlay.Target,
            base.DynamicVars["PublicScrutinyPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["LensFocusPower"].UpgradeValueBy(4m);
    }
}
