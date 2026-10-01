using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.RelicPools;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// 维修工具——温亨廷先生专用版:你每累计花费 300 金币,就获得 1 件随机遗物(Common 池)。
/// 花费追踪经 Content/Patches/RepairToolsLoseGoldPatch(PlayerCmd.LoseGold 无模型钩子,Harmony 收口)。
/// 计数器随存档序列化([SavedProperty])。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
public sealed class RepairTools : CharlotteRelicBase
{
    private const int GrantThreshold = 300;

    private int _goldSpent;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool ShowCounter => true;

    public override int DisplayAmount => GoldSpent;

    [SavedProperty]
    public int GoldSpent
    {
        get => _goldSpent;
        private set
        {
            AssertMutable();
            if (_goldSpent != value)
            {
                _goldSpent = value;
                UpdateDisplay();
            }
        }
    }

    private void UpdateDisplay()
    {
        base.Status = RelicStatus.Active;
        InvokeDisplayAmountChanged();
    }

    /// <summary>由 RepairToolsLoseGoldPatch 在每次 Spent 型失金后调用。</summary>
    public async Task OnGoldSpent(decimal amount, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != base.Owner || amount <= 0m)
        {
            return;
        }
        GoldSpent += (int)amount;
        while (GoldSpent >= GrantThreshold)
        {
            GoldSpent -= GrantThreshold;
            Flash();
            RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Common).ToMutable();
            await RelicCmd.Obtain(relic, base.Owner);
        }
    }
}
