# 美术贡献需求

本页是模板骨架：各衍生 Mod 按自身内容填充资源清单。通用规格与管线约定如下。

## 通用规格

| 资源类别 | 规格约定 | 输出路径（相对 assets/&lt;ModId&gt;/） |
|---|---|---|
| 常规卡图 | 25:19 横图窗（参考 750×570） | `images/cards/<ClassName>.png` |
| 先古卡图 | 竖图窗（参考 606×852）；不要把常规比例套到先古卡上 | `images/cards/<ClassName>.png` |
| 遗物 | 256×256 透明底主图 + `_outline` 描边 + 大图 | `images/relics/` |
| 药水 | 256² 主图 + 描边 | `images/potions/` |
| 能力图标 | 256×256 透明底 | `images/powers/<ClassName>.png` |
| 附魔徽记 | 256²；同族效果可共用一枚徽记、以文本区分 | `images/enchantments/` |
| 能量计 | 透明主图；big 与 text 两枚 | `images/energy/` |
| 角色立绘／头像／手势 | 按 CharacterAssetProfile 各槽位规格 | `images/characters/`、`images/hands/` |
| 纪元肖像 | 全局域大图 + 272×174 缩略图 | `assets/global/images/timeline/epoch_portraits/` |

## 管线约定

- 母版不入公开仓，位于组织私有美术仓（`STS2-<Mod>-art`），本地克隆后经 `ART_SOURCE_DIR` 接入；`scripts/prep-art.sh` 调 `scripts/art/` 下脚本派生。
- Base 家族按类名约定解析路径、缺图回退占位；新增资源槽时同步更新 `audit-assets.py` 的检查面与派生脚本映射。
- 工坊封面为独立预览图，不进游戏资源包； Steam 推荐 JPG／PNG。上传图保留标题文字并检查小尺寸辨识度。
- 修改输入映射时同步更新派生脚本和资源使用方。完整素材与 PCK 检查不能代替游戏内验收。

## 0.2.0 联机内容素材

五张新卡图（合影留念、众目睽睽、随行采访、同行特刊、此刻长存）与三枚 Power 图标（众目睽睽、随行采访、同行特刊）已交付并接入，未使用占位图。卡图母版为 1500×1140，图标母版为 1254×1254 透明底正方形；派生尺寸、透明通道及 PCK 加载已验证，游戏内显示仍待试玩验收。精力充沛、真实至上及其 Power 的私有母版保留作历史资料，现行映射、派生资源和 Godot 缓存已清理。
