#!/usr/bin/env bash
# 专属事件肖像与纪元大图/缩略图；先核对全部母版，再按字节比较更新。
set -euo pipefail
source "$(dirname "$0")/../dev-env.sh"
SRC="${ART_SOURCE_DIR:?需要配置 ART_SOURCE_DIR}"
source "$MOD_ROOT/scripts/art/common.sh"
source "$MOD_ROOT/scripts/art/validate-sources.sh"

event_sources=('事件-美不胜收' '事件-商人的请求' '事件-激烈的辩论')
event_classes=(BreathtakingView MerchantsRequest HeatedDebate)
epoch_sources=('世界线启程' '世界线1' '世界线2' '世界线3')
for name in "${event_sources[@]}" "${epoch_sources[@]}"; do
    require_art_source "图片/$name.png"
done

DST="$MOD_ROOT/assets/$MOD_ID/images"
DST_G="$MOD_ROOT/assets/global/images/timeline/epoch_portraits"
mkdir -p "$DST/events" "$DST/timeline" "$DST_G"
for i in "${!event_sources[@]}"; do
    convert "$SRC/图片/${event_sources[$i]}.png" -resize 1672x941^ -gravity center -extent 1672x941 \
        "$DST/events/${event_classes[$i]}.png"
done
for i in "${!epoch_sources[@]}"; do
    n=$((i + 1))
    stem="${MOD_ID//-/_}"
    stem="${stem,,}_epoch_$n"
    convert "$SRC/图片/${epoch_sources[$i]}.png" -resize 1672x941^ -gravity center -extent 1672x941 \
        "$DST_G/$stem.png"
    convert "$DST_G/$stem.png" -resize 272x174^ -gravity center -extent 272x174 \
        "$DST/timeline/${stem}_thumb.png"
done
echo '剧情素材: 事件肖像×3、纪元大图×4、缩略图×4 就绪'
