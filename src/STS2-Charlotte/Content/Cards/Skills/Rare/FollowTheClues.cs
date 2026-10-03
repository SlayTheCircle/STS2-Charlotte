using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 按图索骥(稀有,0 费技能):从你的抽牌堆、弃牌堆各选择 1 张牌加入你的手牌。消耗。升级:各 2 张。
/// 选出的牌已在战斗牌堆中,必须走通用 CardPileCmd.Add 移堆(vanilla Graveblast 同款)——
/// AddGeneratedCardsToCombat 只接受无堆新卡,喂堆内卡抛 "not allowed to generate cards that
/// already have a pile" 炸断出牌(2026-10-02 事故)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class FollowTheClues : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(1) };

    public FollowTheClues()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int take = base.DynamicVars.Cards.IntValue;
        var drawPrompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        var discardPrompt = new LocString("cards", base.Id.Entry + ".selectionScreenPromptDiscard");
        // 恰好 N 在堆内不足时会被引擎整体跳过,把够数的牌也漏掉——min 钳到现存张数,
        // 有几张拿几张(设计师 2026-10-03 B 批复:「有消耗就消耗没消耗就跳过」同口径)。
        IEnumerable<CardModel> fromDraw = await CardSelectCmd.FromCombatPile(
            choiceContext, PileType.Draw.GetPile(base.Owner), base.Owner,
            new CardSelectorPrefs(drawPrompt, Math.Min(take, PileType.Draw.GetPile(base.Owner).Cards.Count)), null);
        IEnumerable<CardModel> fromDiscard = await CardSelectCmd.FromCombatPile(
            choiceContext, PileType.Discard.GetPile(base.Owner), base.Owner,
            new CardSelectorPrefs(discardPrompt, Math.Min(take, PileType.Discard.GetPile(base.Owner).Cards.Count)), null);
        List<CardModel> taken = fromDraw.Concat(fromDiscard).ToList();
        if (taken.Count > 0)
        {
            await CardPileCmd.Add(taken, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
