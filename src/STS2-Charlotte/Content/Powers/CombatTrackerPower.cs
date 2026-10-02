using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// 消耗堆回归扫描(隐藏):己方回合开始时扫描消耗堆,唤醒唇枪舌剑等 IExileReturner 卡。
/// 由初始遗物(温亨廷先生/千织屋纪念版)在战斗开始时挂载——初始遗物每局常驻且换装后仍在,
/// 双变体都接(幂等)。出牌/消耗计数已迁往 <see cref="PlayCount"/>(History 口径,
/// 2026-10-02 面板慢一拍修复);本类不再承担计数。无需本地化(永不可见)。
/// </summary>
[RegisterPower]
public sealed class CombatTrackerPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            // 消耗堆回归扫描(唇枪舌剑:回合开始若在消耗堆,加伤并回手)。
            foreach (CardModel card in PileType.Exhaust.GetPile(base.Owner.Player).Cards.ToList())
            {
                if (card is IExileReturner returner)
                {
                    await returner.OnTurnStartInExile(choiceContext);
                }
            }
        }
    }
}
