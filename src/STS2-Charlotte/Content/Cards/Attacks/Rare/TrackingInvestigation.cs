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
/// 追踪调查(稀有,9 费攻击):对全体敌人造成 50 点伤害。每当你消耗 1 张牌,这张卡的费用就降低 1 点(使用后恢复)。升级:65。
/// 折扣 = 本战斗累计消耗数 - 上次打出时的快照;打出即重置(「使用后恢复」)。世界线第二章解锁件。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class TrackingInvestigation : CharlotteCardBase
{
    private const decimal BaseCost = 9m;

    private int _exhaustsAtLastCast;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(50m, ValueProp.Move) };

    public TrackingInvestigation()
        : base(9, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    private int Discount => Math.Max(0, PlayCount.ExhaustsThisCombat(base.Owner) - _exhaustsAtLastCast);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this)
        {
            return false;
        }
        modifiedCost = Math.Max(0m, BaseCost - Discount);
        return true;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _exhaustsAtLastCast = PlayCount.ExhaustsThisCombat(base.Owner);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(15m);
    }
}
