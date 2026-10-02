using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.RelicPools;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.HoverTips;
using CharlotteMod.Content.Keywords;
using System.Linq;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// 镜头盖:每当[留影纪念]被消耗,你获得 2 点格挡(原版 AfterCardExhausted 钩子 + IsMemento 判据)。
/// 世界线第三章解锁件:揭示前由 CharlotteRelicPool.GetUnlockedRelics 过滤,稀有度走常规 Common 档(Navia 解锁件同例)。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
public sealed class LensCap : CharlotteRelicBase
{
    // 文案提及[留影纪念]/被消耗/格挡(悬浮扫描 2026-10-03,7 件遗物中唯一全缺悬停者)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Snapshot)
        .Concat(new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.Static(StaticHoverTip.Block) });

    private const decimal BlockAmount = 2m;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner != base.Owner || !Snapshot.IsMemento(card))
        {
            return;
        }
        Flash();
        await CreatureCmd.GainBlock(base.Owner.Creature, BlockAmount, ValueProp.Unpowered, null);
    }
}
