import os
import sys
import json
import time
import urllib.request
import urllib.error

ENV_PATH = r"C:\Users\Irham\Documents\code\xenoasis\.env"
API_KEY = None
if os.path.exists(ENV_PATH):
    with open(ENV_PATH, "r") as f:
        for line in f:
            line = line.strip()
            if line.startswith("TRIPO_API_KEY="):
                API_KEY = line.split("=", 1)[1].strip()
                break

if not API_KEY:
    print("ERROR: TRIPO_API_KEY not found in .env")
    sys.exit(1)

BASE_URL = "https://api.tripo3d.ai/v2/openapi"
HEADERS = {
    "Authorization": f"Bearer {API_KEY}",
    "Content-Type": "application/json"
}

MODELS_DIR = r"C:\Users\Irham\Documents\code\xenoasis\Assets\Models\Tripo"
os.makedirs(MODELS_DIR, exist_ok=True)

PROMPT = (
    "Full body 3D character model of an extraterrestrial alien diplomat and cosmic voyager, masterpiece. "
    "Tall, slender, graceful humanoid alien anatomy with smooth pearlescent dark obsidian-blue iridescent skin, "
    "intricate glowing cyan and turquoise bioluminescent runic energy circuits along the arms, neck, and chest. "
    "Slender elongated four-fingered alien hands with luminous glowing energy fingertips. "
    "Noble elongated alien cranium with smooth aerodynamic cranial crest, glowing starlight cyan almond eyes radiating "
    "wisdom, intelligence, and peace, serene noble alien face. Wearing ceremonial galactic envoy diplomatic robes "
    "and sleek high-tech exosuit armor: flowing iridescent fabrics layered with lightweight dark titanium plates and golden metallic trim, "
    "soft glowing emissive energy lines along armor seams. Standing in a balanced noble standing pose, "
    "game-ready 3D character asset, clean manifold geometry, detailed PBR textures, emissive map, 8k render."
)

def create_task(prompt: str) -> str:
    payload = {
        "type": "text_to_model",
        "prompt": prompt,
        "model_version": "v2.5-20250123",
        "geometry_quality": "detailed",
        "texture_quality": "detailed",
        "pbr": True
    }
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(f"{BASE_URL}/task", data=data, headers=HEADERS, method="POST")
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    if result.get("code") != 0:
        raise Exception(f"Task creation failed: {result}")
    return result["data"]["task_id"]

def poll_task(task_id: str, timeout: int = 600) -> dict:
    start = time.time()
    print(f"[*] Polling Tripo 3D task {task_id}...")
    while time.time() - start < timeout:
        req = urllib.request.Request(f"{BASE_URL}/task/{task_id}", headers=HEADERS, method="GET")
        with urllib.request.urlopen(req) as resp:
            result = json.loads(resp.read().decode("utf-8"))
        status = result["data"]["status"]
        progress = result["data"].get("progress", 0)
        print(f"  [Progress] Status: {status} ({progress}%)")
        if status == "success":
            print(f"  [+] Task completed successfully!")
            return result["data"]
        elif status in ("failed", "cancelled", "unknown"):
            raise Exception(f"Task failed with status: {status}")
        time.sleep(4)
    raise TimeoutError(f"Task {task_id} timed out after {timeout}s")

def download_model(url: str, output_path: str):
    print(f"[*] Downloading GLB model from: {url}")
    print(f"[*] Saving to: {output_path}")
    urllib.request.urlretrieve(url, output_path)
    size_mb = os.path.getsize(output_path) / (1024 * 1024)
    print(f"[+] Download complete! File size: {size_mb:.2f} MB")

def main():
    print("=" * 70)
    print("  TRIPO 3D — GENERATING EXTRATERRESTRIAL ALIEN DIPLOMAT CHARACTER")
    print("=" * 70)
    print(f"Prompt:\n{PROMPT}\n")

    task_id = create_task(PROMPT)
    print(f"Task ID created: {task_id}")

    result = poll_task(task_id)

    output_obj = result.get("output", {})
    model_url = output_obj.get("pbr_model") or output_obj.get("model")
    if not model_url and "result" in result:
        res_obj = result["result"]
        model_url = res_obj.get("pbr_model", {}).get("url") or res_obj.get("model", {}).get("url")

    if not model_url:
        raise Exception(f"No model URL found in task result: {result}")

    out_path = os.path.join(MODELS_DIR, "alien_diplomat_character.glb")
    download_model(model_url, out_path)
    print(f"\n[SUCCESS] Saved Alien Character model: {out_path}")

if __name__ == "__main__":
    main()
