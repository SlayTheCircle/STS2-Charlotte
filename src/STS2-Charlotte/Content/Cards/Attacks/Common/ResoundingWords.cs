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
/// 掷地有声(普通,0 费攻击):造成 4 点伤害,如果这是你本回合打出的第一张攻击牌,则再造成 6 点伤害。
/// 升级:追加 9 点。「第一张」以 CombatTracker 结算时计数为准(自身尚未计入)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class ResoundingWords : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new ExtraDamageVar(6m),
    };

    public ResoundingWords()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    private bool IsFirstAttackThisTurn => (base.Owner.Creature.GetPower<CombatTrackerPower>()?.AttacksThisTurn ?? 0) == 0;

    protected override bool ShouldGlowGoldInternal => IsFirstAttackThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal total = base.DynamicVars.Damage.BaseValue + (IsFirstAttackThisTurn ? base.DynamicVars.ExtraDamage.BaseValue : 0m);
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}
