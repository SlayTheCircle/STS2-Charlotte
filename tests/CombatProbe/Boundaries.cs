using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

// 隔离抽牌动画、原生日志与视觉输出；保留真实模型、克隆、牌堆和能力钩子。
internal static class Boundaries
{
    public static readonly List<Player> Draws = [];
    public static readonly List<decimal> DrawAmounts = [];
    public static readonly List<CardModel> ExhaustedCards = [];
    public static readonly List<Player?> GeneratedCreators = [];

    public static void Install()
    {
        var harmony = new Harmony("charlotte.combat.probe");
        void Patch(MethodBase method, string prefix) => harmony.Patch(method,
            prefix: new HarmonyMethod(typeof(Boundaries), prefix));
        Patch(typeof(Godot.OS).GetMethod("GetCmdlineArgs")!, nameof(CommandLine));
        Patch(typeof(Godot.OS).GetMethod("HasFeature")!, nameof(Feature));
        Patch(typeof(MegaCrit.Sts2.Core.Logging.ConsoleLogPrinter).GetMethod("Print")!, nameof(Skip));
        Patch(typeof(CardPileCmd).GetMethod("Draw", [typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool)])!, nameof(Draw));
#if !MOD_GAME_0107_1
        Patch(typeof(CardPileCmd).GetMethod("DrawWithoutBlockingOnOtherPlayers")!, nameof(DrawWithoutBlocking));
#endif
        Patch(typeof(CardPileCmd).GetMethod("AddGeneratedCardToCombat")!, nameof(AddGenerated));
        Patch(typeof(CreatureCmd).GetMethod("GainBlock", [typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(CardPlay), typeof(bool)])!, nameof(GainBlock));
        Patch(typeof(CardCmd).GetMethod("Exhaust")!, nameof(Exhaust));
        Patch(typeof(CardCmd).GetMethod("Afflict", [typeof(AfflictionModel), typeof(CardModel), typeof(decimal)])!, nameof(Afflict));
        Patch(typeof(CardCmd).GetMethods().Single(m => m.Name == "PreviewCardPileAdd"
            && m.GetParameters()[0].ParameterType == typeof(CardPileAddResult)), nameof(Preview));
        Patch(typeof(PowerCmd).GetMethod("Apply", [typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature),
            typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool)])!, nameof(ApplyPower));
        Patch(typeof(PowerCmd).GetMethod("Remove", [typeof(PowerModel)])!, nameof(RemovePower));
        Patch(typeof(CombatManager).GetProperty("IsInProgress")!.GetMethod!, nameof(InProgress));
        Patch(typeof(CombatManager).GetProperty("IsOverOrEnding")!.GetMethod!, nameof(NotEnding));
        Patch(typeof(CombatManager).GetProperty("IsEnding")!.GetMethod!, nameof(NotEnding));
    }

    public static void Reset()
    {
        Draws.Clear();
        DrawAmounts.Clear();
        ExhaustedCards.Clear();
        GeneratedCreators.Clear();
    }

    public static bool CommandLine(ref string[] __result) { __result = []; return false; }
    public static bool Feature(ref bool __result) { __result = false; return false; }
    public static bool InProgress(ref bool __result) { __result = true; return false; }
    public static bool NotEnding(ref bool __result) { __result = false; return false; }
    public static bool Skip() => false;
    public static bool Preview() => false;

    public static bool Draw(decimal count, Player player, ref Task<IEnumerable<CardModel>> __result)
    {
        Draws.Add(player);
        DrawAmounts.Add(count);
        __result = Task.FromResult(Enumerable.Empty<CardModel>());
        return false;
    }
#if !MOD_GAME_0107_1
    public static bool DrawWithoutBlocking(decimal count, Player player, ref Task __result)
    {
        Draws.Add(player);
        DrawAmounts.Add(count);
        __result = Task.CompletedTask;
        return false;
    }
#endif
    public static bool AddGenerated(CardModel card, PileType newPileType, Player? creator,
        ref Task<CardPileAddResult> __result)
    {
        GeneratedCreators.Add(creator);
        Fixture.AddToPile(card, newPileType);
        __result = Task.FromResult(new CardPileAddResult { success = true, cardAdded = card });
        return false;
    }
    public static bool GainBlock(Creature creature, decimal amount, ref Task<decimal> __result)
    {
        creature.GainBlockInternal(amount);
        __result = Task.FromResult(amount);
        return false;
    }
#if MOD_GAME_0107_1
    public static bool Exhaust(CardModel card, ref Task __result)
    {
        ExhaustedCards.Add(card);
        __result = Task.CompletedTask;
        return false;
    }
#else
    public static bool Exhaust(CardModel card, ref Task<CardPileAddResult?> __result)
    {
        ExhaustedCards.Add(card);
        __result = Task.FromResult<CardPileAddResult?>(null);
        return false;
    }
#endif
    public static bool Afflict(AfflictionModel affliction, CardModel card, decimal amount,
        ref Task<AfflictionModel?> __result)
    {
        card.AfflictInternal(affliction, amount);
        affliction.AfterApplied();
        __result = Task.FromResult(card.Affliction);
        return false;
    }
    public static bool ApplyPower(PowerModel power, Creature target, decimal amount, Creature? applier,
        ref Task __result)
    {
        power.Applier = applier;
        power.ApplyInternal(target, amount);
        __result = Task.CompletedTask;
        return false;
    }
    public static bool RemovePower(PowerModel? power, ref Task __result)
    {
        power?.RemoveInternal();
        __result = Task.CompletedTask;
        return false;
    }
}
