using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.RelicPools;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// 采访稿记录本:每当你[留影]时,消耗你抽牌堆中的 1 张随机状态牌或诅咒牌。
/// 世界线第三章解锁件:揭示前由 CharlotteRelicPool.GetUnlockedRelics 过滤,稀有度走常规 Common 档(Navia 解锁件同例)。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
public sealed class InterviewNotebook : CharlotteRelicBase, ISnapshotObserver
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Snapshot);

    public async Task OnSnapshot(PlayerChoiceContext ctx, CardModel snapped, Player player)
    {
        if (player != base.Owner || !CombatManager.Instance.IsInProgress)
        {
            return;
        }
        List<CardModel> candidates = CardPile.GetCards(base.Owner, PileType.Draw)
            .Where(c => c.Type == CardType.Status || c.Type == CardType.Curse)
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        CardModel victim = base.Owner.RunState.Rng.CombatCardSelection.NextItem(candidates);
        Flash();
        await CardCmd.Exhaust(ctx, victim);
    }
}
