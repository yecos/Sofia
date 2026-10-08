#!/usr/bin/env python3
"""Split the Father rig-parts atlas into clean transparent layers and an OpenRaster master."""
from __future__ import annotations

import json
import math
import zipfile
from collections import defaultdict
from pathlib import Path
from xml.etree.ElementTree import Element, SubElement, tostring

import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont

ROOT = Path(__file__).resolve().parents[2]
BASE = ROOT / "game" / "SofiaUnityProject" / "Assets" / "Sofia" / "VS01" / "Art" / "Layered" / "Father"
SOURCE = BASE / "Father_RigParts_Atlas_v01.png"
PARTS_DIR = BASE / "Parts"
CLEAN_ATLAS = BASE / "Father_RigParts_Atlas_Clean.png"
CONTACT_SHEET = BASE / "Father_RigParts_ContactSheet.png"
ORA_PATH = BASE / "Father_RigParts_Layers.ora"
MANIFEST = BASE / "Father_RigParts_Manifest.json"

NAMES = [
    "Head_Profile",
    "Hair_Back",
    "Hair_Front",
    "Scarf_Collar",
    "Torso_Robe",
    "Pelvis_Belt_Robe",
    "Arm_Near_Upper",
    "Arm_Near_Forearm_Hand",
    "Arm_Far_Upper",
    "Arm_Far_Forearm_Hand",
    "Leg_Near_Pants",
    "Leg_Near_Boot",
    "Leg_Far_Pants",
    "Leg_Far_Boot",
    "Cape_Back_Upper",
    "Cape_Mid_Upper",
    "Cape_Back_Lower",
    "Cape_Mid_Lower",
    "Cape_Front_Upper",
    "Cape_Front_Lower",
]

# Initial pivot suggestions in bottom-left-origin normalized coordinates.
# Fine tune against the assembled character in Unity.
PIVOTS = {
    "Head_Profile": (0.74, 0.12),
    "Hair_Back": (0.82, 0.12),
    "Hair_Front": (0.78, 0.16),
    "Scarf_Collar": (0.50, 0.20),
    "Torso_Robe": (0.50, 0.12),
    "Pelvis_Belt_Robe": (0.52, 0.86),
    "Arm_Near_Upper": (0.80, 0.84),
    "Arm_Near_Forearm_Hand": (0.86, 0.86),
    "Arm_Far_Upper": (0.82, 0.86),
    "Arm_Far_Forearm_Hand": (0.86, 0.87),
    "Leg_Near_Pants": (0.76, 0.90),
    "Leg_Near_Boot": (0.48, 0.84),
    "Leg_Far_Pants": (0.76, 0.90),
    "Leg_Far_Boot": (0.48, 0.84),
    "Cape_Back_Upper": (0.82, 0.88),
    "Cape_Mid_Upper": (0.82, 0.88),
    "Cape_Back_Lower": (0.82, 0.88),
    "Cape_Mid_Lower": (0.82, 0.88),
    "Cape_Front_Upper": (0.82, 0.88),
    "Cape_Front_Lower": (0.82, 0.88),
}

def union_find_components(active: np.ndarray):
    """Run-length connected components (8-connected), avoiding a heavy CV dependency."""
    h, w = active.shape
    parent = [0]
    rank = [0]

    def make():
        n = len(parent)
        parent.append(n)
        rank.append(0)
        return n

    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]
            x = parent[x]
        return x

    def union(a, b):
        a, b = find(a), find(b)
        if a == b:
            return a
        if rank[a] < rank[b]:
            a, b = b, a
        parent[b] = a
        if rank[a] == rank[b]:
            rank[a] += 1
        return a

    all_rows = []
    prev = []
    for y in range(h):
        row = active[y]
        transitions = np.diff(np.pad(row.astype(np.int8), (1, 1)))
        starts = np.flatnonzero(transitions == 1).tolist()
        ends = (np.flatnonzero(transitions == -1) - 1).tolist()
        curr = []
        p_index = 0
        for start, end in zip(starts, ends):
            while p_index < len(prev) and prev[p_index][1] < start - 1:
                p_index += 1
            overlaps = []
            j = p_index
            while j < len(prev) and prev[j][0] <= end + 1:
                overlaps.append(prev[j][2])
                j += 1
            if overlaps:
                label = overlaps[0]
                for other in overlaps[1:]:
                    label = union(label, other)
            else:
                label = make()
            curr.append((start, end, label))
        all_rows.append(curr)
        prev = curr

    components = defaultdict(list)
    for y, runs in enumerate(all_rows):
        for start, end, label in runs:
            components[find(label)].append((y, start, end))
    return components

