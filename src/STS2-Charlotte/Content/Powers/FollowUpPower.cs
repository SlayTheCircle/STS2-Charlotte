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

/// <summary>跟进调查效果:本回合内,你打出[新闻]时,抽 N 张牌。回合结束移除。</summary>
[RegisterPower]
public sealed class FollowUpPower : CharlottePowerBase, INewsObserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnNewsPlayed(PlayerChoiceContext ctx, CardModel news, Player player)
    {
        if (player.Creature == base.Owner)
        {
            await CardPileCmd.Draw(ctx, base.Amount, player);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
