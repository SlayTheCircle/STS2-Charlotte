#!/usr/bin/env bash
# 能量球图标:big 256²(战斗能量计/卡面大图标) + text 24²(正文行内费用图标),Navia 同规格。
# 母版:美术素材/图片/能量.png(1254² 透明底)。
set -euo pipefail
cd "$(dirname "$0")/../.."
source scripts/dev-env.sh
SRC="${ART_SOURCE_DIR:?需要 .local-dev.env 的 ART_SOURCE_DIR}"
DST="assets/$MOD_ID/images"
mkdir -p "$DST/energy"
convert "$SRC/图片/能量.png" -resize 256x256 "$DST/energy/charlotte_energy_big.png"
convert "$DST/energy/charlotte_energy_big.png" -resize 24x24 "$DST/energy/charlotte_energy_text.png"
echo "能量球: big 256² + text 24² 就绪"
