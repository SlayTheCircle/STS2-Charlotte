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
using STS2RitsuLib.Interop.AutoRegistration;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// 看镜头！效果:接下来的数个回合开始时,从被留影消耗的牌中随机(升级后:选择)一张升级并返回你的手牌。
/// </summary>
[RegisterPower]
public sealed class LookAtCameraPower : CharlottePowerBase
{
    private sealed class Data
    {
        public List<CardModel> pending = new();
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>升级版看镜头：改为由玩家选择回归哪张(否则随机)。</summary>
    public bool ChooseInsteadOfRandom { get; set; }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetCards(IEnumerable<CardModel> cards)
    {
        GetInternalData<Data>().pending = cards.ToList();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature != base.Owner)
        {
            return;
        }
        List<CardModel> pending = GetInternalData<Data>().pending;
        if (pending.Count == 0)
        {
            await PowerCmd.Remove(this);
            return;
        }
        CardModel pick;
        if (ChooseInsteadOfRandom && pending.Count > 1)
        {
            var prompt = new MegaCrit.Sts2.Core.Localization.LocString("cards", "STS2_CHARLOTTE_CARD_LOOK_AT_CAMERA.selectionScreenPrompt");
            pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, pending, player, new CardSelectorPrefs(prompt, 1))).First();
        }
        else
        {
            pick = player.RunState.Rng.CombatCardSelection.NextItem(pending);
        }
        pending.Remove(pick);
        CardCmd.Upgrade(pick);
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { pick }, PileType.Hand, player);
        if (pending.Count == 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
