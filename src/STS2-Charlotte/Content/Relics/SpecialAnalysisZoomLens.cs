using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;
using CharlotteMod.Content.RelicPools;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// [特殊分析变焦镜头]:每回合你第一次[留影]时,给予全体敌人 2 层[聚焦]。
/// 世界线第三章解锁件:揭示前由 CharlotteRelicPool.GetUnlockedRelics 过滤,稀有度走常规 Common 档(Navia 解锁件同例)。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
public sealed class SpecialAnalysisZoomLens : CharlotteRelicBase, ISnapshotObserver
{
    private const int FocusAmount = 2;

    private bool _triggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Snapshot, CharlotteKeywords.Focus);

    private bool TriggeredThisTurn
    {
        get => _triggeredThisTurn;
        set
        {
            AssertMutable();
            _triggeredThisTurn = value;
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature))
        {
            TriggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        TriggeredThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnSnapshot(PlayerChoiceContext ctx, CardModel snapped, Player player)
    {
        if (player != base.Owner || TriggeredThisTurn || !CombatManager.Instance.IsInProgress)
        {
            return;
        }
        TriggeredThisTurn = true;
        Flash();
        if (base.Owner.Creature.CombatState is { } combatState)
        {
            foreach (Creature enemy in combatState.Enemies)
            {
                if (enemy.IsAlive)
                {
                    await PowerCmd.Apply<LensFocusPower>(ctx, enemy, FocusAmount, base.Owner.Creature, null);
                }
            }
        }
    }
}
