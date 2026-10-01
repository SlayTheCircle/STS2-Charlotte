using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
/// 单刀直入(稀有,0 费攻击):本场战斗中,你每打出过 1 张攻击牌,此卡就造成 3 点伤害。升级:4 点。
/// 战斗累计攻击数由 CombatTracker.AttacksThisCombat 承载。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SoloThrust : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new CalculationExtraVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => card.Owner.Creature.GetPower<CombatTrackerPower>()?.AttacksThisCombat ?? 0),
    };

    public SoloThrust()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target)).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationExtra"].UpgradeValueBy(1m);
    }
}
