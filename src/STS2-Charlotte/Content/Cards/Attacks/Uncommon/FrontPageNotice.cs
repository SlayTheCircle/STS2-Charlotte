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
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 大字通告(罕见,2 费攻击):造成 12 点伤害,给予 1 层虚弱。升级:16 伤 2 层。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FrontPageNotice : CharlotteCardBase
{
    // 文案提及的效果词/机制动词悬停(悬浮扫描 2026-10-03;ModCardTemplate 封死 ExtraHoverTips,扩展点 AdditionalHoverTips)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<WeakPower>() };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
    };

    public FrontPageNotice()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars["WeakPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}
