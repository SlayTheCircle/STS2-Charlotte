using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.PotionPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Potions;

/// <summary>
/// 提神醒脑茶:给予敌人 9 层[聚焦]。
/// 世界线第四章解锁件:揭示前由 CharlottePotionPool.GetUnlockedPotions 过滤,稀有度走常规档(Navia 解锁件同例)。
/// </summary>
[RegisterPotion(typeof(CharlottePotionPool))]
public sealed class RefreshingTea : CharlottePotionBase
{
    private const int FocusAmount = 9;

    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Focus);

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null)
        {
            await PowerCmd.Apply<LensFocusPower>(choiceContext, target, FocusAmount, base.Owner.Creature, null);
        }
    }
}
