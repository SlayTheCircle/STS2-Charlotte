using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;

namespace CharlotteMod;

/// <summary>
/// Mod 入口。加载清单见仓库根 STS2-Charlotte.json;本文件只做注册接线,
/// 游戏内容逻辑全部位于 Content/ 下的纯模型类中。
/// </summary>
[ModInitializer("Init")]
public static class ModEntry
{
    public const string ModId = "STS2-Charlotte";

    public static void Init()
    {
        // 归属声明:让本程序集内的 RitsuLib 自动注册特性([RegisterCard]/[RegisterCharacter]/…)正确归属到本 mod。
        // 必须第一条;内容注册全部由各模型类上的特性完成,本入口不再手工登记内容清单。
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, typeof(ModEntry).Assembly);

        // 机制关键词注册(有机制名词时启用;文本在 localization/{lang}/card_keywords.json):
        //   ModKeywordRegistry.For(ModId).RegisterCardKeywordOwnedByLocNamespace("MYMECHANIC");
        // 常量与悬停助手见 Content/Keywords/CharlotteKeywords.cs。

        // 角色资产档案(自建场景/图像后启用;完整接线样例见源工程 STS2-Navia 的 ModEntry):
        //   string entry = ModContentRegistry.GetCompoundId(ModId, "character", nameof(Content.Characters.Charlotte)).ToLowerInvariant();
        //   ModContentRegistry.For(ModId).RegisterCharacterAssetReplacement(entry, new CharacterAssetProfile(...));
        // 两个必借的兜底(缺省推导路径在 mod 条目下不存在,会炸开局/卡死牌堆动画):
        //   转场材质留空会推导 res://materials/transitions/<entry>_transition_mat.tres → AssetLoadException;
        //   出牌轨迹留空会推导 vfx/card_trail_<entry> → NCardFlyVfx._Ready 空引用。
        // 先借原版:res://materials/transitions/fade_transition_mat.tres 与 res://scenes/vfx/card_trail_ironclad.tscn。

        // 自带 Harmony 补丁:ModInitializer 通道与游戏自动 PatchAll 是官方二选一语义——
        // 本类有 [ModInitializer] 则游戏只调 Init() 不再自动 PatchAll;有补丁类时必须在此手动补。
        new Harmony(ModId + ".patches").PatchAll(typeof(ModEntry).Assembly);
    }
}
