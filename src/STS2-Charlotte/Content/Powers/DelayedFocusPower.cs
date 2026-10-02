using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Powers;

/// <summary>现场报道效果:持有者所在一方回合开始时,给予持有者 Amount 层[聚焦],随后移除(美术:「下回合聚焦」图标)。
/// 本 power 由攻击牌施加到敌方目标——持有者是敌人,必须以 participants.Contains(Owner) 为判据;
/// 此前误加 side==Player 门,敌方持有者永不命中,延迟段整段死代码(2026-10-03 审阅 #5)。</summary>
[RegisterPower]
public sealed class DelayedFocusPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<LensFocusPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        }
    }
}
