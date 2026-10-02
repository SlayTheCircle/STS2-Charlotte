using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Rewards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.Events;

/// <summary>
/// 商人的请求(2 层事件,Hive):尖塔商人求广告。
/// 应允=1 件随机稀有度遗物(Bugslayer 的 PullNextRelicFromFront 范式);
/// 拒绝=1 瓶稀有药水(角色药池+共享药池按 Rare 过滤抽取;无 Rare 候选时仅完结,设计未定义空池行为)。
/// </summary>
[RegisterActEvent(typeof(Hive))]
public sealed class MerchantsRequest : CharlotteEventBase
{
    /// <summary>临时肖像:事件图待 Mirror;不覆写时游戏按条目名推导原版 images/events/
    /// 路径,mod 无此文件直接 AssetLoadException 灰屏(2026-10-02 事故)——先借同规格选人背景顶替。</summary>
    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Charlotte/images/characters/charlotte_char_select_bg.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            new EventOption(this, Agree, InitialOptionKey("AGREE")),
            new EventOption(this, Decline, InitialOptionKey("DECLINE")),
        };
    }

    /// <summary>欣然应允:随机稀有度遗物。</summary>
    private async Task Agree()
    {
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
        await RelicCmd.Obtain(relic, base.Owner);
        SetEventFinished(PageDescription("AGREE"));
    }

    /// <summary>委婉拒绝:随机稀有药水。</summary>
    private async Task Decline()
    {
        IEnumerable<MegaCrit.Sts2.Core.Models.PotionModel> rarePool = base.Owner.Character.PotionPool
            .GetUnlockedPotions(base.Owner.UnlockState)
            .Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(base.Owner.UnlockState))
            .Where(p => p.Rarity == PotionRarity.Rare);
        MegaCrit.Sts2.Core.Models.PotionModel? potion = base.Owner.PlayerRng.Rewards.NextItem(rarePool);
        if (potion != null)
        {
            await RewardsCmd.OfferCustom(base.Owner,
                new List<Reward> { new PotionReward(potion.ToMutable(), base.Owner) });
        }
        SetEventFinished(PageDescription("DECLINE"));
    }
}
