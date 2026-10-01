using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// [新闻]寻人启事(1 费技能):给予 6 层[聚焦]。虚无。
/// </summary>
[RegisterCard(typeof(CharlotteNewsPool))]
public sealed class CharlotteMissingPersonNotice : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            HashSet<CardKeyword> set = News.KeywordSet();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<LensFocusPower>(6m),
    };

    public CharlotteMissingPersonNotice()
        : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await News.Play(choiceContext, this, async () =>
        {
            if (cardPlay.Target != null)
            {
                await PowerCmd.Apply<LensFocusPower>(choiceContext, cardPlay.Target, base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
            }
        });
    }
}
