#!/usr/bin/env bash
# 角色本体资产(B10 正式管线):头像三件 + 立绘 7 姿势 + 选人套图,布局与 Navia scripts/art 同约定。
# 母版:美术素材/图片/头像兼用地图指示.png(600²,Mirror 指定兼作地图标记)
#       美术素材/立绘/立绘-*.png(1024×1536;倒下为横图 1774×887)
#       美术素材/图片/背景大图.png(1672×941) ;选人半身图由站立立绘裁剪(裁框人工校准,勿改自动裁)。
set -euo pipefail
cd "$(dirname "$0")/../.."
source scripts/dev-env.sh
SRC="${ART_SOURCE_DIR:?需要 .local-dev.env 的 ART_SOURCE_DIR}"
DST="assets/$MOD_ID/images"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT
mkdir -p "$DST/characters"

# ---- 头像三件(128² + 金描边 + 地图标记) ----
convert "$SRC/图片/头像兼用地图指示.png" -resize 128x128 "$DST/characters/charlotte_character_icon.png"
# 描边变体:透明外扩 4px → alpha 通道膨胀 Disk:3 → 金色 #f4cf70 填充(Navia 同配方)
convert "$DST/characters/charlotte_character_icon.png" -bordercolor none -border 4 -alpha extract \
    -morphology Dilate Disk:3 -gravity center -crop 128x128+0+0 +repage "$TMP/iconmask.png"
convert -size 128x128 xc:'#f4cf70' "$TMP/iconmask.png" -alpha off -compose CopyOpacity \
    -composite "$DST/characters/charlotte_character_icon_outline.png"
convert "$SRC/图片/头像兼用地图指示.png" -resize 128x128 "$DST/characters/charlotte_map_marker.png"

# ---- 战斗/商店/休息点立绘(原尺寸直入;倒下为横图,attack/skill/hit/down 为姿势储备) ----
convert "$SRC/立绘/立绘-站立.png" "$DST/characters/charlotte_normal.png"
convert "$SRC/立绘/立绘-攻击.png" "$DST/characters/charlotte_attack.png"
convert "$SRC/立绘/立绘-技能.png" "$DST/characters/charlotte_skill.png"
convert "$SRC/立绘/立绘-受击.png" "$DST/characters/charlotte_hit.png"
convert "$SRC/立绘/立绘-倒下.png" "$DST/characters/charlotte_down.png"
convert "$SRC/立绘/立绘-商店.png" "$DST/characters/charlotte_merchant.png"
convert "$SRC/立绘/立绘-火堆.png" "$DST/characters/charlotte_rest_site.png"

# ---- 选人套图:半身裁剪(裁框人工校准 2026-10-02,勿改回自动裁切)+ 灰阶锁定版 + 背景直入 ----
convert "$SRC/立绘/立绘-站立.png" -crop 640x920+240+130 +repage -trim \
    -bordercolor none -border 8 +repage -resize 264x "$DST/characters/charlotte_select.png"
convert "$DST/characters/charlotte_select.png" -modulate 60,0,100 "$DST/characters/charlotte_select_locked.png"
convert "$SRC/图片/背景大图.png" "$DST/characters/charlotte_char_select_bg.png"

echo "角色资产: 头像三件 + 立绘×7 + 选人半身/锁定/背景就绪"
