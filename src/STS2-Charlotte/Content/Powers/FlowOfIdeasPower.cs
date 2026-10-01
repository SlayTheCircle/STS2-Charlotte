using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Powers;

/// <summary>文思泉涌效果:每当你没有手牌时,抽 Amount 张牌(UnceasingTop 同型的 AfterHandEmptied 钩子,含同款阶段守卫)。</summary>
[RegisterPower]
public sealed class FlowOfIdeasPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == base.Owner && IsValidPhase(player.PlayerCombatState.Phase))
        {
            await CardPileCmd.Draw(choiceContext, base.Amount, player);
        }
    }

    /// <remarks>与 UnceasingTop 相同:起手抽牌前/弃牌冲刷后不触发,否则自动打出期间恒空手会连抽。</remarks>
    private static bool IsValidPhase(PlayerTurnPhase phase)
    {
        return (uint)(phase - 2) <= 2u;
    }
}
