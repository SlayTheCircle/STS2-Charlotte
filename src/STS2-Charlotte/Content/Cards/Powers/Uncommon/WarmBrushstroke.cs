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
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 温馨笔触(罕见,2 费能力):每当你[留影]或给予[聚焦]时,若生命值低于最大生命值的 50%,则恢复 1 点生命。升级:2 点。
/// 双观察者:ISnapshotObserver + IFocusApplyObserver,均由机制收口分发。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class WarmBrushstroke : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<WarmBrushstrokePower>(1m) };

    public WarmBrushstroke()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WarmBrushstrokePower>(choiceContext, base.Owner.Creature, base.DynamicVars["WarmBrushstrokePower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["WarmBrushstrokePower"].UpgradeValueBy(1m);
    }
}
