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

/// <summary>精力充沛效果:你下次造成伤害时,获得与伤害量等量的格挡(一次性,触发即消)。</summary>
[RegisterPower]
public sealed class SecondWindPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 「与伤害量等量」= 总伤害(被格挡部分同样计入);完全格挡(总伤 0)不消耗蓄势。
        int total = result.BlockedDamage + result.UnblockedDamage;
        if (dealer == base.Owner && total > 0)
        {
            await PowerCmd.Remove(this);
            await CreatureCmd.GainBlock(base.Owner, total, ValueProp.Unpowered, null);
        }
    }
}
