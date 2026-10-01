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

/// <summary>紧跟时事效果:本回合内,你的[新闻](原版新闻牌,纪念克隆被消耗不再连锁)被消耗时,生成这张牌的[留影纪念]。回合结束移除。</summary>
[RegisterPower]
public sealed class TopicalLinkPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>紧跟时事升级版:本场战斗持续(战斗内标记,不随回合结束移除)。</summary>
    public bool Permanent { get; set; }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature == base.Owner && News.IsNews(card) && !Snapshot.IsMemento(card))
        {
            await Snapshot.Memento(choiceContext, card, card.Owner);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!Permanent && participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
