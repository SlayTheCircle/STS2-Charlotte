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

/// <summary>
/// [聚焦](类名 LensFocusPower,避开原版宝珠 LensFocusPower):叠层负面效果,敌我皆可持有(吸引视线/临场发挥会给自身叠加,转为资源)。
/// 每当[留影]或打出[新闻]牌时,全部持有者失去等同层数的生命(结算收口见 Mechanics/Focus,
/// 伤害走 Unblockable|Unpowered——不可格挡、不吃力量,PoisonPower 同款)。
/// 回合结束时层数 -1(默认理解:持有者一方回合结束衰减,待设计确认,见 local_dev 待确认清单)。
/// </summary>
[RegisterPower]
public sealed class LensFocusPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 施加方联动(温馨笔触/切中要害等):正增量时经 applier 的能力/遗物分发。
        // 挂在自身实例的 AfterPowerAmountChanged 而非 AfterApplied——引擎对已有 power
        // 续层走 PowerCmd.ModifyAmount,不回调 AfterApplied,续层时的「每次给予聚焦」
        // 联动此前整段丢失(2026-10-03 审阅 #13);两条路径都会分发本钩子,正数门控
        // 排除回合末衰减的负增量。
        if (power != this || amount <= 0m)
        {
            return;
        }
        if (applier?.Player is { } player)
        {
            foreach (PowerModel? observer in applier.Powers)
            {
                if (observer is Mechanics.IFocusApplyObserver o)
                {
                    await o.OnFocusApplied(player);
                }
            }
            foreach (RelicModel relic in player.Relics)
            {
                if (relic is Mechanics.IFocusApplyObserver o)
                {
                    await o.OnFocusApplied(player);
                }
            }
        }
    }
}
