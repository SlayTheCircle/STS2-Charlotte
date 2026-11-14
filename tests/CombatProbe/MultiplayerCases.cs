using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.Cards;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;
using static Fixture;

internal static class MultiplayerCases
{
    public static async Task Run()
    {
        TestMode.IsOn = true;
        try
        {
            Canonical<CharlotteKacha>();
            Canonical<SnapshotMemento>();
            Canonical<LensFocusPower>();
            Canonical<PublicScrutinyPower>();
            Canonical<EmbeddedInterviewPower>();
            Canonical<JointSpecialIssuePower>();

            ConstraintsAndUpgrades();
            await GroupPhoto();
            await PublicScrutiny();
            await EmbeddedInterview();
            await JointSpecialIssue();
            await MomentEternal();
        }
        finally
        {
            TestMode.IsOn = false;
        }
    }

    private static void ConstraintsAndUpgrades()
    {
        var player = Player();
        CardModel[] cards =
        [
            Card<GroupPhoto>(player), Card<PublicScrutiny>(player), Card<EmbeddedInterview>(player),
            Card<JointSpecialIssue>(player), Card<MomentEternal>(player),
        ];
        Require(cards.All(c => c.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly),
            "Every Mirror addition must be multiplayer-only");

        var group = (GroupPhoto)cards[0];
        group.UpgradeInternal();
        Require(group.IsUpgraded, "Group Photo upgrade state should control Ethereal removal");
        var scrutiny = (PublicScrutiny)cards[1];
        scrutiny.UpgradeInternal();
        Require(scrutiny.DynamicVars["LensFocusPower"].BaseValue == 9
            && scrutiny.DynamicVars["PublicScrutinyPower"].BaseValue == 2, "Public Scrutiny upgrade values");
        var interview = (EmbeddedInterview)cards[2];
        interview.UpgradeInternal();
        Require(interview.DynamicVars.Block.BaseValue == 12
            && interview.DynamicVars["EmbeddedInterviewPower"].BaseValue == 2, "Embedded Interview upgrade values");
        var issue = (JointSpecialIssue)cards[3];
        issue.UpgradeInternal();
        Require(issue.EnergyCost.GetAmountToSpend() == 1
            && issue.DynamicVars["JointSpecialIssuePower"].BaseValue == 6, "Joint Special Issue upgrade values");
        var moment = (MomentEternal)cards[4];
        moment.UpgradeInternal();
        Require(moment.DynamicVars.Block.BaseValue == 6, "Moment Eternal should upgrade Block four to six");
        Console.WriteLine("PASS constraints: five multiplayer-only cards and all upgrade values");
    }

    private static async Task GroupPhoto()
    {
        ResetMultiplayer();
        var a = Player(); var b = Player(); var dead = Player(); Combat(a, b, dead);
        dead.Creature.SetCurrentHpInternal(0);
        var ctx = new ThrowingPlayerChoiceContext();
        var card = Card<GroupPhoto>(a);
        await OnPlay(card, ctx, Play(card, target: b.Creature));
        var generated = PileType.Hand.GetPile(b).Cards.OfType<CharlotteKacha>().ToList();
        Require(generated.Count == 1 && generated[0].Owner == b && !generated[0].IsUpgraded
            && generated[0].EnergyCost.GetWithModifiers(CostModifiers.None) == 0
            && generated[0].Keywords.Contains(CardKeyword.Ethereal),
            "Base Group Photo must give the ally an unupgraded zero-cost Ethereal Kacha");
        Require(Boundaries.GeneratedCreators.SequenceEqual([a])
            && !PileType.Hand.GetPile(a).Cards.OfType<CharlotteKacha>().Any(),
            "Kacha creator and recipient must remain distinct");

        var upgraded = Card<GroupPhoto>(a);
        upgraded.UpgradeInternal();
        await OnPlay(upgraded, ctx, Play(upgraded, target: b.Creature));
        generated = PileType.Hand.GetPile(b).Cards.OfType<CharlotteKacha>().ToList();
        Require(generated.Count == 2 && !generated[1].Keywords.Contains(CardKeyword.Ethereal)
            && generated[1].EnergyCost.GetWithModifiers(CostModifiers.None) == 0,
            "Upgraded Group Photo should remove only Ethereal");

        int before = PileType.Hand.GetPile(a).Cards.Count + PileType.Hand.GetPile(dead).Cards.Count;
        await OnPlay(card, ctx, Play(card, target: a.Creature));
        await OnPlay(card, ctx, Play(card, target: dead.Creature));
        Require(PileType.Hand.GetPile(a).Cards.Count + PileType.Hand.GetPile(dead).Cards.Count == before,
            "Group Photo must reject self and dead targets outside the UI");
        Console.WriteLine("PASS group photo: ownership, creator, zero cost, Ethereal upgrade and target guards");
    }

