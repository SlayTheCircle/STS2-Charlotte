using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// [新闻]沐芒公告(0 费技能):给予所有敌人 1 层虚弱,抽 1 张牌。虚无,消耗。
/// </summary>
[RegisterCard(typeof(CharlotteNewsPool))]
public sealed class CharlotteGleamBulletin : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            HashSet<CardKeyword> set = News.KeywordSet();
            set.Add(CardKeyword.Exhaust);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<WeakPower>(1m),
        new CardsVar(1),
    };

    public CharlotteGleamBulletin()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await News.Play(choiceContext, this, async () =>
        {
            foreach (Creature enemy in base.CombatState.Enemies)
            {
                if (enemy.IsAlive)
                {
                    await PowerCmd.Apply<WeakPower>(choiceContext, enemy, base.DynamicVars["WeakPower"].IntValue, base.Owner.Creature, this);
                }
            }
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        });
    }
}
