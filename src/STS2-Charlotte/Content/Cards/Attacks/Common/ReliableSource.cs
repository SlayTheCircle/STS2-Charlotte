using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 可靠信源(普通,0 费攻击):造成 3 点伤害,抽 1 张牌。本回合每打出过 1 张其他攻击牌,
/// 伤害提升 3 点(读 PlayCount/History 的 Finished 条目,结算时自身尚未计入)。升级:每张 +4。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class ReliableSource : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new CalculationBaseVar(3m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => PlayCount.AttacksThisTurn(card.Owner)),
        new CardsVar(1),
    };

    public ReliableSource()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target)).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