    private static async Task PublicScrutiny()
    {
        ResetMultiplayer();
        var a = Player(); var b = Player(); var c = Player(); var dead = Player();
        CombatState combat = Combat(a, b, c, dead);
        dead.Creature.SetCurrentHpInternal(0);
        Creature enemy = Enemy(combat); Creature otherEnemy = Enemy(combat);
        var ctx = new ThrowingPlayerChoiceContext();
        var card = Card<PublicScrutiny>(a);
        await OnPlay(card, ctx, Play(card, target: enemy));
        PublicScrutinyPower power = enemy.GetPower<PublicScrutinyPower>()!;
        Require(enemy.GetPowerAmount<LensFocusPower>() == 5 && power.Amount == 2 && power.Applier == a.Creature,
            "Public Scrutiny should apply five Focus and its two-card marker");
        var attack = Card<CharlotteStrike>(b);
        var skill = Card<CharlotteDefend>(b);

        await power.AfterDamageGiven(ctx, b.Creature, Hit(enemy, 0, 99), ValueProp.Move, enemy, attack);
        await power.AfterDamageGiven(ctx, b.Creature, Hit(enemy, 4, 99), ValueProp.Move, enemy, skill);
        await power.AfterDamageGiven(ctx, b.Creature, Hit(otherEnemy, 4, 99), ValueProp.Move, otherEnemy, attack);
        await power.AfterDamageGiven(ctx, a.Creature, Hit(enemy, 4, 99), ValueProp.Move, enemy, attack);
        await power.AfterDamageGiven(ctx, dead.Creature, Hit(enemy, 4, 99), ValueProp.Move, enemy, attack);
        Require(Boundaries.Draws.Count == 0, "Blocked, non-attack, wrong-target, caster and dead hits must not draw");

        await power.AfterDamageGiven(ctx, b.Creature, Hit(enemy, 4, 99), ValueProp.Move, enemy, attack);
        await power.AfterDamageGiven(ctx, b.Creature, Hit(enemy, 4, 99), ValueProp.Move, enemy, attack);
        await power.AfterDamageGiven(ctx, c.Creature, Hit(enemy, 3, 99), ValueProp.Move, enemy, attack);
        Require(Boundaries.Draws.SequenceEqual([b, c]) && Boundaries.DrawAmounts.All(v => v == 2),
            "Each other living player should draw exactly once despite multi-hit and overkill");
        await power.AfterSideTurnEnd(ctx, CombatSide.Player, [a.Creature, b.Creature, c.Creature]);
        Require(enemy.GetPower<PublicScrutinyPower>() == null, "Public Scrutiny marker must expire at player turn end");

        ResetMultiplayer();
        var upgraded = Card<PublicScrutiny>(a);
        upgraded.UpgradeInternal();
        await OnPlay(upgraded, ctx, Play(upgraded, target: otherEnemy));
        Require(otherEnemy.GetPowerAmount<LensFocusPower>() == 9, "Upgraded Public Scrutiny should apply nine Focus");
        Console.WriteLine("PASS public scrutiny: per-player unblocked attack trigger, exclusions and expiry");
    }

