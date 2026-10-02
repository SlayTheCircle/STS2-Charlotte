using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using CharlotteMod.Content.Characters;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// 一次性格挡保留(可见增益,墨迹未干):自身回合开始的格挡清除被跳过一次,随后自减 1 层。
/// 笔走龙蛇用;机制仿 vanilla BlurPower + Navia OneTurnBlockPersistPower
/// (回合开始顺序:ClearBlock → AfterBlockCleared → AfterSideTurnStart)。
/// 注意不能用 BlockNextTurnPower 快照:那只能保留打出时刻已有的格挡,
/// 之后再打的防御牌照样在下回合流失(2026-10-02 实装偏差,设计语义=壁垒型)。
/// </summary>
[RegisterPower]
public sealed class OneTurnBlockPersistPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldClearBlock(Creature creature)
    {
        // 返回 false 即阻止该生物的回合开始格挡清除;只保护持有者身上的格挡。
        return base.Owner != creature;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }
}
