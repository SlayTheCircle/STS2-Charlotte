using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.RelicPools;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// 温亨廷先生(初始遗物):每回合你第一次[留影]时,恢复 2 点生命。
/// 夏洛蒂的留影机——来自枫丹的老伙计。千织屋纪念版为欧罗巴斯之触的强化替换目标。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
[RegisterTouchOfOrobasRefinement(typeof(MonsieurVeriteChioriyaEdition))]
public sealed class MonsieurVerite : CharlotteRelicBase, ISnapshotObserver
{
    private const decimal HealAmount = 2m;

    private bool _healedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => CharlotteKeywords.HoverTips(CharlotteKeywords.Snapshot);

    private bool HealedThisTurn
    {
        get => _healedThisTurn;
        set
        {
            AssertMutable();
            _healedThisTurn = value;
        }
    }

    public override async Task BeforeCombatStart()
    {
        // 消耗堆回归扫描宿主(唇枪舌剑):初始遗物每局常驻(千织屋版换装后仍在),双变体幂等挂载。
        if (base.Owner.Creature.GetPower<Powers.CombatTrackerPower>() == null)
        {
            await PowerCmd.Apply<Powers.CombatTrackerPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 0, base.Owner.Creature, null);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature))
        {
            HealedThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        HealedThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnSnapshot(PlayerChoiceContext ctx, CardModel snapped, Player player)
    {
        if (player != base.Owner || HealedThisTurn || !CombatManager.Instance.IsInProgress)
        {
            return;
        }
        HealedThisTurn = true;
        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, HealAmount);
    }
}
