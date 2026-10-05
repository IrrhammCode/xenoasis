import os
import math
from PIL import Image, ImageDraw, ImageFont

OUTPUT_DIR = r"C:\Users\Irham\Documents\code\xenoasis\Assets\Textures\Cockpit"
os.makedirs(OUTPUT_DIR, exist_ok=True)

W, H = 2048, 1024

# Palette
COLOR_BG = (6, 12, 22, 255)
COLOR_GRID = (12, 32, 54, 255)
COLOR_GRID_MAJOR = (20, 58, 92, 255)
COLOR_CYAN = (0, 229, 255, 255)
COLOR_CYAN_DIM = (0, 140, 180, 255)
COLOR_CYAN_FAINT = (0, 80, 110, 150)
COLOR_EMERALD = (0, 255, 178, 255)
COLOR_GOLD = (255, 215, 0, 255)
COLOR_ORANGE = (255, 130, 20, 255)
COLOR_WHITE = (235, 245, 255, 255)
COLOR_DEEP_BLUE = (15, 45, 90, 255)

def draw_tech_frame(draw, title, subtitle):
    # Background tech grid
    for x in range(0, W, 64):
        col = COLOR_GRID_MAJOR if x % 256 == 0 else COLOR_GRID
        draw.line([(x, 0), (x, H)], fill=col, width=1 if col == COLOR_GRID else 2)
    for y in range(0, H, 64):
        col = COLOR_GRID_MAJOR if y % 256 == 0 else COLOR_GRID
        draw.line([(0, y), (W, y)], fill=col, width=1 if col == COLOR_GRID else 2)

    # Outer border with sci-fi chamfered corners
    chamfer = 48
    draw.polygon([
        (chamfer, 20), (W - chamfer, 20),
        (W - 20, chamfer), (W - 20, H - chamfer),
        (W - chamfer, H - 20), (chamfer, H - 20),
        (20, H - chamfer), (20, chamfer)
    ], outline=COLOR_CYAN, width=3)

    # Inner decorative border
    draw.polygon([
        (chamfer + 12, 32), (W - chamfer - 12, 32),
        (W - 32, chamfer + 12), (W - 32, H - chamfer - 12),
        (W - chamfer - 12, H - 32), (chamfer + 12, H - 32),
        (32, H - chamfer - 12), (32, chamfer + 12)
    ], outline=COLOR_CYAN_DIM, width=1)

    # Corner brackets
    for cx, cy, dx, dy in [(50, 50, 1, 1), (W - 50, 50, -1, 1), (50, H - 50, 1, -1), (W - 50, H - 50, -1, -1)]:
        draw.line([(cx, cy), (cx + dx * 60, cy)], fill=COLOR_GOLD, width=3)
        draw.line([(cx, cy), (cx, cy + dy * 60)], fill=COLOR_GOLD, width=3)

    # Header bar
    draw.rectangle([(60, 40), (W - 60, 120)], fill=(10, 24, 45, 230), outline=COLOR_CYAN, width=2)
    draw.text((90, 52), title, fill=COLOR_CYAN, font_size=36)
    draw.text((90, 92), subtitle, fill=COLOR_EMERALD, font_size=20)
    
    # Status LED in header
    draw.ellipse([(W - 220, 65), (W - 195, 90)], fill=COLOR_EMERALD, outline=COLOR_WHITE, width=2)
    draw.text((W - 180, 68), "ONLINE // SYNC", fill=COLOR_EMERALD, font_size=22)

    # Footer bar
    draw.rectangle([(60, H - 100), (W - 60, H - 45)], fill=(10, 24, 45, 230), outline=COLOR_CYAN_DIM, width=1)
    draw.text((90, H - 85), "XENOASIS BIO-COCKPIT INTERFACE // ARCHIVE RECEPTION PROTOCOL ACTIVE", fill=COLOR_CYAN_DIM, font_size=20)
    draw.text((W - 340, H - 85), "SEC-07 // TERRAN EXPEDITION", fill=COLOR_GOLD, font_size=20)

