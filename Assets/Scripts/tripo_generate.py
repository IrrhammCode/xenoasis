"""
XENOASIS — Tripo 3D AI Model Generator
Generates production-quality 3D GLB models using Tripo API V2.
Uses image-to-model for the UFO exterior and text-to-model for the cockpit interior.
"""
import os
import sys
import json
import time
import base64
import urllib.request
import urllib.error

# Load API key
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


def upload_image(image_path: str) -> str:
    """Upload an image file to Tripo and get a file token."""
    print(f"  Uploading image: {image_path}")
    
    boundary = "----WebKitFormBoundary7MA4YWxkTrZu0gW"
    
    with open(image_path, "rb") as f:
        image_data = f.read()
    
    filename = os.path.basename(image_path)
    
    body = (
        f"--{boundary}\r\n"
        f'Content-Disposition: form-data; name="file"; filename="{filename}"\r\n'
        f"Content-Type: image/jpeg\r\n\r\n"
    ).encode("utf-8") + image_data + f"\r\n--{boundary}--\r\n".encode("utf-8")
    
    req = urllib.request.Request(
        f"{BASE_URL}/upload",
        data=body,
        headers={
            "Authorization": f"Bearer {API_KEY}",
            "Content-Type": f"multipart/form-data; boundary={boundary}"
        },
        method="POST"
    )
    
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    
    if result.get("code") != 0:
        raise Exception(f"Upload failed: {result}")
    
    token = result["data"]["image_token"]
    print(f"  Image token: {token}")
    return token


def create_task(task_type: str, params: dict) -> str:
    """Create a Tripo generation task."""
    payload = {"type": task_type, **params}
    data = json.dumps(payload).encode("utf-8")
    
    req = urllib.request.Request(
        f"{BASE_URL}/task",
        data=data,
        headers=HEADERS,
        method="POST"
    )
    
    with urllib.request.urlopen(req) as resp:
        result = json.loads(resp.read().decode("utf-8"))
    
    if result.get("code") != 0:
        raise Exception(f"Task creation failed: {result}")
    
    task_id = result["data"]["task_id"]
    print(f"  Task created: {task_id}")
    return task_id


def poll_task(task_id: str, timeout: int = 600) -> dict:
    """Poll a task until completion."""
    start = time.time()
    while time.time() - start < timeout:
        req = urllib.request.Request(
            f"{BASE_URL}/task/{task_id}",
            headers=HEADERS,
            method="GET"
        )
        
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
    """Download a model file from URL."""
    print(f"  Downloading to: {output_path}")
    urllib.request.urlretrieve(url, output_path)
    size_mb = os.path.getsize(output_path) / (1024 * 1024)
    print(f"  Downloaded: {size_mb:.1f} MB")


def generate_ufo_exterior():
    """Generate UFO exterior using image-to-model."""
    print("\n" + "="*60)
    print("GENERATING UFO EXTERIOR (Image-to-Model)")
    print("="*60)
    
    # Use the generated reference image
    ref_image = os.path.join(
        os.path.dirname(os.path.abspath(__file__)), "..", "..", 
        ".gemini", "antigravity-cli", "brain", 
        "0a29a8e0-a0ce-4546-a7c8-d32da40355b5"
    )
    
    # Find the UFO reference image
    brain_dir = r"C:\Users\Irham\.gemini\antigravity-cli\brain\0a29a8e0-a0ce-4546-a7c8-d32da40355b5"
    ref_files = [f for f in os.listdir(brain_dir) if f.startswith("ufo_reference")]
    if not ref_files:
        print("  No UFO reference image found, falling back to text-to-model")
        return generate_ufo_exterior_text()
    
    ref_path = os.path.join(brain_dir, ref_files[0])
    print(f"  Using reference image: {ref_path}")
    
    # Upload image
    token = upload_image(ref_path)
    
    # Create image-to-model task
    task_id = create_task("image_to_model", {
        "file": {"type": "jpg", "file_token": token},
        "model_version": "v2.5-20250123"
    })
    
    # Poll for completion
    result = poll_task(task_id)
    
def extract_and_download_model(result: dict, output_path: str):
    """Safely extract model URL from Tripo task result and download it."""
    output_obj = result.get("output", {})
    model_url = output_obj.get("pbr_model") or output_obj.get("model")
    if not model_url and "result" in result:
        res_obj = result["result"]
        if "pbr_model" in res_obj:
            model_url = res_obj["pbr_model"].get("url")
        elif "model" in res_obj:
            model_url = res_obj["model"].get("url")
    if not model_url:
        raise Exception(f"No model URL found in result: {result}")
    download_model(model_url, output_path)
    return output_path


