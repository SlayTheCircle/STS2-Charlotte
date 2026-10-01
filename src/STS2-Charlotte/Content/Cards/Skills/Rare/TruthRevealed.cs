using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 真相大白(稀有,2 费技能):消耗目标所有的[聚焦],然后对目标造成 2 倍于原有[聚焦]层数的伤害。升级:3 倍。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class TruthRevealed : CharlotteCardBase
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
        new CalculationBaseVar(0m),
        new CalculationExtraVar(2m),
    };

    public TruthRevealed()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not Creature target)
        {
            return;
        }
        int stacks = target.GetPowerAmount<LensFocusPower>();
        if (stacks <= 0)
        {
            return;
        }
        if (target.GetPower<LensFocusPower>() is { } focus)
        {
            await PowerCmd.Remove(focus);
        }
        decimal total = stacks * base.DynamicVars["CalculationExtra"].BaseValue;
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationExtra"].UpgradeValueBy(1m);
    }
}