def clean_transparent_rgb(image: Image.Image) -> Image.Image:
    """Spread nearby painted edge color into transparent texels to prevent bilinear halos."""
    arr = np.array(image.convert("RGBA"), dtype=np.uint8)
    rgb = arr[:, :, :3].astype(np.uint16)
    alpha = arr[:, :, 3]
    h, w = alpha.shape
    # Two pixels of RGB-only extension; alpha remains zero.
    for _ in range(2):
        pending = (alpha == 0)
        sums = np.zeros((h, w, 3), dtype=np.uint32)
        counts = np.zeros((h, w), dtype=np.uint8)
        for dy, dx in ((-1,0),(1,0),(0,-1),(0,1),(-1,-1),(-1,1),(1,-1),(1,1)):
            sy0, sy1 = max(0, -dy), min(h, h-dy)
            sx0, sx1 = max(0, -dx), min(w, w-dx)
            ty0, ty1 = max(0, dy), min(h, h+dy)
            tx0, tx1 = max(0, dx), min(w, w+dx)
            donor = alpha[sy0:sy1, sx0:sx1] > 0
            target_pending = pending[ty0:ty1, tx0:tx1]
            take = donor & target_pending
            if not np.any(take):
                continue
            target_sum = sums[ty0:ty1, tx0:tx1]
            target_count = counts[ty0:ty1, tx0:tx1]
            target_sum[take] += rgb[sy0:sy1, sx0:sx1][take]
            target_count[take] += 1
        filled = (alpha == 0) & (counts > 0)
        rgb[filled] = (sums[filled] // counts[filled, None]).astype(np.uint16)
        alpha[filled] = 0
    arr[:, :, :3] = rgb.astype(np.uint8)
    return Image.fromarray(arr, "RGBA")

def main():
    if not SOURCE.exists():
        raise FileNotFoundError(SOURCE)
    PARTS_DIR.mkdir(parents=True, exist_ok=True)

    src = Image.open(SOURCE).convert("RGBA")
    rgba = np.array(src, dtype=np.uint8)
    alpha = rgba[:, :, 3].copy()
    r, g, b = [rgba[:, :, i].astype(np.int16) for i in range(3)]

    # Remove only highly saturated generator-red/yellow marks within 12 px of transparency.
    # The hue limits keep warm skin, leather, and gold fabric details intact.
    transparent = Image.fromarray((alpha <= 8).astype(np.uint8) * 255, "L")
    near_silhouette_edge = np.array(transparent.filter(ImageFilter.MaxFilter(25))) > 0
    maximum = np.max(rgba[:, :, :3], axis=2).astype(np.float32)
    minimum = np.min(rgba[:, :, :3], axis=2).astype(np.float32)
    delta = maximum - minimum
    hue = np.zeros_like(maximum)
    non_gray = delta > 0
    is_red = (maximum == r) & non_gray
    is_green = (maximum == g) & non_gray
    is_blue = (maximum == b) & non_gray
    hue[is_red] = 60 * np.mod((g[is_red] - b[is_red]) / delta[is_red], 6)
    hue[is_green] = 60 * ((b[is_green] - r[is_green]) / delta[is_green] + 2)
    hue[is_blue] = 60 * ((r[is_blue] - g[is_blue]) / delta[is_blue] + 4)
    saturation = np.divide(delta, maximum, out=np.zeros_like(delta), where=maximum > 0)
    red_fringe = ((hue < 12) | (hue > 348)) & (saturation > 0.85) & (maximum > 180)
    yellow_fringe = (hue > 47) & (hue < 68) & (saturation > 0.85) & (maximum > 180)
    contaminated = near_silhouette_edge & (red_fringe | yellow_fringe)
    alpha[contaminated] = 0
    alpha[alpha <= 2] = 0
    rgba[:, :, 3] = alpha
    clean = Image.fromarray(rgba, "RGBA")

    components = union_find_components(alpha >= 8)
    # Ignore tiny detached specks; the 20 intended cutouts are all substantial.
    components = {k:v for k,v in components.items() if sum(end-start+1 for _,start,end in v) >= 3000}
    if len(components) != len(NAMES):
        raise RuntimeError(f"Expected {len(NAMES)} art pieces, found {len(components)}. Refusing to export a partial rig.")

    h, w = alpha.shape
    cell_w, cell_h = w / 4.0, h / 5.0
    by_cell = {}
    for root, runs in components.items():
        xs = [item[1] for item in runs] + [item[2] for item in runs]
        ys = [item[0] for item in runs]
        x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
        cx, cy = (x0 + x1) * 0.5, (y0 + y1) * 0.5
        col = min(3, int(cx / cell_w))
        row = min(4, int(cy / cell_h))
        key = (row, col)
        if key in by_cell:
            raise RuntimeError(f"More than one art component mapped to atlas cell {key}")
        by_cell[key] = (root, runs, (x0, y0, x1 + 1, y1 + 1), len(runs))

    parts = []
    sheet_cols, sheet_rows = 4, 5
    tile_w, tile_h = 300, 320
    margin = 28
    sheet = Image.new("RGBA", (sheet_cols * tile_w, sheet_rows * tile_h), (27, 31, 37, 255))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.load_default()

    # OpenRaster stack is top-to-bottom. Draw the atlas layers in the same layout.
    ora_layers = []
    atlas_layers = []
    ordered_names = []
    for idx, name in enumerate(NAMES):
        row, col = divmod(idx, 4)
        if (row, col) not in by_cell:
            raise RuntimeError(f"Missing atlas cutout in expected cell {(row, col)} ({name})")
        root, runs, (x0, y0, x1, y1), run_count = by_cell[(row, col)]
        pad = 14
        crop_box = (max(0, x0-pad), max(0, y0-pad), min(w, x1+pad), min(h, y1+pad))
        left, top, right, bottom = crop_box
        component_mask = Image.new("L", (right-left, bottom-top), 0)
        md = ImageDraw.Draw(component_mask)
        for yy, xx0, xx1 in runs:
            if top <= yy < bottom:
                md.line((xx0-left, yy-top, xx1-left, yy-top), fill=255, width=1)
        expanded = component_mask.filter(ImageFilter.MaxFilter(5))
        crop = clean.crop(crop_box)
        crop_alpha = np.array(crop.getchannel("A"), dtype=np.uint8)
        expand_arr = np.array(expanded, dtype=np.uint8) > 0
        crop_alpha[~expand_arr] = 0
        cropped_rgba = np.array(crop, dtype=np.uint8)
        cropped_rgba[:, :, 3] = crop_alpha
        part_img = clean_transparent_rgb(Image.fromarray(cropped_rgba, "RGBA"))

        part_path = PARTS_DIR / f"SPR_Father_{name}.png"
        part_img.save(part_path, optimize=True)
        pivot = PIVOTS[name]
        pixels_per_unit = 260
        parts.append({
            "name": name,
            "unitySprite": f"Assets/Sofia/VS01/Art/Layered/Father/Parts/{part_path.name}",
            "atlasRect": {"x": left, "y": top, "width": right-left, "height": bottom-top},
            "contentBounds": {"x": x0, "y": y0, "width": x1-x0, "height": y1-y0},
            "trimPadding": pad,
            "atlasCell": {"row": row, "column": col},
            "pivotNormalizedBottomLeft": {"x": pivot[0], "y": pivot[1]},
            "initialSortingOrder": len(NAMES)-idx,
            "pixelsPerUnit": pixels_per_unit,
            "areaAtAlpha8": sum(e-s+1 for _,s,e in runs),
            "notes": "Pivot is a first-pass anatomical anchor; adjust against the assembled neutral pose in Unity."
        })

        # Full-canvas layer uses atlas-space placement so the ORA reopens as the source atlas.
        full = Image.new("RGBA", (w,h), (0,0,0,0))
        full.alpha_composite(part_img, (left, top))
        atlas_layers.append((name, full))
        ora_layers.append((name, part_img, left, top))

        # Transparent checkerboard contact sheet with short labels.
        tile = (row * sheet_cols + col)
        tx, ty = col * tile_w, row * tile_h
        checker = Image.new("RGBA", (tile_w, tile_h), (37,42,49,255))
        cd = ImageDraw.Draw(checker)
        for cy in range(0, tile_h, 20):
            for cx in range(0, tile_w, 20):
                if ((cx//20)+(cy//20))%2 == 0:
                    cd.rectangle((cx,cy,cx+19,cy+19), fill=(46,52,60,255))
        fit = part_img.copy()
        fit.thumbnail((tile_w-2*margin, tile_h-56), Image.Resampling.LANCZOS)
        px = (tile_w-fit.width)//2
        py = 8 + (tile_h-52-fit.height)//2
        checker.alpha_composite(fit, (px, py))
        sheet.alpha_composite(checker, (tx,ty))
        draw.text((tx+8,ty+tile_h-34), name.replace("_", " "), fill=(238,240,243,255), font=font)
        ordered_names.append(name)

    clean.save(CLEAN_ATLAS, optimize=True)
    sheet.convert("RGB").save(CONTACT_SHEET, quality=95, optimize=True)

    # Layered OpenRaster master. Every layer is an isolated, positioned RGBA cutout.
    root = Element("image", {
        "version":"0.0.1", "w":str(w), "h":str(h), "name":"Father — layered rig parts v01"
    })
    stack = SubElement(root, "stack")
    data_entries = []
    for i, (name, part_img, x, y) in enumerate(ora_layers):
        filename = f"data/layer{i:02d}.png"
        SubElement(stack, "layer", {
            "name":name, "src":filename, "opacity":"1.0", "visibility":"visible",
            "composite-op":"svg:src-over", "x":str(x), "y":str(y)
        })
        data_entries.append((filename, part_img))
    stack_xml = tostring(root, encoding="utf-8", xml_declaration=True)

    # Flattened preview of the separated pieces in source-atlas positions.
    merged = Image.new("RGBA", (w,h), (0,0,0,0))
    for name, layer, x, y in ora_layers:
        merged.alpha_composite(layer, (x,y))
    preview_bytes = __import__("io").BytesIO()
    merged.save(preview_bytes, format="PNG")
    thumb = merged.copy()
    thumb.thumbnail((256,256), Image.Resampling.LANCZOS)
    thumb_bytes = __import__("io").BytesIO()
    thumb.save(thumb_bytes, format="PNG")

    with zipfile.ZipFile(ORA_PATH, "w") as zf:
        zf.writestr("mimetype", "image/openraster", compress_type=zipfile.ZIP_STORED)
        zf.writestr("stack.xml", stack_xml, compress_type=zipfile.ZIP_DEFLATED)
        zf.writestr("mergedimage.png", preview_bytes.getvalue(), compress_type=zipfile.ZIP_DEFLATED)
        zf.writestr("Thumbnails/thumbnail.png", thumb_bytes.getvalue(), compress_type=zipfile.ZIP_DEFLATED)
        for filename, image in data_entries:
            buf = __import__("io").BytesIO()
            image.save(buf, format="PNG")
            zf.writestr(filename, buf.getvalue(), compress_type=zipfile.ZIP_DEFLATED)

    manifest = {
        "schemaVersion": 1,
        "source": str(SOURCE.relative_to(ROOT)).replace("\\\\","/"),
        "sourceSize": {"width":w, "height":h},
        "partsCount": len(parts),
        "transparencyCleanup": {
            "method": "Highly saturated red/yellow fringe removed within 12px of alpha<=8 silhouette; alpha<=2 cleared; 2px RGB-only edge dilation on isolated parts.",
            "removedPixels": int(np.count_nonzero(contaminated)),
            "componentThresholdAlpha": 8
        },
        "layeredMaster": str(ORA_PATH.relative_to(ROOT)).replace("\\\\","/"),
        "cleanAtlas": str(CLEAN_ATLAS.relative_to(ROOT)).replace("\\\\","/"),
        "contactSheet": str(CONTACT_SHEET.relative_to(ROOT)).replace("\\\\","/"),
        "rigNotes": [
            "Parts are independent painted cutouts in a consistent side-view style.",
            "The .ora master preserves the atlas arrangement for editing; the PNG cutouts are the Unity inputs.",
            "Pivots are anatomical starting points and require visual tuning in the assembled neutral pose.",
            "This source does not yet add hidden overlap extensions or alternate facial-expression layers."
        ],
        "parts": parts
    }
    MANIFEST.write_text(json.dumps(manifest, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps({
        "source": str(SOURCE),
        "dimensions": [w,h],
        "partCount": len(parts),
        "removedFringePixels": int(np.count_nonzero(contaminated)),
        "oraBytes": ORA_PATH.stat().st_size,
        "parts": [{"name":p["name"],"atlasRect":p["atlasRect"],"area":p["areaAtAlpha8"]} for p in parts]
    }, indent=2))

if __name__ == "__main__":
    main()