def generate_ufo_exterior():
    """Generate UFO exterior using image-to-model."""
    print("\n" + "="*60)
    print("GENERATING UFO EXTERIOR (Image-to-Model)")
    print("="*60)
    
    brain_dir = r"C:\Users\Irham\.gemini\antigravity-cli\brain\0a29a8e0-a0ce-4546-a7c8-d32da40355b5"
    ref_files = [f for f in os.listdir(brain_dir) if f.startswith("ufo_reference")]
    if not ref_files:
        print("  No UFO reference image found, falling back to text-to-model")
        return generate_ufo_exterior_text()
    
    ref_path = os.path.join(brain_dir, ref_files[0])
    print(f"  Using reference image: {ref_path}")
    
    token = upload_image(ref_path)
    task_id = create_task("image_to_model", {
        "file": {"type": "jpg", "file_token": token},
        "model_version": "v2.5-20250123"
    })
    
    result = poll_task(task_id)
    output_path = os.path.join(MODELS_DIR, "ufo_exterior.glb")
    return extract_and_download_model(result, output_path)


def generate_ufo_exterior_text():
    """Generate UFO exterior using text-to-model as fallback."""
    print("\n  Using text-to-model for UFO exterior...")
    task_id = create_task("text_to_model", {
        "prompt": "A highly detailed alien UFO flying saucer spacecraft. Classic disc shape with raised dome cockpit on top with transparent blue glass. Dark metallic gunmetal hull with panel lines. Underside has glowing cyan-blue plasma ring propulsion. 4 landing struts underneath. Warm golden reactor core glowing from center bottom. Aerodynamic, mysterious sci-fi design. Game-ready 3D model.",
        "model_version": "v2.5-20250123"
    })
    result = poll_task(task_id)
    output_path = os.path.join(MODELS_DIR, "ufo_exterior.glb")
    return extract_and_download_model(result, output_path)


def generate_ufo_cockpit():
    """Generate UFO cockpit interior using image-to-model."""
    print("\n" + "="*60)
    print("GENERATING UFO COCKPIT INTERIOR (Image-to-Model)")
    print("="*60)
    
    brain_dir = r"C:\Users\Irham\.gemini\antigravity-cli\brain\0a29a8e0-a0ce-4546-a7c8-d32da40355b5"
    ref_files = [f for f in os.listdir(brain_dir) if f.startswith("ufo_cockpit_reference")]
    if not ref_files:
        print("  No cockpit reference image found, using text-to-model")
        return generate_ufo_cockpit_text()
    
    ref_path = os.path.join(brain_dir, ref_files[0])
    print(f"  Using reference image: {ref_path}")
    
    token = upload_image(ref_path)
    task_id = create_task("image_to_model", {
        "file": {"type": "jpg", "file_token": token},
        "model_version": "v2.5-20250123"
    })
    
    result = poll_task(task_id)
    output_path = os.path.join(MODELS_DIR, "ufo_cockpit_interior.glb")
    return extract_and_download_model(result, output_path)


def generate_ufo_cockpit_text():
    """Generate UFO cockpit interior using text-to-model."""
    print("\n  Using text-to-model for cockpit...")
    task_id = create_task("text_to_model", {
        "prompt": "Interior of alien UFO cockpit command bridge. Curved wrap-around holographic instrument console with glowing screens. Panoramic glass canopy dome. Structural titanium arch ribs. Central pilot command chair with armrests. Three angled display screens on dashboard. Dark metallic walls with blue accent lighting. Sci-fi interior design. Game-ready 3D model.",
        "model_version": "v2.5-20250123"
    })
    result = poll_task(task_id)
    output_path = os.path.join(MODELS_DIR, "ufo_cockpit_interior.glb")
    return extract_and_download_model(result, output_path)


if __name__ == "__main__":
    print("XENOASIS — Tripo 3D AI Model Generator")
    print("="*60)
    print(f"API Key: {API_KEY[:8]}...{API_KEY[-4:]}")
    print(f"Output dir: {MODELS_DIR}")
    
    mode = sys.argv[1] if len(sys.argv) > 1 else "exterior"
    
    if mode == "exterior":
        path = generate_ufo_exterior()
        print(f"\n✅ UFO Exterior model saved to: {path}")
    elif mode == "cockpit":
        path = generate_ufo_cockpit()
        print(f"\n✅ UFO Cockpit model saved to: {path}")
    elif mode == "all":
        ext_path = generate_ufo_exterior()
        cock_path = generate_ufo_cockpit()
        print(f"\n✅ UFO Exterior: {ext_path}")
        print(f"✅ UFO Cockpit: {cock_path}")
    else:
        print(f"Usage: python {sys.argv[0]} [exterior|cockpit|all]")
