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
/// 镜头清洁剂效果:你的下 1 张[留影纪念]费用为 0(FreePowerPower 同型的费用改写+触发即消)。
/// </summary>
[RegisterPower]
public sealed class LensCleanerPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != base.Owner || !Snapshot.IsMemento(card))
        {
            return false;
        }
        if (card.Pile?.Type != PileType.Hand && card.Pile?.Type != PileType.Play)
        {
            return false;
        }
        modifiedCost = 0m;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && Snapshot.IsMemento(cardPlay.Card))
        {
            await PowerCmd.Remove(this);
        }
    }
}
