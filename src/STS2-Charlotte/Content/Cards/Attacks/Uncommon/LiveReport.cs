using System;
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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 现场报道(罕见,1 费攻击):给予 3 层[聚焦]。下个回合开始时,给予 6 层[聚焦]。升级:延迟段 9 层。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class LiveReport : CharlotteCardBase
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
        new PowerVar<DelayedFocusPower>(6m),
    };

    public LiveReport()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
        {
            return;
        }
        await PowerCmd.Apply<LensFocusPower>(choiceContext, cardPlay.Target, base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DelayedFocusPower>(choiceContext, cardPlay.Target, base.DynamicVars["DelayedFocusPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DelayedFocusPower"].UpgradeValueBy(3m);
    }
}
