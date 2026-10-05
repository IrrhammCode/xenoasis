"""
XENOASIS — World 2: The Forge of Civilization — Asset Generator
Generates WorldLabs 3D environment using marble-1.1-plus / marble-1.1
and 6 Tripo 3D model assets using model_version v3.1-20260211 (detailed geometry & PBR).
"""
import os
import sys
import json
import time
import urllib.request
import urllib.error
import threading
import io

# Fix Windows cp1252 encoding for console output
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace', line_buffering=True)
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace', line_buffering=True)

# ─── Config ───────────────────────────────────────────────────────────
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
ENV_PATH = os.path.join(SCRIPT_DIR, "..", "..", ".env")

TRIPO_KEY = None
WORLDLABS_KEY = None
with open(ENV_PATH, "r") as f:
    for line in f:
        line = line.strip()
        if line.startswith("TRIPO_API_KEY="):
            TRIPO_KEY = line.split("=", 1)[1]
        elif line.startswith("WORLDLABS_API_KEY="):
            WORLDLABS_KEY = line.split("=", 1)[1]

TRIPO_URL = "https://api.tripo3d.ai/v2/openapi"
TRIPO_HEADERS = {
    "Authorization": f"Bearer {TRIPO_KEY}",
    "Content-Type": "application/json"
}

WORLDLABS_URL = "https://api.worldlabs.ai/marble/v1"
WORLDLABS_HEADERS = {
    "WLT-Api-Key": WORLDLABS_KEY,
    "Content-Type": "application/json"
}

TRIPO_DIR = os.path.join(SCRIPT_DIR, "..", "Models", "Tripo")
WORLDLABS_DIR = os.path.join(SCRIPT_DIR, "..", "Models", "WorldLabs")
SKYBOX_DIR = os.path.join(SCRIPT_DIR, "..", "Textures", "Skybox")
os.makedirs(TRIPO_DIR, exist_ok=True)
os.makedirs(WORLDLABS_DIR, exist_ok=True)
os.makedirs(SKYBOX_DIR, exist_ok=True)

# ─── Tripo API helpers (v3.1 Production Model) ───────────────────────
def tripo_create_task(prompt, filename):
    """Create a Tripo text-to-model task using the best v3.1 model."""
    payload = {
        "type": "text_to_model",
        "prompt": prompt,
        "model_version": "v3.1-20260211",
        "geometry_quality": "detailed",
        "texture_quality": "detailed",
        "pbr": True
    }
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        f"{TRIPO_URL}/task", data=data, headers=TRIPO_HEADERS, method="POST"
    )
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    if result.get("code") != 0:
        raise Exception(f"Tripo task creation failed for {filename}: {result}")
    task_id = result["data"]["task_id"]
    print(f"  [TRIPO] Created task {task_id} for {filename} (v3.1 detailed PBR)")
    return task_id


def tripo_poll_task(task_id, timeout=600):
    """Poll a Tripo task until completion."""
    start = time.time()
    while time.time() - start < timeout:
        req = urllib.request.Request(
            f"{TRIPO_URL}/task/{task_id}", headers=TRIPO_HEADERS, method="GET"
        )
        with urllib.request.urlopen(req) as resp:
            result = json.loads(resp.read().decode("utf-8"))
        status = result["data"]["status"]
        progress = result["data"].get("progress", 0)
        if status == "success":
            return result["data"]
        elif status in ("failed", "cancelled", "unknown"):
            raise Exception(f"Tripo task {task_id} failed: {status}")
        time.sleep(8)
    raise TimeoutError(f"Tripo task {task_id} timed out")


def tripo_download(result, output_path):
    """Download model from Tripo result."""
    output_obj = result.get("output", {})
    model_url = output_obj.get("pbr_model") or output_obj.get("model")
    if not model_url and "result" in result:
        res_obj = result["result"]
        if "pbr_model" in res_obj:
            model_url = res_obj["pbr_model"].get("url")
        elif "model" in res_obj:
            model_url = res_obj["model"].get("url")
    if not model_url:
        raise Exception(f"No model URL in Tripo result: {json.dumps(result, indent=2)}")
    urllib.request.urlretrieve(model_url, output_path)
    size_mb = os.path.getsize(output_path) / (1024 * 1024)
    print(f"  [TRIPO] [OK] Downloaded {os.path.basename(output_path)} ({size_mb:.1f} MB)")