    private static async Task EmbeddedInterview()
    {
        ResetMultiplayer();
        var a = Player(); var b = Player(); var c = Player(); var dead = Player(); Combat(a, b, c, dead);
        dead.Creature.SetCurrentHpInternal(0);
        var ctx = new ThrowingPlayerChoiceContext();
        var card = Card<EmbeddedInterview>(a);
        await OnPlay(card, ctx, Play(card, target: b.Creature));
        EmbeddedInterviewPower power = b.Creature.GetPower<EmbeddedInterviewPower>()!;
        Require(b.Creature.Block == 8 && power.Amount == 2 && power.Applier == a.Creature,
            "Embedded Interview should block the ally and retain its interviewer");

        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(c), false);
        Require(Boundaries.Draws.Count == 0, "Another player's exhaust must not trigger Embedded Interview");
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(b), false);
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(b), false);
        Require(Boundaries.Draws.SequenceEqual([a]) && Boundaries.DrawAmounts.SequenceEqual([2m])
            && b.Creature.GetPower<EmbeddedInterviewPower>() == null,
            "Embedded Interview should draw once for the interviewer then remove itself");

        var etherealPower = Power<EmbeddedInterviewPower>(b.Creature, 2, a.Creature);
        var ethereal = Card<CharlotteDefend>(b);
        await etherealPower.AfterCardExhausted(ctx, ethereal, true);
        Require(Boundaries.Draws.Count == 1, "Ethereal exhaust must defer its draw until after the flush");
        await etherealPower.AfterSideTurnEnd(ctx, CombatSide.Player, [b.Creature]);
        Require(Boundaries.Draws.SequenceEqual([a, a]) && Boundaries.DrawAmounts.All(v => v == 2)
            && b.Creature.GetPower<EmbeddedInterviewPower>() == null,
            "Deferred Ethereal draw should resolve once after the flush");

        var upgraded = Card<EmbeddedInterview>(a);
        upgraded.UpgradeInternal();
        await OnPlay(upgraded, ctx, Play(upgraded, target: c.Creature));
        Require(c.Creature.Block == 12, "Upgraded Embedded Interview should grant twelve Block");
        int selfBlock = a.Creature.Block;
        await OnPlay(card, ctx, Play(card, target: a.Creature));
        Require(a.Creature.Block == selfBlock && a.Creature.GetPower<EmbeddedInterviewPower>() == null,
            "Embedded Interview must reject self targets");
        await OnPlay(card, ctx, Play(card, target: dead.Creature));
        Require(dead.Creature.Block == 0 && dead.Creature.GetPower<EmbeddedInterviewPower>() == null,
            "Embedded Interview must reject dead targets");
        a.Creature.SetCurrentHpInternal(0);
        var deadInterviewerPower = Power<EmbeddedInterviewPower>(b.Creature, 2, a.Creature);
        int drawsBeforeDeadInterviewer = Boundaries.Draws.Count;
        await deadInterviewerPower.AfterCardExhausted(ctx, Card<CharlotteDefend>(b), false);
        Require(Boundaries.Draws.Count == drawsBeforeDeadInterviewer,
            "Embedded Interview must not draw for a dead interviewer");
        Console.WriteLine("PASS embedded interview: ally Block, ownership, one-shot and Ethereal deferral");
    }

    private static async Task JointSpecialIssue()
    {
        ResetMultiplayer();
        var a = Player(); var b = Player(); var c = Player(); var dead = Player(); Combat(a, b, c, dead);
        dead.Creature.SetCurrentHpInternal(0);
        var ctx = new ThrowingPlayerChoiceContext();
        var card = Card<JointSpecialIssue>(a);
        await OnPlay(card, ctx, Play(card, target: b.Creature));
        JointSpecialIssuePower power = b.Creature.GetPower<JointSpecialIssuePower>()!;
        Require(power.Amount == 6 && power.Applier == a.Creature,
            "Joint Special Issue should retain its interviewer and six Block");

        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(c), false);
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(a), false);
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(a), false);
        Require(Boundaries.Draws.SequenceEqual([b]) && Boundaries.DrawAmounts.SequenceEqual([1m]),
            "Only the interviewer's first exhaust should make the ally draw");
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(b), false);
        await power.AfterCardExhausted(ctx, Card<CharlotteDefend>(b), false);
        Require(a.Creature.Block == 6 && b.Creature.Block == 6 && c.Creature.Block == 6 && dead.Creature.Block == 0,
            "Only the ally's first exhaust should grant all living players six Block");

        await power.BeforeSideTurnStart(ctx, CombatSide.Player, [a.Creature, b.Creature, c.Creature], null!);
        var ethereal = Card<CharlotteDefend>(a);
        await power.AfterCardExhausted(ctx, ethereal, true);
        Require(Boundaries.Draws.Count == 1, "Joint Special Issue must defer Ethereal-triggered draw");
        await power.AfterSideTurnEnd(ctx, CombatSide.Player, [b.Creature]);
        Require(Boundaries.Draws.SequenceEqual([b, b]) && Boundaries.DrawAmounts.All(v => v == 1),
            "Joint Special Issue should reset and resolve its deferred draw after the flush");
        await OnPlay(card, ctx, Play(card, target: a.Creature));
        await OnPlay(card, ctx, Play(card, target: dead.Creature));
        Require(a.Creature.GetPower<JointSpecialIssuePower>() == null
            && dead.Creature.GetPower<JointSpecialIssuePower>() == null,
            "Joint Special Issue must reject self and dead targets");
        Console.WriteLine("PASS joint special issue: independent triggers, reset, deferral and living-player Block");
    }

    private static async Task MomentEternal()
    {
        ResetMultiplayer();
        var a = Player(); var b = Player(); var dead = Player(); Combat(a, b, dead);
        dead.Creature.SetCurrentHpInternal(0);
        var moment = Card<MomentEternal>(a, PileType.Play);
        var normal = Card<CharlotteStrike>(a);
        Card<Dazed>(a); Card<Regret>(a);
        await OnPlay(moment, new ThrowingPlayerChoiceContext(), Play(moment));
        Require(Boundaries.ExhaustedCards.Count == 3, "Moment Eternal should Snapshot all three initial hand cards");
        Require(a.Creature.Block == 12 && b.Creature.Block == 12 && dead.Creature.Block == 0,
            "Each normal, Status and Curse Snapshot should grant all living players four Block");
        var mementos = PileType.Draw.GetPile(a).Cards.Where(Snapshot.IsMemento).ToList();
        Require(mementos.Count == 1 && mementos[0].Keywords.Contains(CardKeyword.Exhaust),
            "Only the normal card should create a memento; Status and Curse still count as Snapshots");
        Require(Boundaries.Draws.SequenceEqual([a, a]) && Boundaries.DrawAmounts.All(v => v == 1),
            "Status and Curse Snapshot should preserve their normal one-card draws");

        ResetMultiplayer();
        var upgradedA = Player(); var upgradedB = Player(); var upgradedDead = Player();
        Combat(upgradedA, upgradedB, upgradedDead);
        upgradedDead.Creature.SetCurrentHpInternal(0);
        var upgraded = Card<MomentEternal>(upgradedA, PileType.Play);
        upgraded.UpgradeInternal();
        Card<CharlotteStrike>(upgradedA);
        await OnPlay(upgraded, new ThrowingPlayerChoiceContext(), Play(upgraded));
        Require(upgradedA.Creature.Block == 6 && upgradedB.Creature.Block == 6 && upgradedDead.Creature.Block == 0,
            "Upgraded Moment Eternal should grant six Block per Snapshot");

        ResetMultiplayer();
        var emptyA = Player(); var emptyB = Player(); Combat(emptyA, emptyB);
        var empty = Card<MomentEternal>(emptyA, PileType.Play);
        await OnPlay(empty, new ThrowingPlayerChoiceContext(), Play(empty));
        Require(emptyA.Creature.Block == 0 && emptyB.Creature.Block == 0 && Boundaries.ExhaustedCards.Count == 0,
            "Moment Eternal with an empty hand should be a no-op");
        Console.WriteLine("PASS moment eternal: initial hand, Status/Curse rules, living-player Block and upgrade");
    }

    private static DamageResult Hit(Creature target, int unblocked, int overkill) =>
        new(target, ValueProp.Move) { UnblockedDamage = unblocked, OverkillDamage = overkill };

    private static void ResetMultiplayer() => Boundaries.Reset();
}
