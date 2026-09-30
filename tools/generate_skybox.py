import math
import random
from PIL import Image, ImageDraw, ImageFilter

def generate_cosmic_panorama(width=2048, height=1024, output_path="Assets/Textures/Skybox/CosmicVoid_Pano.png"):
    print(f"[*] Generating procedural 360 celestial nebula skybox ({width}x{height})...")
    img = Image.new("RGB", (width, height), (3, 3, 8))
    draw = ImageDraw.Draw(img)

    # 1. Base Gradient (Deep Space Void)
    for y in range(height):
        # Vertical latitude factor (-1 at poles, 0 at equator)
        lat = math.cos((y / height) * math.pi)
        r = int(4 + 8 * (1.0 - abs(lat)))
        g = int(3 + 6 * (1.0 - abs(lat)))
        b = int(12 + 20 * (1.0 - abs(lat)))
        draw.line([(0, y), (width, y)], fill=(r, g, b))

    # 2. Nebula Cloud Blobs (Cyan & Cosmic Violet)
    nebula_layer = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    neb_draw = ImageDraw.Draw(nebula_layer)

    random.seed(42)
    # Cyan Nebula centers
    cyan_centers = [
        (int(width * 0.25), int(height * 0.45), 350, (0, 240, 220, 35)),
        (int(width * 0.30), int(height * 0.50), 240, (0, 200, 255, 45)),
        (int(width * 0.75), int(height * 0.40), 300, (0, 255, 200, 30)),
    ]
    # Violet / Magenta Nebula centers
    violet_centers = [
        (int(width * 0.45), int(height * 0.35), 400, (130, 40, 200, 35)),
        (int(width * 0.50), int(height * 0.42), 260, (200, 50, 160, 40)),
        (int(width * 0.85), int(height * 0.55), 320, (100, 30, 180, 30)),
        (int(width * 0.10), int(height * 0.60), 280, (80, 20, 160, 25)),
    ]

    for cx, cy, rad, col in cyan_centers + violet_centers:
        for r_step in range(rad, 10, -15):
            alpha = int(col[3] * (1.0 - (r_step / rad)**1.5))
            c = (col[0], col[1], col[2], alpha)
            neb_draw.ellipse([cx - r_step, cy - r_step, cx + r_step, cy + r_step], fill=c)

    nebula_layer = nebula_layer.filter(ImageFilter.GaussianBlur(radius=60))
    img.paste(nebula_layer, (0, 0), nebula_layer)

    # 3. Starfield (2500+ crisp stars)
    star_draw = ImageDraw.Draw(img)
    for _ in range(2500):
        sx = random.randint(0, width - 1)
        sy = random.randint(0, height - 1)
        brightness = random.random()

        if brightness > 0.985:
            # Bright star with cross flare
            c = (255, 255, 255)
            star_draw.point((sx, sy), fill=c)
            star_draw.point((sx + 1, sy), fill=(180, 240, 255))
            star_draw.point((sx - 1, sy), fill=(180, 240, 255))
            star_draw.point((sx, sy + 1), fill=(180, 240, 255))
            star_draw.point((sx, sy - 1), fill=(180, 240, 255))
        elif brightness > 0.92:
            # Medium blue/white star
            col = random.choice([(220, 240, 255), (200, 255, 240), (255, 230, 210)])
            star_draw.rectangle([sx, sy, sx + 1, sy + 1], fill=col)
        elif brightness > 0.6:
            # Faint background star
            star_draw.point((sx, sy), fill=(160, 170, 200))
        else:
            # Distant pinprick star
            star_draw.point((sx, sy), fill=(90, 95, 120))

    img.save(output_path, "PNG")
    print(f"[+] Saved breathtaking celestial skybox to {output_path}")

if __name__ == "__main__":
    generate_cosmic_panorama()
