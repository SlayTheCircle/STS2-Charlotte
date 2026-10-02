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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 笑一个！(先古,0 费技能):获得 5 点格挡。选择你最多 3 张手牌,将他们[留影]。升级:8 格挡。
/// 奥洛巴斯古老牙齿给予:超越映射挂在 CharlotteKacha 上(其自身进池仅因 CardRarity.Ancient
/// 需在池内,接牙后由 RitsuLib TranscendenceCardsPatch 自动从尘封魔典候选剔除)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Smile : CharlotteCardBase
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(3),
    };

    public Smile()
        : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        IEnumerable<CardModel> chosen = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 0, base.DynamicVars.Cards.IntValue),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);
        foreach (CardModel victim in chosen.ToList())
        {
            await Snapshot.Card(choiceContext, victim, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}
