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
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 核验稿件(罕见,1 费攻击):造成 9 点伤害,从弃牌堆中选择至多 2 张费用为 0 的牌加入你的手牌。升级:至多 3 张。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class VerifyCopy : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, ValueProp.Move),
        new CardsVar(2),
    };

    public VerifyCopy()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        IEnumerable<CardModel> taken = await CardSelectCmd.FromCombatPile(
            choiceContext, PileType.Discard.GetPile(base.Owner), base.Owner,
            new CardSelectorPrefs(prompt, 0, base.DynamicVars.Cards.IntValue),
            c => c.EnergyCost.GetResolved() == 0);
        List<CardModel> list = taken.ToList();
        if (list.Count > 0)
        {
            // 弃牌堆选出的牌仍在堆中,在堆卡移动必须走 CardPileCmd.Add
            // (vanilla Dredge 同款;AddGeneratedCardsToCombat 对有堆卡必抛,2026-10-03 审阅 #4)。
            await CardPileCmd.Add(list, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
