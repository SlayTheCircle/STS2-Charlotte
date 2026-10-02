#!/usr/bin/env bash
# 角色头像类资产:头像图标 + 程序化描边 + 地图标记,母版一图三用(1080²→128²)。
# 母版:美术素材/图片/头像兼用地图指示.png(600²,Mirror 2026-10-02 指定兼作地图标记)。
# 其余角色场景视觉(战斗立绘/商店/休息点/选人)待 B10 正式美术管线,现仍借原版铁甲。
set -euo pipefail
cd "$(dirname "$0")/../.."
source scripts/dev-env.sh
SRC="${ART_SOURCE_DIR:?需要 .local-dev.env 的 ART_SOURCE_DIR}"
DST="assets/$MOD_ID/images"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT
mkdir -p "$DST/characters"
convert "$SRC/图片/头像兼用地图指示.png" -resize 128x128 "$DST/characters/charlotte_character_icon.png"
# 描边变体:透明外扩 4px → alpha 通道膨胀 Disk:3 → 金色 #f4cf70 填充(Navia 同配方)
convert "$DST/characters/charlotte_character_icon.png" -bordercolor none -border 4 -alpha extract \
    -morphology Dilate Disk:3 -gravity center -crop 128x128+0+0 +repage "$TMP/iconmask.png"
convert -size 128x128 xc:'#f4cf70' "$TMP/iconmask.png" -alpha off -compose CopyOpacity \
    -composite "$DST/characters/charlotte_character_icon_outline.png"
convert "$SRC/图片/头像兼用地图指示.png" -resize 128x128 "$DST/characters/charlotte_map_marker.png"
echo "角色头像: icon 128² + 描边 + 地图标记就绪"
