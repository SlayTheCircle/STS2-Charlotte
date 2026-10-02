using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Rewards;
using CharlotteMod.Content.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.Events;

/// <summary>
/// 美不胜收(1 层事件,Overgrowth):绝美晚霞前选择拍摄对象。
/// 河水=选 1 张打击或防御附魔涡旋 1 层(StoneOfAllTime 的附魔范式+打击防御过滤);
/// 鸟儿=1 张原版[异鸟扑击]加入手牌;晚霞=3 张全升级的卡牌奖励(全部升级,非升级概率 roll)。
/// </summary>
[RegisterActEvent(typeof(Overgrowth))]
public sealed class BreathtakingView : CharlotteEventBase
{
    /// <summary>专属事件肖像由美术母版派生；显式覆盖 Mod 资源路径。</summary>
    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Charlotte/images/events/BreathtakingView.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            // 选项文案点名打击/防御两张卡+涡旋附魔 →整卡悬浮(业主 2026-10-03 规则,ThisOrThat 同款)。
            new EventOption(this, ShootRiver, InitialOptionKey("RIVER"),
                HoverTipFactory.FromCardWithCardHoverTips<CharlotteStrike>()
                    .Concat(HoverTipFactory.FromCardWithCardHoverTips<CharlotteDefend>())
                    .Concat(HoverTipFactory.FromEnchantment<Spiral>())),
            // 选项文案直接点名原版卡「异鸟扑击」→整卡悬浮(业主 2026-10-03 规则,ByrdonisNest 同款)。
            new EventOption(this, ShootBirds, InitialOptionKey("BIRDS"), HoverTipFactory.FromCardWithCardHoverTips<ByrdSwoop>()),
            new EventOption(this, ShootSunset, InitialOptionKey("SUNSET")),
        };
    }

    /// <summary>拍摄河水:选 1 张打击/防御附魔涡旋 1 层(FromDeckForEnchantment 的过滤重载)。</summary>
    private async Task ShootRiver()
    {
        EnchantmentModel spiral = ModelDb.Enchantment<Spiral>();
        CardModel? card = (await CardSelectCmd.FromDeckForEnchantment(base.Owner, spiral, 1,
            c => c is CharlotteStrike or CharlotteDefend,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1))).FirstOrDefault();
        if (card != null)
        {
            CardCmd.Enchant<Spiral>(card, 1m);
        }
        SetEventFinished(PageDescription("RIVER"));
    }

    /// <summary>拍摄飞鸟:1 张原版[异鸟扑击]入手。
    /// 经 RunState.CreateCard 实例化为带 owner 的玩家卡(EventModel.SelectCardsToAddToDeckFromGrid 同款);
    /// 裸 ToMutable() 无 owner,Add 会抛 "it has no owner"(2026-10-02 事故)。
    /// 事件场景没有手牌,「加入手牌」按原版事件惯例落为进卡组+入堆预览。</summary>
    private async Task ShootBirds()
    {
        CardModel swoop = base.Owner.RunState.CreateCard(ModelDb.Card<ByrdSwoop>(), base.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(swoop, PileType.Deck));
        SetEventFinished(PageDescription("BIRDS"));
    }

    /// <summary>拍摄晚霞:3 张卡牌奖励,全部为升级态(统一 Upgrade,不用升级概率 roll)。
    /// CreateForReward 的 r.Card 已是可直接入奖励/入堆的实例,再 ToMutable() 会抛
    /// MutableModelException 把选项卡死(2026-10-02 事故;EndlessConveyor/HeftyTablet 均直用 .Card)。</summary>
    private async Task ShootSunset()
    {
        CardCreationOptions options = CardCreationOptions.ForNonCombatWithDefaultOdds(
            new[] { base.Owner.Character.CardPool });
        List<CardModel> cards = CardFactory.CreateForReward(base.Owner, 3, options)
            .Select(r => r.Card)
            .ToList();
        foreach (CardModel card in cards)
        {
            CardCmd.Upgrade(card);
        }
        await RewardsCmd.OfferCustom(base.Owner,
            new List<Reward> { new CardReward(cards, CardCreationSource.Other, base.Owner, options) });
        SetEventFinished(PageDescription("SUNSET"));
    }
}
