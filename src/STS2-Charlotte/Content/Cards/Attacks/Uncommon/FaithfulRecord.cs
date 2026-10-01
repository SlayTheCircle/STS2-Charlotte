using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 忠实记录(罕见,1 费攻击):造成 10 点伤害,给予 1 层虚弱。本回合打出其他攻击牌时,都会给予目标 1 层虚弱。升级:14。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FaithfulRecord : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new PowerVar<FaithfulRecordPower>(1m),
    };

    public FaithfulRecord()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars["WeakPower"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<FaithfulRecordPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FaithfulRecordPower"].IntValue, base.Owner.Creature, this);
        // 目标在 power 落地后注入(内部数据,不走 Amount)。
        if (base.Owner.Creature.GetPower<FaithfulRecordPower>() is { } record)
        {
            record.SetTarget(cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
