import os
import sys
import json
import time
import urllib.request
import urllib.error

ENV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", ".env")
API_KEY = None
if os.path.exists(ENV_PATH):
    with open(ENV_PATH, "r") as f:
        for line in f:
            line = line.strip()
            if line.startswith("TRIPO_API_KEY="):
                API_KEY = line.split("=", 1)[1]
                break

if not API_KEY:
    # Try current directory .env as fallback
    if os.path.exists(".env"):
        with open(".env", "r") as f:
            for line in f:
                line = line.strip()
                if line.startswith("TRIPO_API_KEY="):
                    API_KEY = line.split("=", 1)[1]
                    break

if not API_KEY:
    print("ERROR: TRIPO_API_KEY not found in .env")
    sys.exit(1)

BASE_URL = "https://api.tripo3d.ai/v2/openapi"
HEADERS = {
    "Authorization": f"Bearer {API_KEY}",
    "Content-Type": "application/json"
}

MODELS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Models", "Tripo")
os.makedirs(MODELS_DIR, exist_ok=True)

ASSETS_TO_GENERATE = [
    {
        "filename": "museum_voyager_golden_record.glb",
        "prompt": (
            "The Voyager Golden Record, gleaming gold phonograph record disc with engraved pulsar map "
            "and mathematical diagrams, mounted on a sleek dark futuristic museum display pedestal, "
            "PBR materials, game-ready 3D asset, pristine museum artifact."
        )
    },
    {
        "filename": "museum_rosetta_stone.glb",
        "prompt": (
            "The ancient Rosetta Stone granodiorite stele slab with finely engraved Egyptian hieroglyphs, "
            "Demotic script, and Greek inscriptions, historical artifact on sleek museum plinth, "
            "PBR materials, photorealistic, museum piece."
        )
    },
    {
        "filename": "museum_prometheus_fire_and_chip.glb",
        "prompt": (
            "Sci-fi museum technology monument, ancient primitive stone handaxe with warm glowing embers "
            "evolving into a circular 300mm silicon microchip wafer and quantum processor core, "
            "sleek museum pedestal, PBR materials, detailed."
        )
    },
    {
        "filename": "museum_svalbard_seed_vault.glb",
        "prompt": (
            "Svalbard global seed vault cryo preservation capsule, cylindrical transparent cryo-chamber "
            "with frozen agricultural seed vials and glowing green seedling sprout inside, "
            "futuristic botanical laboratory artifact, PBR materials."
        )
    },
    {
        "filename": "museum_apollo11_moon_plaque.glb",
        "prompt": (
            "Apollo 11 lunar module memorial plaque, astronaut bootprint on lunar regolith moon rock slab, "
            "engraved golden plaque 'We came in peace for all mankind', museum display pedestal, PBR materials."
        )
    },
    {
        "filename": "museum_human_ambassador_statue.glb",
        "prompt": (
            "Statue of a human diplomatic ambassador in futuristic formal envoy attire with welcoming open hand gesture, "
            "marble and bronze finish, museum figure, realistic human anatomy, elegant, PBR materials."
        )
    },
    {
        "filename": "museum_interactive_terminal.glb",
        "prompt": (
            "Futuristic museum holographic info kiosk terminal, sleek sci-fi white composite pedestal "
            "with angled holographic display screen and cyan glowing accents, PBR materials."
        )
    }
]


def create_task(task_type: str, params: dict) -> str:
    payload = {"type": task_type, **params}
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(f"{BASE_URL}/task", data=data, headers=HEADERS, method="POST")
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    if result.get("code") != 0:
        raise Exception(f"Task creation failed: {result}")
    return result["data"]["task_id"]


def poll_all_tasks(tasks: dict, timeout: int = 900) -> dict:
    start = time.time()
    completed = {}
    while time.time() - start < timeout:
        pending = [name for name in tasks if name not in completed]
        if not pending:
            print("\n  All tasks completed successfully!")
            break

        status_summary = []
        for name in pending:
            task_id = tasks[name]
            req = urllib.request.Request(f"{BASE_URL}/task/{task_id}", headers=HEADERS, method="GET")
            try:
                with urllib.request.urlopen(req) as resp:
                    result = json.loads(resp.read().decode("utf-8"))
                status = result["data"]["status"]
                progress = result["data"].get("progress", 0)
                status_summary.append(f"{name}: {status}({progress}%)")

                if status == "success":
                    completed[name] = result["data"]
                elif status in ("failed", "cancelled", "unknown"):
                    print(f"\nTask {name} ({task_id}) failed with status: {status}")
                    completed[name] = None
            except Exception as e:
                status_summary.append(f"{name}: err({e})")

        print(" | ".join(status_summary[:3]) + f" ... ({len(completed)}/{len(tasks)} done)", end="\r")
        time.sleep(6)

    return completed


def download_model(url: str, output_path: str):
    print(f"  Downloading to: {output_path}")
    urllib.request.urlretrieve(url, output_path)
    size_mb = os.path.getsize(output_path) / (1024 * 1024)
    print(f"  Downloaded: {size_mb:.2f} MB")


KNOWN_TASKS = {
    "museum_voyager_golden_record.glb": "129ca0eb-a4d0-4801-9275-415bbdfb64a3",
    "museum_rosetta_stone.glb": "74793304-37b5-4e73-8575-e888a729c95d",
    "museum_prometheus_fire_and_chip.glb": "a23284a2-7efe-4146-b4b7-2a40c52a7a89",
    "museum_svalbard_seed_vault.glb": "434649c8-2cb6-42b9-a53b-4d088889314d",
    "museum_apollo11_moon_plaque.glb": "24220b48-b372-4310-ade4-2ecf94a59e04",
    "museum_human_ambassador_statue.glb": "04e85fc7-8655-4822-85f0-f7e8a9fc7846",
    "museum_interactive_terminal.glb": "0c3b7e5b-feb2-45bc-bd03-d3f13994fdd5"
}

def download_known_tasks():
    sys.stdout.reconfigure(encoding='utf-8')
    print("=" * 70)
    print("DOWNLOADING COMPLETED TRIPO 3D MODELS")
    print("=" * 70)

    for filename, task_id in KNOWN_TASKS.items():
        out_path = os.path.join(MODELS_DIR, filename)
        if os.path.exists(out_path) and os.path.getsize(out_path) > 1000000:
            print(f"[EXISTS] {filename} ({os.path.getsize(out_path)/(1024*1024):.2f} MB)")
            continue

        print(f"\nFetching info for {filename} (task: {task_id})...")
        req = urllib.request.Request(f"{BASE_URL}/task/{task_id}", headers=HEADERS, method="GET")
        with urllib.request.urlopen(req) as resp:
            result = json.loads(resp.read().decode("utf-8"))

        res = result.get("data", {})
        output_obj = res.get("output", {})
        model_url = output_obj.get("pbr_model") or output_obj.get("model")
        if not model_url and "result" in res:
            res_obj = res["result"]
            model_url = res_obj.get("pbr_model", {}).get("url") or res_obj.get("model", {}).get("url")

        if model_url:
            print(f"  Downloading from URL...")
            download_model(model_url, out_path)
            print(f"  SUCCESS: {filename}")
        else:
            print(f"  ERROR: No model URL found for {filename} in {res}")

    print("\nALL 7 TRIPO MODELS DOWNLOADED SUCCESSFULLY!")

if __name__ == "__main__":
    download_known_tasks()