def generate_left_scanner():
    img = Image.new("RGBA", (W, H), COLOR_BG)
    draw = ImageDraw.Draw(img)
    draw_tech_frame(draw, "PLANETARY BIOSPHERE & ATMOSPHERE SPECTROGRAPH", "SENSOR TELEMETRY // SOL-3 TERRESTRIAL RECONNAISSANCE")

    # LEFT COLUMN: Atmospheric Composition
    draw.rectangle([(80, 150), (950, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((110, 170), "ATMOSPHERIC COMPOSITION ANALYSIS", fill=COLOR_CYAN, font_size=28)
    draw.line([(110, 210), (920, 210)], fill=COLOR_CYAN_DIM, width=1)

    gases = [
        ("NITROGEN (N2)", 78.08, COLOR_CYAN, "PRIMARY CARRIER GAS - STABLE"),
        ("OXYGEN (O2)", 20.95, COLOR_EMERALD, "PHOTOSYNTHETIC BYPRODUCT - BREATHABLE"),
        ("ARGON (Ar)", 0.93, (140, 220, 255, 255), "NOBLE GAS MATRIX"),
        ("CARBON DIOXIDE (CO2)", 0.04, COLOR_GOLD, "421 PPM - INDUSTRIAL TRACE"),
        ("WATER VAPOR (H2O)", 1.24, (100, 200, 255, 255), "TROPOSPHERIC HUMIDITY DYNAMIC")
    ]

    gy = 230
    for name, pct, col, note in gases:
        draw.text((110, gy), f"{name}: {pct}%", fill=col, font_size=22)
        # Bar background
        draw.rectangle([(110, gy + 28), (920, gy + 52)], fill=(15, 30, 50, 255), outline=COLOR_GRID_MAJOR, width=1)
        # Bar fill
        fill_w = int(110 + (pct / 100.0) * (920 - 110) * (1.0 if pct > 5 else 3.5))
        fill_w = min(fill_w, 920)
        draw.rectangle([(110, gy + 28), (fill_w, gy + 52)], fill=col)
        draw.text((110, gy + 58), note, fill=(180, 210, 230, 200), font_size=15)
        gy += 102

    # Planetary Spheres breakdown
    draw.line([(110, gy + 5), (920, gy + 5)], fill=COLOR_CYAN_DIM, width=1)
    draw.text((110, gy + 15), "TERRAN SPHERES OVERVIEW:", fill=COLOR_CYAN, font_size=20)
    spheres = [
        ("HYDROSPHERE", "70.8% Surface Water Coverage (Liquid H2O)", COLOR_CYAN),
        ("LITHOSPHERE", "Silicate Continental Crust & Tectonic Plates", COLOR_GOLD),
        ("BIOSPHERE", "Dense Carbon Life (Flora, Fauna, Sapient Species #001)", COLOR_EMERALD)
    ]
    sy = gy + 45
    for s_name, s_desc, s_col in spheres:
        draw.text((120, sy), f"> {s_name}: {s_desc}", fill=s_col, font_size=16)
        sy += 28

    # RIGHT COLUMN: Radio / Audio Spectrogram & Live Signals
    draw.rectangle([(980, 150), (1960, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((1010, 170), "ELECTROMAGNETIC SPECTRUM & AUDIO TELEMETRY", fill=COLOR_CYAN, font_size=28)
    draw.line([(1010, 210), (1930, 210)], fill=COLOR_CYAN_DIM, width=1)

    # Waveform display box
    draw.rectangle([(1010, 230), (1930, 520)], fill=(4, 10, 20, 255), outline=COLOR_CYAN_FAINT, width=2)
    
    # Draw waveform
    points = []
    for x in range(1010, 1930, 4):
        rel = (x - 1010) / 920.0
        y = 375 + math.sin(rel * 25.0) * 45.0 + math.sin(rel * 65.0) * 25.0 + math.sin(rel * 130.0) * 12.0
        # Envelope falloff at ends
        env = math.sin(rel * math.pi)
        y = 375 + (y - 375) * env
        points.append((x, y))
    
    # Draw glow underneath wave
    for i in range(len(points) - 1):
        draw.line([points[i], points[i+1]], fill=COLOR_CYAN, width=3)
        draw.line([(points[i][0], points[i][1] + 2), (points[i+1][0], points[i+1][1] + 2)], fill=COLOR_CYAN_DIM, width=1)

    # Central zero line
    draw.line([(1010, 375), (1930, 375)], fill=COLOR_GRID_MAJOR, width=1)
    draw.text((1025, 240), "FREQ: 1420.405 MHz [HYDROGEN LINE RESONANCE]", fill=COLOR_GOLD, font_size=18)
    draw.text((1025, 490), "AMPLITUDE: -14.2 dB // CARRIER DETECTED: HUMAN SYMBOLIC TRANSMISSION", fill=COLOR_EMERALD, font_size=18)

    # Equalizer / Frequency Bins
    by = 550
    draw.text((1010, by), "MULTIBAND HARMONIC SPECTROGRAM:", fill=COLOR_CYAN, font_size=22)
    by += 40
    num_bins = 28
    bin_w = 26
    gap = 7
    start_bx = 1010
    for b in range(num_bins):
        bx = start_bx + b * (bin_w + gap)
        val = (math.sin(b * 0.45) * 0.5 + 0.5) * 0.7 + (math.cos(b * 1.2) * 0.5 + 0.5) * 0.3
        bar_h = int(val * 160)
        col = COLOR_EMERALD if b < 10 else (COLOR_CYAN if b < 20 else COLOR_GOLD)
        draw.rectangle([(bx, by + 180 - bar_h), (bx + bin_w, by + 180)], fill=col)
        draw.rectangle([(bx, by), (bx + bin_w, by + 180)], outline=COLOR_GRID, width=1)

    # Communications Decryption Box
    draw.rectangle([(1010, 780), (1930, 880)], fill=(12, 28, 50, 240), outline=COLOR_GOLD, width=2)
    draw.text((1030, 795), "DIPLOMATIC HANDSHAKE PROTOCOL: INITIALIZED", fill=COLOR_GOLD, font_size=20)
    draw.text((1030, 825), "TERRAN ARCHIVE WELCOME BEACON RECEIVED. ACCESS CLEARANCE: LEVEL OMEGA", fill=COLOR_WHITE, font_size=18)
    draw.text((1030, 852), "ESTIMATED FIRST CONTACT RECEPTION: IMMEDIATE UPON AIRLOCK CYCLE", fill=COLOR_EMERALD, font_size=18)

    path = os.path.join(OUTPUT_DIR, "Cockpit_Monitor_Left_Scan.png")
    img.save(path)
    print("Saved Left Monitor:", path)

def generate_center_flight():
    img = Image.new("RGBA", (W, H), COLOR_BG)
    draw = ImageDraw.Draw(img)
    draw_tech_frame(draw, "TACTICAL DESCENT CORRIDOR // SOL-3 APPROACH", "PRIMARY FLIGHT DISPLAY & TERMINAL GUIDANCE")

    # CENTER RADAR: Massive Circular HUD (cx = 1024, cy = 520, r = 340)
    cx, cy, r = 1024, 520, 340

    # Draw radar backdrop
    draw.ellipse([(cx - r, cy - r), (cx + r, cy + r)], fill=(8, 22, 42, 255), outline=COLOR_CYAN, width=3)
    
    # Concentric rings
    for ring_r, label in [(r, "1000 KM"), (int(r * 0.75), "500 KM"), (int(r * 0.50), "100 KM"), (int(r * 0.25), "25 KM")]:
        draw.ellipse([(cx - ring_r, cy - ring_r), (cx + ring_r, cy + ring_r)], outline=COLOR_CYAN_DIM, width=1)
        draw.text((cx + 10, cy - ring_r + 4), label, fill=COLOR_CYAN_DIM, font_size=14)

    # Crosshair axes
    draw.line([(cx - r - 20, cy), (cx + r + 20, cy)], fill=COLOR_CYAN_DIM, width=2)
    draw.line([(cx, cy - r - 20), (cx, cy + r + 20)], fill=COLOR_CYAN_DIM, width=2)

    # Degree ticks around outer ring
    for deg in range(0, 360, 15):
        rad = math.radians(deg)
        length = 16 if deg % 45 == 0 else 8
        x1 = cx + math.cos(rad) * r
        y1 = cy + math.sin(rad) * r
        x2 = cx + math.cos(rad) * (r - length)
        y2 = cy + math.sin(rad) * (r - length)
        draw.line([(x1, y1), (x2, y2)], fill=COLOR_CYAN, width=2 if deg % 45 == 0 else 1)
        if deg % 45 == 0:
            tx = cx + math.cos(rad) * (r - 35) - 12
            ty = cy + math.sin(rad) * (r - 35) - 8
            draw.text((tx, ty), f"{deg}°", fill=COLOR_GOLD, font_size=14)

    # Wireframe Earth Globe inside radar
    er = int(r * 0.65)
    draw.ellipse([(cx - er, cy - er), (cx + er, cy + er)], outline=COLOR_CYAN, width=2)
    # Latitude lines
    for lat in [-0.6, -0.3, 0.0, 0.3, 0.6]:
        ly = cy + int(lat * er)
        w_at_lat = int(math.sqrt(max(0, er * er - (ly - cy) * (ly - cy))))
        draw.ellipse([(cx - w_at_lat, ly - 8), (cx + w_at_lat, ly + 8)], outline=COLOR_CYAN_FAINT, width=1)
    # Longitude lines
    for lon in [0.25, 0.50, 0.75]:
        ew = int(er * lon)
        draw.ellipse([(cx - ew, cy - er), (cx + ew, cy + er)], outline=COLOR_CYAN_FAINT, width=1)

    # Target Landing Reticle
    tx, ty = cx - 45, cy + 30
    draw.ellipse([(tx - 35, ty - 35), (tx + 35, ty + 35)], outline=COLOR_GOLD, width=3)
    draw.ellipse([(tx - 18, ty - 18), (tx + 18, ty + 18)], outline=COLOR_GOLD, width=2)
    draw.line([(tx - 50, ty), (tx + 50, ty)], fill=COLOR_GOLD, width=2)
    draw.line([(tx, ty - 50), (tx, ty + 50)], fill=COLOR_GOLD, width=2)
    draw.text((tx + 45, ty - 25), "TARGET LOCKED", fill=COLOR_GOLD, font_size=18)
    draw.text((tx + 45, ty), "TERRAN EMBASSY SANCTUARY", fill=COLOR_WHITE, font_size=16)
    draw.text((tx + 45, ty + 20), "SECTOR 07 // DAIS 01", fill=COLOR_EMERALD, font_size=16)

    # LEFT TELEMETRY COLUMN
    draw.rectangle([(80, 150), (620, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((110, 170), "FLIGHT TELEMETRY", fill=COLOR_CYAN, font_size=28)
    draw.line([(110, 210), (590, 210)], fill=COLOR_CYAN_DIM, width=1)

    flight_stats = [
        ("ALTITUDE", "1,420 m", "BAROMETRIC DOCKING HEIGHT", COLOR_CYAN),
        ("DESCENT SPEED", "MACH 0.85", "SUB-SONIC APPROACH GLIDE", COLOR_EMERALD),
        ("DESCENT RATE", "-12.4 m/s", "NOMINAL BRAKING VECTOR", COLOR_CYAN),
        ("RETRO-THRUST", "42.8 %", "STABILIZED INERTIAL LIFT", COLOR_GOLD),
        ("ATMOSPHERIC RES", "0.98 bar", "STANDARD TERRAN SEA LEVEL", COLOR_WHITE),
        ("SURFACE TEMP", "+19.4 °C", "OPTIMAL PLANETARY CLIMATE", COLOR_EMERALD)
    ]
    fy = 230
    for label, val, sub, col in flight_stats:
        draw.text((110, fy), label, fill=COLOR_CYAN_DIM, font_size=16)
        draw.text((110, fy + 22), val, fill=col, font_size=32)
        draw.text((110, fy + 60), sub, fill=(180, 210, 230, 180), font_size=14)
        draw.line([(110, fy + 82), (590, fy + 82)], fill=COLOR_GRID, width=1)
        fy += 105

    # RIGHT TELEMETRY COLUMN: Docking Protocol
    draw.rectangle([(1428, 150), (1968, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((1458, 170), "LANDING PROTOCOL", fill=COLOR_CYAN, font_size=28)
    draw.line([(1458, 210), (1938, 210)], fill=COLOR_CYAN_DIM, width=1)

    protocols = [
        ("TRACTOR BEAM", "ENGAGED // 100%", COLOR_EMERALD, "Guidance lock firmly holding UFO descent"),
        ("CHAMBER ROOF", "IRIS UNLOCKED", COLOR_CYAN, "Grand Welcome Chamber ceiling open"),
        ("AIRLOCK CYCLING", "PRIMED", COLOR_GOLD, "Atmospheric equalization ready"),
        ("DIPLOMATIC ENVOY", "HOST DETECTED", COLOR_CYAN, "Human Holographic Ambassador active"),
        ("DECK STATUS", "ALL SYSTEMS NOMINAL", COLOR_EMERALD, "Inertial dampeners at maximum dampening"),
        ("FIRST CONTACT", "AUTHORIZED", COLOR_GOLD, "Galactic Council Envoy credentials verified")
    ]
    py = 230
    for p_name, p_stat, p_col, p_desc in protocols:
        draw.text((1458, py), p_name, fill=COLOR_CYAN_DIM, font_size=16)
        draw.text((1458, py + 22), p_stat, fill=p_col, font_size=24)
        draw.text((1458, py + 54), p_desc, fill=(180, 210, 230, 180), font_size=14)
        draw.line([(1458, py + 78), (1938, py + 78)], fill=COLOR_GRID, width=1)
        py += 105

    path = os.path.join(OUTPUT_DIR, "Cockpit_Monitor_Center_Flight.png")
    img.save(path)
    print("Saved Center Monitor:", path)

def generate_right_system():
    img = Image.new("RGBA", (W, H), COLOR_BG)
    draw = ImageDraw.Draw(img)
    draw_tech_frame(draw, "SOL-3 ORBITAL DYNAMICS & VESSEL DIAGNOSTICS", "PROPULSION, SHIELD MATRIX & SOLAR NAVIGATION")

    # LEFT HALF: Solar System Orbit Mechanics
    draw.rectangle([(80, 150), (1050, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((110, 170), "SOLAR SYSTEM TRAJECTORY TRACKING", fill=COLOR_CYAN, font_size=28)
    draw.line([(110, 210), (1020, 210)], fill=COLOR_CYAN_DIM, width=1)

    # Draw Solar System Orbits
    sun_x, sun_y = 565, 520
    # Sun
    draw.ellipse([(sun_x - 30, sun_y - 30), (sun_x + 30, sun_y + 30)], fill=COLOR_GOLD, outline=COLOR_ORANGE, width=3)
    draw.text((sun_x - 18, sun_y + 36), "SOL", fill=COLOR_GOLD, font_size=18)

    # Planets
    planet_data = [
        ("MERCURY", 70, 0.45, (180, 180, 180, 255), 5),
        ("VENUS", 130, 2.3, (255, 200, 120, 255), 8),
        ("EARTH [SOL-3]", 210, 3.4, COLOR_CYAN, 10),
        ("MARS", 290, 4.9, COLOR_ORANGE, 7),
        ("JUPITER", 380, 0.9, (230, 180, 140, 255), 16)
    ]

    for p_name, orb_r, angle, p_col, p_size in planet_data:
        # Elliptical orbit
        draw.ellipse([(sun_x - orb_r, sun_y - int(orb_r * 0.45)), (sun_x + orb_r, sun_y + int(orb_r * 0.45))], outline=COLOR_GRID_MAJOR, width=1)
        # Position planet on orbit
        px = sun_x + int(math.cos(angle) * orb_r)
        py = sun_y + int(math.sin(angle) * orb_r * 0.45)
        draw.ellipse([(px - p_size, py - p_size), (px + p_size, py + p_size)], fill=p_col, outline=COLOR_WHITE, width=1)
        draw.text((px + 12, py - 8), p_name, fill=p_col, font_size=14)

    # Interplanetary Arrival Hyperbola to Earth
    earth_px = sun_x + int(math.cos(3.4) * 210)
    earth_py = sun_y + int(math.sin(3.4) * 210 * 0.45)
    # UFO marker at Earth
    draw.ellipse([(earth_px - 18, earth_py - 18), (earth_px + 18, earth_py + 18)], outline=COLOR_EMERALD, width=2)
    draw.text((earth_px - 40, earth_py - 35), "> UFO CRAFT", fill=COLOR_EMERALD, font_size=16)

    # Trajectory curve
    traj_pts = []
    for t in range(0, 100):
        frac = t / 100.0
        tx = 130 + frac * (earth_px - 130)
        ty = 260 + math.sin(frac * math.pi * 0.6) * 350 + (earth_py - 610) * frac
        traj_pts.append((tx, ty))
    for i in range(len(traj_pts) - 1):
        if i % 3 != 0: # dashed
            draw.line([traj_pts[i], traj_pts[i+1]], fill=COLOR_GOLD, width=2)
    draw.text((130, 235), "INTERSTELLAR ENTRY VECTOR // SPEED: 0.12 C", fill=COLOR_GOLD, font_size=16)

    # RIGHT HALF: Ship Diagnostics & Energy Core
    draw.rectangle([(1080, 150), (1960, 900)], fill=(8, 20, 36, 200), outline=COLOR_CYAN_DIM, width=2)
    draw.text((1110, 170), "EXTRATERRESTRIAL VESSEL SYSTEMS", fill=COLOR_CYAN, font_size=28)
    draw.line([(1110, 210), (1930, 210)], fill=COLOR_CYAN_DIM, width=1)

    # Saucer Wireframe Blueprint
    scx, scy = 1520, 360
    # Outer disc
    draw.ellipse([(scx - 220, scy - 70), (scx + 220, scy + 70)], outline=COLOR_CYAN, width=3)
    draw.ellipse([(scx - 170, scy - 50), (scx + 170, scy + 50)], outline=COLOR_CYAN_DIM, width=1)
    # Upper cockpit dome
    draw.ellipse([(scx - 80, scy - 90), (scx + 80, scy - 20)], outline=COLOR_EMERALD, width=2)
    # Emissive ring vents
    draw.ellipse([(scx - 120, scy - 35), (scx + 120, scy + 35)], outline=COLOR_GOLD, width=2)
    draw.text((scx - 120, scy + 85), "UF-772 \"VOYAGER ENVOY\" CLASS", fill=COLOR_CYAN, font_size=18)

    # Diagnostics bars
    sy = 480
    sys_list = [
        ("GRAVITON DEFLECTOR SHIELDS", "100%", COLOR_EMERALD, 1.0),
        ("QUANTUM REACTION DRIVE", "98.4%", COLOR_CYAN, 0.984),
        ("INERTIAL COMPENSATOR MATRIX", "100%", COLOR_EMERALD, 1.0),
        ("ATMOSPHERIC HULL TEMP", "850 K (NOMINAL)", COLOR_CYAN, 0.45),
        ("LIFE SUPPORT & BIO-RESONANCE", "OPTIMAL", COLOR_EMERALD, 1.0),
        ("ENERGY CORE (ZERO-POINT TAP)", "STABILIZED", COLOR_GOLD, 0.94)
    ]
    for s_label, s_val, s_col, s_pct in sys_list:
        draw.text((1110, sy), s_label, fill=COLOR_WHITE, font_size=18)
        draw.text((1820, sy), s_val, fill=s_col, font_size=18)
        draw.rectangle([(1110, sy + 25), (1930, sy + 42)], fill=(12, 28, 48, 255), outline=COLOR_GRID_MAJOR, width=1)
        draw.rectangle([(1110, sy + 25), (int(1110 + s_pct * (1930 - 1110)), sy + 42)], fill=s_col)
        sy += 65

    path = os.path.join(OUTPUT_DIR, "Cockpit_Monitor_Right_System.png")
    img.save(path)
    print("Saved Right Monitor:", path)

def main():
    print("=" * 60)
    print("GENERATING COCKPIT TRIPLE-MONITOR HIGH-DEFINITION HUD TEXTURES")
    print("=" * 60)
    generate_left_scanner()
    generate_center_flight()
    generate_right_system()
    print("[SUCCESS] All 3 Monitor Textures generated!")

if __name__ == "__main__":
    main()
