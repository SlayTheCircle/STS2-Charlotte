using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.Afflictions;

namespace CharlotteMod.Content.Mechanics;

/// <summary>
/// [留影]机制内核:选择一张手牌,将其消耗,并在抽牌堆放入一张效果/类型/费用完全相同、
/// 带有消耗的克隆([留影纪念],以 <see cref="SnapshotMemento"/> 标记)。
/// 选择状态牌或诅咒牌时不生成克隆,改为抽 1 张牌(所选牌保留在手)。
/// 单一收口:卡牌/遗物/药水的一切留影入口都经由本类;[聚焦]触发与「每回合首次留影」
/// 计数后续在此挂接,不在各调用点散落。
/// </summary>
public static class Snapshot
{
    /// <summary>留影纪念的唯一判据(延时摄影/剪贴相册等卡与镜头盖等遗物共用)。</summary>
    public static bool IsMemento(CardModel card) => card.Affliction is SnapshotMemento;

    /// <summary>从手牌选择一张牌留影(DualWield 同款选牌;手牌为空时静默跳过)。</summary>
    public static async Task FromHand(PlayerChoiceContext ctx, CardModel source, Player player)
    {
        var prompt = new LocString("cards", source.Id.Entry + ".selectionScreenPrompt");
        CardModel? selection = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 1),
            context: ctx,
            player: player,
            filter: null,
            source: source)).FirstOrDefault();
        if (selection != null)
        {
            await Card(ctx, selection, player);
        }
    }

    /// <summary>对指定牌执行留影(不经过选牌;笑一个!/看镜头!/广角镜头等多张入口用)。</summary>
    public static async Task Card(PlayerChoiceContext ctx, CardModel target, Player player)
    {
        if (target.Type == CardType.Status || target.Type == CardType.Curse)
        {
            await CardPileCmd.Draw(ctx, 1m, player);
            // 状态/诅咒的转化同样视为一次[留影]发生(聚焦照常结算;每回合首次留影计数亦然)。
            await TriggerFocus(target);
            return;
        }

        CardModel memento = target.CreateClone();
        await CardCmd.Afflict<SnapshotMemento>(memento, 1m);
        CardCmd.ApplyKeyword(memento, CardKeyword.Exhaust);
        await CardCmd.Exhaust(ctx, target);
        // 抽牌堆入堆走 Random 位(FuneraryMask/Severance 等原版同型惯例)。
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(memento, PileType.Draw, player, CardPilePosition.Random),
            2.2f);
        await TriggerFocus(target);
    }

    private static async Task TriggerFocus(CardModel target)
    {
        if (target.CombatState is { } combatState)
        {
            await Focus.TriggerAll(combatState);
        }
    }
}
