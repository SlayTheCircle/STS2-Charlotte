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
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 动作捕捉(罕见,1 费能力):每当你打出 1 张[留影纪念],抽 1 张牌,消耗 1 张手牌。升级:无变化(两表一致,无升级路径)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class MotionCapture : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<MotionCapturePower>(1m) };

    public MotionCapture()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MotionCapturePower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
    }
}
