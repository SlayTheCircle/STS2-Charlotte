using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Characters;

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

        // 机制关键词注册(stem 必须与 CharlotteKeywords 常量的 KEYWORD_ 后缀一致;
        // 文本在 localization/{lang}/card_keywords.json)。
        ModKeywordRegistry keywords = ModKeywordRegistry.For(ModId);
        keywords.RegisterCardKeywordOwnedByLocNamespace("SNAPSHOT"); // → 留影
        keywords.RegisterCardKeywordOwnedByLocNamespace("FOCUS");    // → 聚焦
        keywords.RegisterCardKeywordOwnedByLocNamespace("NEWS");     // → 新闻

        // 角色资产档案(头像/地图标记/能量球已用自备素材;其余场景仍借原版铁甲,官方 PCK 实测路径)。
        // 自建场景/立绘接入(B10 美术管线)后逐项替换;两处留空会炸的推导路径已按模板注释处理。
        string charlotteEntry = ModContentRegistry.GetCompoundId(ModId, "character", nameof(Content.Characters.Charlotte)).ToLowerInvariant();
        ModContentRegistry.For(ModId).RegisterCharacterAssetReplacement(charlotteEntry, new CharacterAssetProfile(
            new CharacterSceneAssetSet(
                "res://scenes/creature_visuals/ironclad.tscn",
                "res://STS2-Charlotte/scenes/combat/charlotte_energy_counter.tscn",
                "res://scenes/merchant/characters/ironclad_merchant.tscn",
                "res://scenes/rest_site/characters/ironclad_rest_site.tscn"),
            new CharacterUiAssetSet(
                "res://STS2-Charlotte/images/characters/charlotte_character_icon.png",
                "res://STS2-Charlotte/images/characters/charlotte_character_icon_outline.png",
                "res://STS2-Charlotte/scenes/characters/charlotte_icon.tscn",
                "res://scenes/screens/char_select/char_select_bg_ironclad.tscn",
                "res://images/packed/character_select/char_select_ironclad.png",
                "res://images/packed/character_select/char_select_ironclad_locked.png",
                // 转场材质留空会按条目名推导 mod 条目下不存在的 .tres → AssetLoadException 炸开局;先借通用淡入淡出。
                "res://materials/transitions/fade_transition_mat.tres",
                "res://STS2-Charlotte/images/characters/charlotte_map_marker.png"),
            // 出牌轨迹留空会推导 vfx/card_trail_<entry> → NCardFlyVfx._Ready 空引用,牌堆动画卡死;先借铁甲轨迹。
            new CharacterVfxAssetSet("res://scenes/vfx/card_trail_ironclad.tscn"),
            null,
            null,
            null));

        // 自带 Harmony 补丁:ModInitializer 通道与游戏自动 PatchAll 是官方二选一语义——
        // 本类有 [ModInitializer] 则游戏只调 Init() 不再自动 PatchAll;有补丁类时必须在此手动补。
        new Harmony(ModId + ".patches").PatchAll(typeof(ModEntry).Assembly);
    }
}
