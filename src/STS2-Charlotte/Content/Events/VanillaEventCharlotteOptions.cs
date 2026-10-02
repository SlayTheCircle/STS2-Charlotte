using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using CharlotteMod.Content.Characters;
using CharlotteMod.Content.Relics;

namespace CharlotteMod.Content.Events;

/// <summary>
/// 给 5 个原版事件追加夏洛蒂专属选项(设计:特殊事件扩充)。
/// 注入点:EventModel.GenerateInitialOptionsWrapper 基方法后缀——五个目标事件均未覆写它(已核实),
/// Ancient 事件覆写了该方法,天然不受影响。仅当事件归属者是夏洛蒂时追加。
/// 补丁由 ModEntry.Init 显式 Harmony.PatchAll 挂载(带 ModInitializer 的程序集不会自动 PatchAll)。
/// SetEventFinished 是 protected,外部 handler 经缓存的反射委托调用(每次启动仅反射一次)。
/// 设计案事件名对照:透镜高塔=DrowningBeacon(沉没灯塔)、圆桌骑士=RoundTeaParty(圆桌茶会)。
/// </summary>
[HarmonyPatch(typeof(EventModel), "GenerateInitialOptionsWrapper")]
internal static class VanillaEventCharlotteOptions
{
    private static readonly Action<EventModel, LocString>? FinishDelegate = CreateFinishDelegate();

    private static Action<EventModel, LocString>? CreateFinishDelegate()
    {
        MethodInfo? method = typeof(EventModel).GetMethod("SetEventFinished",
            BindingFlags.Instance | BindingFlags.NonPublic, new[] { typeof(LocString) });
        if (method == null)
        {
            Log.Error("[STS2-Charlotte] 找不到 EventModel.SetEventFinished——游戏版本不兼容,原版事件扩充将不可用");
            return null;
        }
        return (Action<EventModel, LocString>)Delegate.CreateDelegate(typeof(Action<EventModel, LocString>), method);
    }

    [HarmonyPostfix]
    private static void AppendCharlotteOptions(EventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        try
        {
            if (FinishDelegate == null || __instance.Owner == null || __instance.Owner.Character is not Charlotte)
            {
                return;
            }
            EventOption? extra = __instance switch
            {
                DrowningBeacon => new EventOption(__instance, () => PolishLensAsync(__instance),
                    "DROWNING_BEACON.pages.INITIAL.options.CHARLOTTE_POLISH_LENS", HoverTipFactory.FromRelic<FresnelLens>()),
                SelfHelpBook => new EventOption(__instance, () => RecordInfoAsync(__instance),
                    "SELF_HELP_BOOK.pages.INITIAL.options.CHARLOTTE_RECORD_INFO"),
                RoundTeaParty => new EventOption(__instance, () => AnalyzeTeaAsync(__instance),
                    "ROUND_TEA_PARTY.pages.INITIAL.options.CHARLOTTE_INCISIVE_ANALYSIS"),
                PotionCourier => new EventOption(__instance, () => WakeHimAsync(__instance),
                    "POTION_COURIER.pages.INITIAL.options.CHARLOTTE_WAKE_HIM", HoverTipFactory.FromRelic<MembershipCard>()),
                CrystalSphere => new EventOption(__instance, () => PawnAsync(__instance),
                    "CRYSTAL_SPHERE.pages.INITIAL.options.CHARLOTTE_PAWN", HoverTipFactory.FromCardWithCardHoverTips<Doubt>()),
                _ => null,
            };
            if (extra == null)
            {
                return;
            }
            if (__result is List<EventOption> list)
            {
                list.Add(extra);
            }
            else
            {
                __result = __result.Append(extra).ToList();
            }
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Charlotte] 原版事件选项追加失败({__instance.GetType().Name}): {e}");
        }
    }

    /// <summary>擦拭镜片:失去初始遗物(温亨廷先生),获得原版菲涅耳透镜。</summary>
    private static async Task PolishLensAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        RelicModel? starter = player.Relics.OfType<MonsieurVerite>().FirstOrDefault();
        if (starter != null)
        {
            await RelicCmd.Remove(starter);
        }
        await RelicCmd.Obtain(ModelDb.Relic<FresnelLens>().ToMutable(), player);
        FinishDelegate!.Invoke(evt, new LocString("events", "DROWNING_BEACON.pages.CHARLOTTE_POLISH_LENS.description"));
    }

    /// <summary>记录有用的信息:选 1 张卡升级(SpiralingWhirlpool 的升级范式)。</summary>
    private static async Task RecordInfoAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        CardModel? card = (await CardSelectCmd.FromDeckForUpgrade(player,
            new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1))).FirstOrDefault();
        if (card != null)
        {
            CardCmd.Upgrade(card);
        }
        FinishDelegate!.Invoke(evt, new LocString("events", "SELF_HELP_BOOK.pages.CHARLOTTE_RECORD_INFO.description"));
    }

    /// <summary>鞭辟入里地分析:获得一瓶随机药水(角色药池+共享药池随机,ForeignBall 的晚宴范式)。</summary>
    private static async Task AnalyzeTeaAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        IEnumerable<MegaCrit.Sts2.Core.Models.PotionModel> pool = player.Character.PotionPool
            .GetUnlockedPotions(player.UnlockState)
            .Concat(ModelDb.PotionPool<MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool>().GetUnlockedPotions(player.UnlockState));
        MegaCrit.Sts2.Core.Models.PotionModel? potion = player.PlayerRng.Rewards.NextItem(pool);
        if (potion != null)
        {
            await RewardsCmd.OfferCustom(player,
                new System.Collections.Generic.List<Reward> { new PotionReward(potion.ToMutable(), player) });
        }
        FinishDelegate!.Invoke(evt, new LocString("events", "ROUND_TEA_PARTY.pages.CHARLOTTE_INCISIVE_ANALYSIS.description"));
    }

    /// <summary>试着唤醒他:失去所有药水,获得原版会员卡。</summary>
    private static async Task WakeHimAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        foreach (var potion in player.Potions.ToList())
        {
            await PotionCmd.Discard(potion);
        }
        await RelicCmd.Obtain(ModelDb.Relic<MembershipCard>().ToMutable(), player);
        FinishDelegate!.Invoke(evt, new LocString("events", "POTION_COURIER.pages.CHARLOTTE_WAKE_HIM.description"));
    }

    /// <summary>抵押:移除卡组 1 张牌,1 张[疑虑]入手,占卜 4 次(PaymentPlan 的水晶球范式)。
    /// 疑虑经 AddCurseToDeck 一条龙(PaymentPlan 同款):裸 ToMutable() 实例无 owner,
    /// CardPileCmd.Add 直接抛 "it has no owner" 炸断事件(2026-10-02 事故);事件场景没有手牌,
    /// 「加入手牌」按原版事件惯例落为进卡组。</summary>
    private static async Task PawnAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        await CardPileCmd.RemoveFromDeck(
            (await CardSelectCmd.FromDeckForRemoval(player, new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1))).ToList());
        await CardPileCmd.AddCurseToDeck<Doubt>(player);
        CrystalSphereMinigame minigame = new CrystalSphereMinigame(player, evt.Rng, 4);
        await minigame.PlayMinigame();
        FinishDelegate!.Invoke(evt, new LocString("events", "CRYSTAL_SPHERE.pages.CHARLOTTE_PAWN.description"));
    }
}
