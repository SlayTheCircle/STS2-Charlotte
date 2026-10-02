using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Characters;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 看镜头！(先古,X 费能力):选择 X 张手牌[留影]。在接下来的 X 个回合内,将被消耗的手牌中
/// 随机一张牌升级并返回你的手牌。升级:改为选择。达弗给予的先古卡(尘封魔典确定性候选;
/// 不注册则魔典在两张先古间随机,可能给出属于古老牙齿的笑一个!)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
[RegisterDustyTomeCard(typeof(Charlotte))]
public sealed class LookAtCamera : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    public LookAtCamera()
        : base(-1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue();
        if (x <= 0)
        {
            return;
        }
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        IEnumerable<CardModel> chosen = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, x, x),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);
        List<CardModel> victims = chosen.ToList();
        if (victims.Count == 0)
        {
            return;
        }
        await PowerCmd.Apply<LookAtCameraPower>(choiceContext, base.Owner.Creature, victims.Count, base.Owner.Creature, this);
        if (base.Owner.Creature.GetPower<LookAtCameraPower>() is { } camera)
        {
            camera.ChooseInsteadOfRandom = IsUpgraded;
            var exhausted = new List<CardModel>();
            foreach (CardModel victim in victims)
            {
                exhausted.Add(victim);
                await Snapshot.Card(choiceContext, victim, base.Owner);
            }
            camera.SetCards(exhausted);
        }
    }
}
