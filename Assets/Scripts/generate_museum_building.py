import os
import sys
import json
import time
import urllib.request
import urllib.error

ENV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", ".env")
API_KEY = None
with open(ENV_PATH, "r") as f:
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


def create_task(task_type: str, params: dict) -> str:
    payload = {"type": task_type, **params}
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(f"{BASE_URL}/task", data=data, headers=HEADERS, method="POST")
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    if result.get("code") != 0:
        raise Exception(f"Task creation failed: {result}")
    return result["data"]["task_id"]


def poll_task(task_id: str, timeout: int = 600) -> dict:
    start = time.time()
    while time.time() - start < timeout:
        req = urllib.request.Request(f"{BASE_URL}/task/{task_id}", headers=HEADERS, method="GET")
        with urllib.request.urlopen(req) as resp:
            result = json.loads(resp.read().decode("utf-8"))
        status = result["data"]["status"]
        progress = result["data"].get("progress", 0)
        print(f"  Status: {status} ({progress}%)", end="\r")
        if status == "success":
            print(f"\n  Task completed successfully!")
            return result["data"]
        elif status in ("failed", "cancelled", "unknown"):
            raise Exception(f"Task failed with status: {status}")
        time.sleep(5)
    raise TimeoutError(f"Task {task_id} timed out after {timeout}s")


def download_model(url: str, output_path: str):
    print(f"  Downloading to: {output_path}")
    urllib.request.urlretrieve(url, output_path)
    size_mb = os.path.getsize(output_path) / (1024 * 1024)
    print(f"  Downloaded: {size_mb:.2f} MB")


def main():
    print("=" * 60)
    print("TRIPO 3D — GENERATING MUSEUM OF HUMANITY BUILDING")
    print("=" * 60)

    prompt = (
        "Grand futuristic museum building of humanity, modern parametric sci-fi architecture, "
        "sweeping curved white titanium composite shell, central circular glass skylight dome atrium, "
        "exhibition wings radiating outward, glowing cyan architectural light conduits, "
        "curved panoramic glass facade, architectural pavilion masterpiece, game-ready 3D asset, clean topology, PBR materials."
    )
    print(f"Prompt: {prompt}\n")

    task_id = create_task("text_to_model", {
        "prompt": prompt,
        "model_version": "v2.5-20250123"
    })
    print(f"Task ID: {task_id}")

    result = poll_task(task_id)

    output_obj = result.get("output", {})
    model_url = output_obj.get("pbr_model") or output_obj.get("model")
    if not model_url and "result" in result:
        res_obj = result["result"]
        model_url = res_obj.get("pbr_model", {}).get("url") or res_obj.get("model", {}).get("url")

    if not model_url:
        raise Exception(f"No model URL found in task result: {result}")

    out_path = os.path.join(MODELS_DIR, "museum_of_humanity_building.glb")
    download_model(model_url, out_path)
    print(f"\n✅ Successfully generated Museum Building: {out_path}")


if __name__ == "__main__":
    main()
