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

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 高危调查(普通,0 费攻击):造成 4 点伤害。抽 1 张牌。这张卡的伤害下降 1 点
/// (每次打出后永久-1,本场战斗内累计;升级:伤害 4→7,Mirror 2026-10-02 确认)。
/// 衰减直接下修 DamageVar.BaseValue(vanilla TheBombPower.SetDamage 同款):面板 {Damage}
/// 随打出自减,不再私有计数另算一套(2026-10-03 审阅 #3,面板慢一拍病根同族)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class HighRiskInvestigation : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new CardsVar(1),
    };

    public HighRiskInvestigation()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        base.DynamicVars.Damage.BaseValue = Math.Max(0m, base.DynamicVars.Damage.BaseValue - 1m);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
