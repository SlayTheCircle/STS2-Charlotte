using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 有力物证(罕见,3 费攻击):造成 30 点伤害。本回合内每打出过 1 张其他牌,这张卡的伤害下降 6,同时耗能减少 1。
/// 升级:36。「越早打越狠」的反协同设计;费用改写走卡牌自身的费用钩子(BrilliantScarf 同型)。
/// 伤害三件套必须用 ExtraDamageVar(PerfectedStrike 同型):CalculatedDamageVar.GetExtraVar 只认 ExtraDamage 键,
/// 误用格挡系 CalculationExtra 会在 Calculate/奖励镜像时 KeyNotFound(2026-10-02 商店事故)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SolidEvidence : CharlotteCardBase
{
    private const decimal PenaltyPerCard = 6m;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(30m),
        new ExtraDamageVar(6m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (CardModel card, Creature? _) => -PlayCount.CardsThisTurn(card.Owner)),
    };

    public SolidEvidence()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this)
        {
            return false;
        }
        // 增量式:基于链式 originalCost 扣减,保留先序费用钩子的修正(2026-10-03 审阅 #48)。
        int others = PlayCount.CardsThisTurn(base.Owner);
        modifiedCost = Math.Max(0m, originalCost - others);
        return true;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal total = Math.Max(0m, base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target));
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(6m);
    }
}
