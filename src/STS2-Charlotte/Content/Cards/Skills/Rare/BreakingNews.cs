using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
/// 爆炸新闻(稀有,3 费技能):获得 9 点格挡。用随机的[新闻]填满你的手牌(至 10 张)。升级:13 格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class BreakingNews : CharlotteCardBase
{
    private const int HandSizeLimit = 10;

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(9m, ValueProp.Move) };

    public BreakingNews()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (base.Owner.Creature.CombatState is not { } combatState)
        {
            return;
        }
        int current = PileType.Hand.GetPile(base.Owner).Cards.Count;
        int toAdd = HandSizeLimit - current;
        for (int i = 0; i < toAdd; i++)
        {
            CardModel news = News.CreateRandom(base.Owner, combatState);
            await CardPileCmd.AddGeneratedCardsToCombat(new[] { news }, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}
