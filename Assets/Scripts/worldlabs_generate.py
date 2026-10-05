"""
XENOASIS — WorldLabs Marble 3D World Generator
Generates immersive 3D worlds and environments via WorldLabs Marble API.
"""
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
        if line.startswith("WORLDLABS_API_KEY="):
            API_KEY = line.split("=", 1)[1]
            break

if not API_KEY:
    print("ERROR: WORLDLABS_API_KEY not found in .env")
    sys.exit(1)

BASE_URL = "https://api.worldlabs.ai/marble/v1"
HEADERS = {
    "WLT-Api-Key": API_KEY,
    "Content-Type": "application/json"
}

WORLDLABS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Models", "WorldLabs")
os.makedirs(WORLDLABS_DIR, exist_ok=True)


def check_credits():
    req = urllib.request.Request(f"{BASE_URL}/credits", headers=HEADERS)
    with urllib.request.urlopen(req) as resp:
        res = json.loads(resp.read().decode("utf-8"))
    print(f"Credits remaining: {res.get('remaining_credits')}")
    return res


def generate_world(prompt: str, display_name: str = "Terran Embassy Sanctuary", model: str = "marble-1.1"):
    print("\n" + "="*60)
    print(f"WORLDLABS GENERATION: {display_name}")
    print("="*60)
    print(f"Model: {model}")
    print(f"Prompt: {prompt[:100]}...")

    payload = {
        "display_name": display_name,
        "model": model,
        "world_prompt": {
            "type": "text",
            "text_prompt": prompt
        }
    }
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        f"{BASE_URL}/worlds:generate",
        data=data,
        headers=HEADERS,
        method="POST"
    )

    try:
        with urllib.request.urlopen(req) as resp:
            res = json.loads(resp.read().decode("utf-8"))
        print("\nGeneration request successful!")
        print(json.dumps(res, indent=2))
        return res
    except urllib.error.HTTPError as e:
        err_msg = e.read().decode("utf-8")
        print(f"\nHTTP Error {e.code}: {e.reason}")
        print(f"Response: {err_msg}")
        raise


def get_operation(operation_id: str):
    req = urllib.request.Request(f"{BASE_URL}/operations/{operation_id}", headers=HEADERS)
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read().decode("utf-8"))


def generate_and_download(prompt: str, display_name: str = "Pristine Earth Alpine Sanctuary", model: str = "marble-1.1"):
    res = generate_world(prompt, display_name, model)
    op_id = res["operation_id"]
    print(f"Polling operation {op_id}...")
    while True:
        time.sleep(15)
        op = get_operation(op_id)
        done = op.get("done", False)
        status = op.get("metadata", {}).get("progress", {}).get("status", "IN_PROGRESS")
        print(f"  Status: {status} (done={done})")
        if done:
            if op.get("error"):
                raise Exception(f"WorldLabs generation failed: {op['error']}")
            response_obj = op.get("response", {})
            assets = response_obj.get("assets", {})
            pano_url = assets.get("imagery", {}).get("pano_url")
            if pano_url:
                pano_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Textures", "Skybox", "WorldLabs_TerranEmbassy_Pano.png")
                urllib.request.urlretrieve(pano_url, pano_path)
                print(f"✅ Successfully downloaded new 360 panorama to {pano_path} ({os.path.getsize(pano_path)} bytes)")
            
            collider_url = assets.get("mesh", {}).get("collider_mesh_url")
            if collider_url:
                collider_path = os.path.join(WORLDLABS_DIR, "terran_embassy_sanctuary.glb")
                urllib.request.urlretrieve(collider_url, collider_path)
                print(f"✅ Downloaded 3D collider mesh to {collider_path}")
            return op


if __name__ == "__main__":
    check_credits()
    prompt = (
        "A seamless 360-degree panoramic landscape of Planet Earth: breathtaking natural alpine wilderness with an azure crystal mirror lake in the center valley, majestic snow-capped mountain peaks surrounding the horizon 360 degrees, lush green pine forests lining the water, vibrant atmosphere with glowing sunset lighting and clear clouds. Full expansive 360 open world, eye-level natural view, no close walls or floor obstructions."
    )
    if len(sys.argv) > 1 and sys.argv[1] == "generate":
        generate_and_download(prompt, "Earth Alpine Sanctuary 360", model="marble-1.1")
