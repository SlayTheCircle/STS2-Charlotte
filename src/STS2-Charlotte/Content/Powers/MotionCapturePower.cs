using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.CardSelection;
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

/// <summary>动作捕捉效果:每当你打出 1 张[留影纪念],抽 1 张牌,消耗 1 张手牌(选择,可空手跳过)。</summary>
[RegisterPower]
public sealed class MotionCapturePower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || !Snapshot.IsMemento(cardPlay.Card))
        {
            return;
        }
        await CardPileCmd.Draw(choiceContext, 1m, base.Owner.Player);
        var prompt = new MegaCrit.Sts2.Core.Localization.LocString("cards", "STS2_CHARLOTTE_CARD_MOTION_CAPTURE.selectionScreenPrompt");
        CardModel? victim = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 0, 1),
            context: choiceContext,
            player: base.Owner.Player,
            filter: null,
            source: this)).FirstOrDefault();
        if (victim != null)
        {
            await CardCmd.Exhaust(choiceContext, victim);
        }
    }
}
