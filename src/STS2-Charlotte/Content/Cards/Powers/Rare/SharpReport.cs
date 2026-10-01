using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 切中要害(稀有,2 费能力):每当你给予[gold]聚焦[/gold]，都对全体敌人造成{SharpReportPower:diff()}点伤害。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class SharpReport : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<SharpReportPower>(2m) };

    public SharpReport()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }


    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SharpReportPower>(choiceContext, base.Owner.Creature, base.DynamicVars["SharpReportPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SharpReportPower"].UpgradeValueBy(1m);
    }

}
