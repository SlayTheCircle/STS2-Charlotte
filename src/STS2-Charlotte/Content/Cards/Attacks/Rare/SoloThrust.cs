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
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 单刀直入(稀有,0 费攻击):本场战斗中,你每打出过 1 张攻击牌,此卡就造成 3 点伤害。升级:4 点。
/// 战斗累计攻击数由 PlayCount(History 口径)承载。
/// 伤害三件套必须用 ExtraDamageVar(PerfectedStrike 同型):CalculatedDamageVar.GetExtraVar 只认 ExtraDamage 键,
/// 误用格挡系 CalculationExtra 会在 Calculate/奖励镜像时 KeyNotFound(2026-10-02 商店事故)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SoloThrust : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(0m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => PlayCount.AttacksThisCombat(card.Owner)),
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
        base.DynamicVars["ExtraDamage"].UpgradeValueBy(1m);
    }
}
