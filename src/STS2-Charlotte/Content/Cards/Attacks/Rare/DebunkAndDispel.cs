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
/// 辟谣祛魅(稀有,0 费攻击):造成 4 点伤害。这张卡的耗能增加 1,且造成的伤害翻倍(每次打出后永久累计,本场战斗)。升级:变为三倍。
/// 面板伤害随打出直接推进(BaseValue setter),数字始终预告下一打;费用 +次数(_casts)。
/// 升级差异另由 loc 的 {IfUpgraded:show:变为三倍|翻倍} 呈现。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class DebunkAndDispel : CharlotteCardBase
{
    private int _casts;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move) };

    public DebunkAndDispel()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    private decimal Multiplier => IsUpgraded ? 3m : 2m;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 结算用当前面板值,随后把面板推进到下一次的期望值(4→8→16…/升级 4→12→36…)——
        // 卡面数字始终预告下一打伤害(DynamicVar.BaseValue setter 自动重置预览;战斗实例重置自然回 4)。
        // 倍率取「打出当时」的:升级只放大利好后续增长,不追溯放大已打出的次数。
        decimal total = base.DynamicVars.Damage.BaseValue;
        _casts++;
        base.DynamicVars.Damage.BaseValue = total * Multiplier;
        await DamageCmd.Attack(total).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this)
        {
            return false;
        }
        modifiedCost = originalCost + _casts;
        return true;
    }
}
