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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 脑洞大开(罕见,1 费技能):消耗你手中的一张[新闻]。接下来的三个回合开始时,
/// 将这张新闻的 0 费、带有消耗的复制品加入你的手牌。升级:四个回合。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Brainstorm : CharlotteCardBase
{
    // 「消耗」为文案中的动作/牌堆引用(非本卡自身关键词),挂词条悬停而非 CanonicalKeywords 横幅(BurningPact 同款,悬浮扫描 2026-10-03)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BrainstormPower>(3m),
    };

    public Brainstorm()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        CardModel? news = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: News.IsNews,
            source: this)).FirstOrDefault();
        if (news == null)
        {
            return;
        }
        await PowerCmd.Apply<BrainstormPower>(choiceContext, base.Owner.Creature, base.DynamicVars["BrainstormPower"].IntValue, base.Owner.Creature, this);
        if (base.Owner.Creature.GetPower<BrainstormPower>() is { } brainstorm)
        {
            brainstorm.SetNews(news);
        }
        await CardCmd.Exhaust(choiceContext, news);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BrainstormPower"].UpgradeValueBy(1m);
    }
}
