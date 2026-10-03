using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.HoverTips;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 实地考察(罕见,1 费技能):你的抽牌堆中每有 1 张[留影纪念],都会对全体敌人施加 2 层[聚焦]。升级:3 层。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SiteSurvey : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot); // 卡面提及[留影纪念](#63)
            return set;
        }
    }

    // [留影纪念]名词直挂纪念标记悬停(设计师 2026-10-03 D3)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        HoverTipFactory.FromAffliction<SnapshotMemento>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<LensFocusPower>(2m) };

    public SiteSurvey()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState is not { } combatState)
        {
            return;
        }
        int mementos = PileType.Draw.GetPile(base.Owner).Cards.Count(Snapshot.IsMemento);
        if (mementos <= 0)
        {
            return;
        }
        int per = base.DynamicVars["LensFocusPower"].IntValue;
        foreach (Creature enemy in combatState.Enemies)
        {
            if (enemy.IsAlive)
            {
                await PowerCmd.Apply<LensFocusPower>(choiceContext, enemy, per * mementos, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["LensFocusPower"].UpgradeValueBy(1m);
    }
}
