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

/// <summary>消耗堆回归者:回合开始时若在消耗堆,由 CombatTracker 扫描调用(唇枪舌剑)。</summary>
public interface IExileReturner
{
    Task OnTurnStartInExile(PlayerChoiceContext ctx);
}

/// <summary>留影观察者:遗物/能力实现本接口,由 Snapshot 收口在每次留影(含状态/诅咒转化)后分发。</summary>
public interface ISnapshotObserver
{
    Task OnSnapshot(PlayerChoiceContext ctx, CardModel snapped, Player player);
}

/// <summary>
/// [留影]机制内核:选择一张手牌,将其消耗,并在抽牌堆放入一张效果/类型/费用完全相同、
/// 带有消耗的克隆([留影纪念],以 <see cref="SnapshotMemento"/> 标记)。
/// 选择状态牌或诅咒牌时同样将其消耗,但不生成克隆,改为抽 1 张牌。
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
            // 设计语义:留影的「将其消耗」照常执行,只是不生成纪念,改为抽 1 张牌
            // (2026-10-02 Mirror 反馈:此前误实现为「保留在手」)。
            await CardCmd.Exhaust(ctx, target);
            await CardPileCmd.Draw(ctx, 1m, player);
            // 状态/诅咒的转化同样视为一次[留影]发生(聚焦照常结算;每回合首次留影计数亦然)。
            await TriggerFocus(target);
            await NotifyObservers(ctx, target, player);
            return;
        }

        await CardCmd.Exhaust(ctx, target);
        // Memento 内部已含聚焦结算与观察者分发,勿重复。
        await Memento(ctx, target, player);
    }

    /// <summary>
    /// 只生成留影纪念并入抽牌堆,不消耗原牌——供「把自己留影」的卡(精彩纷呈)使用:
    /// 打出中的牌在 Play 堆,立即 CardCmd.Exhaust 会与打出后的入堆路由打架,改由调用方
    /// 设置 ExhaustOnNextPlay 让引擎自行送入消耗堆。
    /// </summary>
    public static async Task Memento(PlayerChoiceContext ctx, CardModel target, Player player)
    {
        CardModel memento = target.CreateClone();
        await CardCmd.Afflict<SnapshotMemento>(memento, 1m);
        CardCmd.ApplyKeyword(memento, CardKeyword.Exhaust);
        // 抽牌堆入堆走 Random 位,预览时长用默认 1.2s(CaptureSpirit/Reave 等原版「生成牌入抽牌堆」
        // 同型惯例;2.2s 是原版弃牌循环卡的特例时长,留影高频触发下滞空观感偏长,2026-10-02 调整)。
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(memento, PileType.Draw, player, CardPilePosition.Random));
        await TriggerFocus(target);
        await NotifyObservers(ctx, target, player);
    }

    private static async Task TriggerFocus(CardModel target)
    {
        if (target.CombatState is { } combatState)
        {
            await Focus.TriggerAll(combatState, target.Owner);
        }
    }

    /// <summary>留影观察者分发(遗物面;能力面如温馨笔触后续同样在此查询)。在聚焦结算之后调用。</summary>
    private static async Task NotifyObservers(PlayerChoiceContext ctx, CardModel snapped, Player player)
    {
        foreach (RelicModel relic in player.Relics)
        {
            if (relic is ISnapshotObserver observer)
            {
                await observer.OnSnapshot(ctx, snapped, player);
            }
        }
    }
}
