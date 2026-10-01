using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 茄子!(初始卡,0 费攻击):造成 3 点伤害。你的消耗牌堆里每有 1 张牌,就造成额外 3 点伤害。
/// 升级:基础伤害 6(每张额外值不变)。
/// 预览感知三件套:显示值 = CalculationBase + ExtraDamage × 消耗堆牌数;CalculatedDamageVar
/// 的 UpdateCardPreview 会过 Hook.ModifyDamage(力量/虚弱/附魔),面板与实际出伤一致。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class CharlotteSayCheese : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new CalculationBaseVar(3m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => CardPile.GetCards(card.Owner, PileType.Exhaust).Count()),
    };

    public CharlotteSayCheese()
        : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 结算时点:本牌此时尚未入消耗堆,计数不含自身(设计语义「消耗牌堆里每有1张牌」)。
        decimal total = base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["CalculationBase"].UpgradeValueBy(3m);
    }
}
