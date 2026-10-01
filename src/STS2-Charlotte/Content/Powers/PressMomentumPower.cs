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

/// <summary>左右逢源效果:本场战斗中,你打出的其他攻击牌造成的伤害 +Amount
/// (Strength 同型钩子;「其他」以来源卡类型排除,含其克隆,无状态、存档安全)。</summary>
[RegisterPower]
public sealed class PressMomentumPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

#if MOD_GAME_0107_1
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
#else
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
#endif
    {
        if (base.Owner != dealer || !props.IsPoweredAttack() || cardSource is Cards.WellConnected)
        {
            return 0m;
        }
        return base.Amount;
    }
}
