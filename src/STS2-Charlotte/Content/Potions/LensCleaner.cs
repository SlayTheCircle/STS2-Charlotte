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
/// 镜头清洁剂:你的下 1 张[留影纪念]可以免费打出(经 LensCleanerPower)。
/// 世界线第四章解锁件:揭示前由 CharlottePotionPool.GetUnlockedPotions 过滤,稀有度走常规档(Navia 解锁件同例)。
/// </summary>
[RegisterPotion(typeof(CharlottePotionPool))]
public sealed class LensCleaner : CharlottePotionBase
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Snapshot);

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<LensCleanerPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, null);
    }
}
