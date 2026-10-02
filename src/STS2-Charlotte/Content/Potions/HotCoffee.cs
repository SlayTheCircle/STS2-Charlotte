using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.PotionPools;

namespace CharlotteMod.Content.Potions;

/// <summary>
/// 香浓的热咖啡:恢复你已损失生命值的 40%(向下取整)。随时可用。
/// 世界线第四章解锁件:揭示前由 CharlottePotionPool.GetUnlockedPotions 过滤,稀有度走常规档(Navia 解锁件同例)。
/// </summary>
[RegisterPotion(typeof(CharlottePotionPool))]
public sealed class HotCoffee : CharlottePotionBase
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.AnyTime;

    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        decimal missing = base.Owner.Creature.MaxHp - base.Owner.Creature.CurrentHp;
        decimal heal = Math.Floor(missing * 0.4m);
        if (heal > 0m)
        {
            await CreatureCmd.Heal(base.Owner.Creature, heal);
        }
    }
}
