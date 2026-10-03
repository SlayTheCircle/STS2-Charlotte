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
        // 抽N耗N(业主 2026-10-03 裁决 C4,设计师复批「不要乱动」):抽牌与消耗都随层数放大。
        await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
        // 有消耗就消耗、没消耗就跳过(设计师 B 批复口径):恰好 N 在手牌不足时会被
        // 引擎 count<=min 路径整体跳过,把够数的牌也漏掉——min 钳到现存张数,有几张耗几张。
        int hand = PileType.Hand.GetPile(base.Owner.Player).Cards.Count;
        if (hand == 0)
        {
            return;
        }
        var prompt = new MegaCrit.Sts2.Core.Localization.LocString("cards", "STS2_CHARLOTTE_CARD_MOTION_CAPTURE.selectionScreenPrompt");
        List<CardModel> victims = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, Math.Min(base.Amount, hand)),
            context: choiceContext,
            player: base.Owner.Player,
            filter: null,
            source: this)).ToList();
        // 全部消耗(0.1.1 实装缺口:选 N 张却只 FirstOrDefault 消耗 1 张,与抽N耗N裁定不符)。
        foreach (CardModel victim in victims)
        {
            await CardCmd.Exhaust(choiceContext, victim);
        }
    }
}
