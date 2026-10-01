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

/// <summary>温馨笔触效果:每当你[留影]或给予[聚焦]时,若生命值低于最大生命值的 50%,恢复 Amount 点生命。</summary>
[RegisterPower]
public sealed class WarmBrushstrokePower : CharlottePowerBase, ISnapshotObserver, IFocusApplyObserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool IsLowHp => base.Owner.CurrentHp * 2 < base.Owner.MaxHp;

    public Task OnSnapshot(PlayerChoiceContext ctx, CardModel snapped, Player player)
    {
        return HealIfLow();
    }

    public Task OnFocusApplied(Player player)
    {
        return HealIfLow();
    }

    private async Task HealIfLow()
    {
        if (IsLowHp)
        {
            await CreatureCmd.Heal(base.Owner, base.Amount);
        }
    }
}
