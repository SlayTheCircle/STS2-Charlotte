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
/// 枫丹洋葱汤:本回合内,你的[新闻]可以免费打出(经 OnionSoupPower,回合结束移除)。
/// </summary>
[RegisterPotion(typeof(CharlottePotionPool))]
public sealed class FontaineOnionSoup : CharlottePotionBase
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.News);

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<OnionSoupPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, null);
    }
}
