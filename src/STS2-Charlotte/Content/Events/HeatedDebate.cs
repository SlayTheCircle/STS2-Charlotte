using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Acts;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.Events;

/// <summary>
/// 激烈的辩论(3 层事件,Glory):两位厨师争烹饪法。
/// 煎制=失 3 最大生命+回 22 生命(DrowningBeacon 的 LoseMaxHp 范式);
/// 清蒸=失 1 瓶药水+得 50 金。药水由工程侧取第一瓶(设计未指定选择方式,待 Mirror 确认);
/// 无药水时选项锁死(null handler 锁定范式,同 ForeignBall 的 GIFT_LOCKED)。
/// </summary>
[RegisterActEvent(typeof(Glory))]
public sealed class HeatedDebate : CharlotteEventBase
{
    private const decimal MaxHpLoss = 3m;
    private const decimal HealAmount = 22m;
    private const decimal GoldGain = 50m;

    /// <summary>临时肖像:事件图待 Mirror;不覆写时游戏按条目名推导原版 images/events/
    /// 路径,mod 无此文件直接 AssetLoadException 灰屏(2026-10-02 事故)——先借同规格选人背景顶替。</summary>
    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Charlotte/images/characters/charlotte_char_select_bg.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> options = new()
        {
            new EventOption(this, Fry, InitialOptionKey("FRY")),
        };
        if (base.Owner.Potions.Any())
        {
            options.Add(new EventOption(this, Steam, InitialOptionKey("STEAM")));
        }
        else
        {
            options.Add(new EventOption(this, null, InitialOptionKey("STEAM_LOCKED")));
        }
        return options;
    }

    /// <summary>煎制:失 3 最大生命,回 22 生命。</summary>
    private async Task Fry()
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, MaxHpLoss, isFromCard: false);
        await CreatureCmd.Heal(base.Owner.Creature, HealAmount);
        SetEventFinished(PageDescription("FRY"));
    }

    /// <summary>清蒸:失第一瓶药水,得 50 金。</summary>
    private async Task Steam()
    {
        var potion = base.Owner.Potions.First();
        await PotionCmd.Discard(potion);
        await PlayerCmd.GainGold(GoldGain, base.Owner);
        SetEventFinished(PageDescription("STEAM"));
    }
}
