"""
XENOASIS — World 1: The Primordial Cradle — Asset Generator
Generates WorldLabs 3D environment and 6 Tripo 3D model assets for World 1.
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

# ─── Tripo API helpers ───────────────────────────────────────────────
def tripo_create_task(prompt, filename):
    """Create a Tripo text-to-model task."""
    payload = {
        "type": "text_to_model",
        "prompt": prompt,
        "model_version": "v2.5-20250123"
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
    print(f"  [TRIPO] Created task {task_id} for {filename}")
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
    """Full pipeline: create task → poll → download."""
    print(f"\n{'='*60}")
    print(f"TRIPO: {filename}")
    print(f"{'='*60}")
    print(f"  Prompt: {prompt[:80]}...")
    task_id = tripo_create_task(prompt, filename)
    result = tripo_poll_task(task_id)
    output_path = os.path.join(TRIPO_DIR, filename)
    tripo_download(result, output_path)
    return output_path


# ─── WorldLabs API helpers ───────────────────────────────────────────
def worldlabs_generate(prompt, display_name):
    """Generate WorldLabs world and download assets."""
    print(f"\n{'='*60}")
    print(f"WORLDLABS: {display_name}")
    print(f"{'='*60}")
    print(f"  Prompt: {prompt[:100]}...")

    payload = {
        "display_name": display_name,
        "model": "marble-1.1",
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
    
    op_id = res["operation_id"]
    print(f"  Operation ID: {op_id}")

    # Poll
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
                pano_path = os.path.join(SKYBOX_DIR, "WorldLabs_PrimordialCradle_Pano.png")
                urllib.request.urlretrieve(pano_url, pano_path)
                size_mb = os.path.getsize(pano_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Panorama: {pano_path} ({size_mb:.1f} MB)")
            
            # Download 3D mesh/splat
            splat_url = assets.get("splat", {}).get("splat_url")
            if splat_url:
                splat_path = os.path.join(WORLDLABS_DIR, "primordial_cradle.spz")
                urllib.request.urlretrieve(splat_url, splat_path)
                size_mb = os.path.getsize(splat_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Gaussian Splat: {splat_path} ({size_mb:.1f} MB)")
            
            collider_url = assets.get("mesh", {}).get("collider_mesh_url")
            if collider_url:
                mesh_path = os.path.join(WORLDLABS_DIR, "primordial_cradle_collider.glb")
                urllib.request.urlretrieve(collider_url, mesh_path)
                size_mb = os.path.getsize(mesh_path) / (1024 * 1024)
                print(f"  [WORLDLABS] [OK] Collider mesh: {mesh_path} ({size_mb:.1f} MB)")
            
            return op


# ─── Asset Definitions ───────────────────────────────────────────────
TRIPO_ASSETS = [
    {
        "filename": "primordial_hydrothermal_chimney.glb",
        "prompt": "A towering hydrothermal vent chimney, conical volcanic rock structure with steaming fissures, encrusted with glowing cyan and copper mineral deposits, game-ready 3D prop, PBR textures, clean topology, single connected mesh."
    },
    {
        "filename": "primordial_stromatolite_colony.glb",
        "prompt": "A fossilized primordial stromatolite colony, dome-shaped layered sedimentary microbial rock with organic ripple texture, weathered ancient basalt base, game-ready asset, PBR material, clean geometry."
    },
    {
        "filename": "primordial_giant_ammonite_rock.glb",
        "prompt": "A massive ancient primordial ammonite spiral shell, iridescent pearlescent nautilus shell embedded in dark volcanic rock, ancient fossil relic, game asset, PBR metallic sheen, high detail."
    },
    {
        "filename": "primordial_prebiotic_crystal_spire.glb",
        "prompt": "A levitating prebiotic kyanite and quartz crystal cluster, sharp hexagonal crystal prisms with a glowing liquid water core trapped inside, alien-terran mineral, game-ready, translucent emissive."
    },
    {
        "filename": "primordial_bioluminescent_lotus.glb",
        "prompt": "An ancient bioluminescent primordial water lotus flower, translucent glowing cyan and teal petals, floating on an organic lily pad, game-ready botanical asset, PBR texture."
    },
    {
        "filename": "primordial_return_monolith.glb",
        "prompt": "An alien-terran obelisk return altar, carved obsidian pillar with glowing gold glyphs holding a concave pedestal for a miniature world sphere, monumental sci-fi artifact, game-ready."
    }
]

WORLDLABS_PROMPT = (
    "A monumental primordial volcanic lagoon on ancient Earth during the Archean eon. "
    "In the foreground, steaming crystal-clear turquoise mineral tide pools rest upon "
    "jet-black wet basalt rock and glowing bioluminescent cyan microbial mats. "
    "Towering hexagonal volcanic basalt columns and obsidian cliffs rise in the midground, "
    "with warm geothermal steam gently venting from mineral fissures. "
    "In the distance, dramatic volcanic spires and a smoking caldera silhouette against "
    "an atmospheric twilight sky with glowing celestial auroral ribbons and soft crepuscular "
    "rays from a faint young sun. Serene primeval ocean sanctuary, photorealistic 8K spatial "
    "depth, highly detailed rock textures and reflective geothermal water."
)


# ─── Thread Workers ──────────────────────────────────────────────────
results = {}
errors = {}

def worker_tripo(asset_def):
    name = asset_def["filename"]
    out_file = os.path.join(TRIPO_DIR, name)
    if os.path.exists(out_file) and os.path.getsize(out_file) > 100000:
        print(f"  [TRIPO] [SKIP] {name} already exists ({os.path.getsize(out_file)/(1024*1024):.1f} MB)")
        results[name] = out_file
        return
    try:
        path = generate_tripo_asset(asset_def["prompt"], name)
        results[name] = path
    except Exception as e:
        errors[name] = str(e)
        print(f"  [TRIPO] [FAIL] {name}: {e}")

def worker_worldlabs():
    pano_path = os.path.join(SKYBOX_DIR, "WorldLabs_PrimordialCradle_Pano.png")
    if os.path.exists(pano_path) and os.path.getsize(pano_path) > 100000:
        print(f"  [WORLDLABS] [SKIP] Panorama already exists ({os.path.getsize(pano_path)/(1024*1024):.1f} MB)")
        results["worldlabs"] = pano_path
        return
    try:
        op = worldlabs_generate(WORLDLABS_PROMPT, "Primordial Cradle - Archean Volcanic Lagoon")
        results["worldlabs"] = op
    except Exception as e:
        errors["worldlabs"] = str(e)
        print(f"  [WORLDLABS] [FAIL]: {e}")


# ─── Main ─────────────────────────────────────────────────────────────
if __name__ == "__main__":
    print("=" * 70)
    print("XENOASIS — World 1: The Primordial Cradle — Asset Generator")
    print("=" * 70)
    print(f"Tripo API Key: {TRIPO_KEY[:8]}...{TRIPO_KEY[-4:]}")
    print(f"WorldLabs API Key: {WORLDLABS_KEY[:8]}...{WORLDLABS_KEY[-4:]}")

    mode = sys.argv[1] if len(sys.argv) > 1 else "all"

    if mode in ("tripo", "all"):
        # Launch all 6 Tripo tasks as threads
        threads = []
        for asset in TRIPO_ASSETS:
            t = threading.Thread(target=worker_tripo, args=(asset,))
            t.start()
            threads.append(t)
            time.sleep(1)  # Stagger requests slightly
        
        if mode == "tripo":
            for t in threads:
                t.join()

    if mode in ("worldlabs", "all"):
        wl_thread = threading.Thread(target=worker_worldlabs)
        wl_thread.start()
        if mode == "worldlabs":
            wl_thread.join()

    if mode == "all":
        # Wait for all threads
        for t in threads:
            t.join()
        wl_thread.join()

    # Summary
    print("\n" + "=" * 70)
    print("GENERATION SUMMARY")
    print("=" * 70)
    for name, path in results.items():
        if name == "worldlabs":
            print(f"  [OK] WorldLabs environment generated successfully")
        else:
            print(f"  [OK] {name}: {path}")
    for name, err in errors.items():
        print(f"  [FAIL] {name}: {err}")
    
    total = len(TRIPO_ASSETS) + 1  # +1 for WorldLabs
    success = len(results)
    failed = len(errors)
    print(f"\nTotal: {total} | Success: {success} | Failed: {failed}")