def generate_tripo_asset(prompt, filename):
    """Full pipeline: create task -> poll -> download."""
    print(f"\n{'='*60}")
    print(f"TRIPO v3.1: {filename}")
    print(f"{'='*60}")
    print(f"  Prompt: {prompt[:80]}...")
    task_id = tripo_create_task(prompt, filename)
    result = tripo_poll_task(task_id)
    output_path = os.path.join(TRIPO_DIR, filename)
    tripo_download(result, output_path)
    return output_path


# ─── WorldLabs API helpers (marble-1.1-plus) ──────────────────────────
def worldlabs_generate(prompt, display_name):
    """Generate WorldLabs world and download assets."""
    print(f"\n{'='*60}")
    print(f"WORLDLABS: {display_name}")
    print(f"{'='*60}")
    print(f"  Prompt: {prompt[:100]}...")

    # Try marble-1.1-plus first, fallback to marble-1.1 if needed
    for model_name in ["marble-1.1-plus", "marble-1.1"]:
        try:
            payload = {
                "display_name": display_name,
                "model": model_name,
                "world_prompt": {
                    "type": "text",
                    "text_prompt": prompt
                }
            }
            data = json.dumps(payload).encode("utf-8")
            req = urllib.request.Request(
                f"{WORLDLABS_URL}/worlds:generate",
                data=data, headers=WORLDLABS_HEADERS, method="POST"
            )
            with urllib.request.urlopen(req) as resp:
                res = json.loads(resp.read().decode("utf-8"))
            print(f"  [WORLDLABS] Successfully initiated with model: {model_name}")
            break
        except urllib.error.HTTPError as e:
            err_body = e.read().decode("utf-8")
            print(f"  [WORLDLABS] Model {model_name} error ({e.code}): {err_body}")
            if model_name == "marble-1.1":
                raise
            print("  [WORLDLABS] Falling back to marble-1.1...")

    op_id = res["operation_id"]
    print(f"  Operation ID: {op_id}")

    # Poll operation
    while True:
        time.sleep(15)
        req2 = urllib.request.Request(
            f"{WORLDLABS_URL}/operations/{op_id}", headers=WORLDLABS_HEADERS
        )
        with urllib.request.urlopen(req2) as resp2:
            op = json.loads(resp2.read().decode("utf-8"))
        done = op.get("done", False)
        status = op.get("metadata", {}).get("progress", {}).get("status", "IN_PROGRESS")
        print(f"  [WORLDLABS] Status: {status} (done={done})")
        if done:
            if op.get("error"):
                raise Exception(f"WorldLabs failed: {op['error']}")
            response_obj = op.get("response", {})
            assets = response_obj.get("assets", {})
            
            # Download panorama
            pano_url = assets.get("imagery", {}).get("pano_url")
            if pano_url:
                pano_path = os.path.join(SKYBOX_DIR, "WorldLabs_ForgeOfCivilization_Pano.png")
                urllib.request.urlretrieve(pano_url, pano_path)
                size_mb = os.path.getsize(pano_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Panorama: {pano_path} ({size_mb:.1f} MB)")
            
            # Download 3D splat/mesh
            splat_url = assets.get("splat", {}).get("splat_url")
            if splat_url:
                splat_path = os.path.join(WORLDLABS_DIR, "forge_of_civilization.spz")
                urllib.request.urlretrieve(splat_url, splat_path)
                size_mb = os.path.getsize(splat_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Gaussian Splat: {splat_path} ({size_mb:.1f} MB)")
            
            mesh_url = assets.get("mesh", {}).get("mesh_url")
            if mesh_url:
                mesh_path = os.path.join(WORLDLABS_DIR, "forge_of_civilization_collider.glb")
                urllib.request.urlretrieve(mesh_url, mesh_path)
                size_mb = os.path.getsize(mesh_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Mesh Collider: {mesh_path} ({size_mb:.1f} MB)")
            
            return assets


# ─── Asset Definitions for World 2 ───────────────────────────────────
TRIPO_ASSETS = [
    {
        "filename": "civilization_prometheus_hearth.glb",
        "prompt": "An ancient monumental prehistoric ceremonial stone hearth and forge with glowing hot embers, burning logs, carved tribal petroglyphs on weathered basalt stones, iron forge tongs and primitive bronze brackets, realistic embers and ashes, highly detailed game asset, PBR 4k textures"
    },
    {
        "filename": "civilization_paleolithic_flint_anvil.glb",
        "prompt": "A primitive paleolithic stone knapping station with a large mossy boulder anvil, sharp chipped obsidian hand-axe, flint spearheads, antler hammer tool, scattered stone flakes on the ground, prehistoric archaeology artifact, ultra realistic PBR texture"
    },
    {
        "filename": "civilization_bronze_astrolabe_armillary.glb",
        "prompt": "An intricate antique Renaissance brass armillary sphere and astrolabe on an ornate weathered marble pedestal, interlocking concentric astronomical rings, engraved zodiac glyphs, mechanical brass gears, oxidized patina, museum masterpiece, realistic PBR"
    },
    {
        "filename": "civilization_gutenberg_printing_press.glb",
        "prompt": "A 15th century Gutenberg wooden printing press with cast-iron screw mechanism, movable type trays with lead letters, stack of handmade parchment paper, ink dauber balls, realistic aged oak wood and dark wrought iron, historical artifact, detailed PBR"
    },
    {
        "filename": "civilization_silicon_quantum_monolith.glb",
        "prompt": "A futuristic tech monolith altar made of polished dark obsidian with embedded glowing golden silicon wafer microchips, circuit board traces glowing with subtle warm amber and cyan fiber-optic pulses, cybernetic artifact, clean sci-fi aesthetic, detailed PBR"
    },
    {
        "filename": "civilization_return_monolith.glb",
        "prompt": "A tall sacred granite monolith obelisk carved with human writing evolution: cuneiform, Egyptian hieroglyphs, Greek letters, and binary code, featuring a glowing golden brazier with flames at the center holding a floating glass sphere, ancient futuristic portal, detailed PBR"
    }
]

WORLDLABS_PROMPT = (
    "A vast dramatic prehistoric mountain sanctuary and ancient forge valley at sunset. "
    "Towering obsidian and basalt cliff formations, glowing volcanic fissures, stone hearth circles with blazing fires and drifting embers. "
    "Prehistoric megalithic archways, cave entrances glowing with warm firelight, dramatic dusk sky with crimson clouds and rising smoke. "
    "Cinematic lighting, highly detailed 3D environment, realistic rocky textures, atmospheric haze."
)


def main():
    print("=" * 70)
    print("XENOASIS — GENERATING WORLD 2 ASSETS (TRIPO v3.1 + WORLDLABS MARBLE)")
    print("=" * 70)
    
    # Check what already exists
    needed_tripo = []
    for item in TRIPO_ASSETS:
        dest = os.path.join(TRIPO_DIR, item["filename"])
        if os.path.exists(dest) and os.path.getsize(dest) > 1000:
            print(f"  [SKIP] Already exists: {item['filename']} ({os.path.getsize(dest)/(1024*1024):.1f} MB)")
        else:
            needed_tripo.append(item)
    
    # 1. WorldLabs generation in a background thread
    wl_thread = None
    pano_dest = os.path.join(SKYBOX_DIR, "WorldLabs_ForgeOfCivilization_Pano.png")
    if not (os.path.exists(pano_dest) and os.path.getsize(pano_dest) > 10000):
        print("\nStarting WorldLabs generation thread...")
        wl_thread = threading.Thread(
            target=worldlabs_generate,
            args=(WORLDLABS_PROMPT, "Xenoasis_World2_ForgeOfCivilization")
        )
        wl_thread.start()
    else:
        print(f"\n  [SKIP] WorldLabs panorama already exists: {os.path.basename(pano_dest)}")

    # 2. Tripo assets parallel generation
    if needed_tripo:
        print(f"\nStarting parallel generation of {len(needed_tripo)} Tripo assets using v3.1...")
        threads = []
        for item in needed_tripo:
            t = threading.Thread(
                target=generate_tripo_asset,
                args=(item["prompt"], item["filename"])
            )
            threads.append(t)
            t.start()
            time.sleep(2)  # brief stagger to prevent rate-limiting

        for t in threads:
            t.join()
        print("\n[OK] All Tripo v3.1 assets generated successfully!")
    else:
        print("\n[OK] All Tripo assets already exist!")

    # Wait for WorldLabs if running
    if wl_thread:
        print("\nWaiting for WorldLabs generation to complete...")
        wl_thread.join()
        print("[OK] WorldLabs environment generated successfully!")

    print("\n" + "=" * 70)
    print("ALL WORLD 2 ASSETS SUCCESSFULLY GENERATED AND SAVED!")
    print("=" * 70)


if __name__ == "__main__":
    main()
