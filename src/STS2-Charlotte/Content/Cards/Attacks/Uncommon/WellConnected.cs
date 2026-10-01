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
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 左右逢源(罕见,1 费攻击):造成 6 点伤害。在本场战斗中,这张卡以外的攻击牌造成的伤害 +3。消耗。升级:+4。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class WellConnected : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<PressMomentumPower>(3m),
    };

    public WellConnected()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<PressMomentumPower>(choiceContext, base.Owner.Creature, base.DynamicVars["PressMomentumPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PressMomentumPower"].UpgradeValueBy(1m);
    }
}
