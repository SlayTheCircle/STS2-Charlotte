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
/// (每次打出后永久-1,本场战斗内累计;升级无变化——设计待确认项#1,两表一致故照录)。
/// 战斗实例每场重建,惩罚字段天然随战斗清零。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class HighRiskInvestigation : CharlotteCardBase
{
    private decimal _damagePenalty;

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
        decimal total = Math.Max(0m, base.DynamicVars.Damage.BaseValue - _damagePenalty);
        _damagePenalty += 1m;
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }
}
