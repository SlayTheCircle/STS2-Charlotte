using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 力透纸背(罕见,1 费攻击):造成 10 点伤害。如果敌人的意图是防御,则额外再造成 6 点伤害。升级:14。
/// 触发条件可视化(设计师 2026-10-03 A2 附议,参考原版战士哥「失去生命后可打2次」的金光):
/// 场上任意存活敌人意图带防御时,手牌中的本卡金光高亮(vanilla Impatience/Clash 同款钩子)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class PenetratingProse : CharlotteCardBase
{
    // 意图判定口径与 OnPlay 一致(图标有防御即算,含混合意图);卡牌图鉴等无战斗场景下静默不亮。
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(e => e.Monster?.NextMove?.Intents?.Any(i => i is DefendIntent) == true) ?? false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new CalculationBaseVar(10m),
        new ExtraDamageVar(6m),
    };

    public PenetratingProse()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    private static bool IsTargetDefending(CardPlay cardPlay)
    {
        return cardPlay.Target?.Monster?.NextMove?.Intents?.Any(i => i is DefendIntent) == true;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal total = base.DynamicVars.Damage.BaseValue + (IsTargetDefending(cardPlay) ? base.DynamicVars.ExtraDamage.BaseValue : 0m);
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
