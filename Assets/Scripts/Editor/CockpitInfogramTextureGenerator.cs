using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// XENOASIS — CockpitInfogramTextureGenerator.cs
/// Generates crisp sci-fi holographic Earth scanning and orbital telemetry textures
/// for the UFO cockpit's large curved panoramic displays.
/// </summary>
public static class CockpitInfogramTextureGenerator
{
    public static void GenerateInfogramTextures()
    {
        string dir = "Assets/Textures/Cockpit";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string leftPath = Path.Combine(dir, "Screen_Left_EarthScan.png");
        string rightPath = Path.Combine(dir, "Screen_Right_EarthOrbit.png");

        GenerateLeftEarthScanTexture(leftPath);
        GenerateRightEarthOrbitTexture(rightPath);

        AssetDatabase.Refresh();

        CreateInfogramMaterial(leftPath, "Assets/Materials/Screen_Left_EarthScan.mat", new Color(0f, 0.95f, 1f));
        CreateInfogramMaterial(rightPath, "Assets/Materials/Screen_Right_EarthOrbit.mat", new Color(0.1f, 1f, 0.7f));
    }

    private static void GenerateLeftEarthScanTexture(string path)
    {
        int w = 2048;
        int h = 1024;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color bg = new Color(0.015f, 0.035f, 0.065f, 1.0f);
        Color gridFine = new Color(0.03f, 0.12f, 0.20f, 0.5f);
        Color gridMajor = new Color(0.05f, 0.28f, 0.45f, 0.8f);
        Color cyanBright = new Color(0f, 0.92f, 1f, 1f);
        Color cyanMuted = new Color(0f, 0.65f, 0.85f, 1f);
        Color emerald = new Color(0.05f, 0.95f, 0.55f, 1f);
        Color gold = new Color(1.0f, 0.82f, 0.22f, 1f);
        Color deepOcean = new Color(0.02f, 0.10f, 0.28f, 1.0f);

        // Fill background with subtle tech grid and telemetry borders
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool isMajor = (x % 128 == 0) || (y % 128 == 0);
                bool isFine = (x % 32 == 0) || (y % 32 == 0);
                Color c = isMajor ? gridMajor : (isFine ? gridFine : bg);

                // Header banner
                if (y > h - 70) c = new Color(0.02f, 0.08f, 0.15f, 1f);
                if (y == h - 70 || y == h - 69) c = cyanBright;

                // Footer banner
                if (y < 60) c = new Color(0.02f, 0.07f, 0.13f, 1f);
                if (y == 60 || y == 61) c = cyanBright;

                tex.SetPixel(x, y, c);
            }
        }

        // Draw Left: High-Definition Circular Planetary Radar (cx = 580, cy = 500, radius = 370)
        int cx = 580;
        int cy = 500;
        int r = 370;

        for (int y = cy - r; y <= cy + r; y++)
        {
            for (int x = cx - r; x <= cx + r; x++)
            {
                if (x < 0 || x >= w || y < 0 || y >= h) continue;
                float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (d <= r)
                {
                    float normD = d / r;
                    // Realistic spherical depth gradient for Earth globe
                    float sphereZ = Mathf.Sqrt(Mathf.Max(0f, 1f - normD * normD));
                    Color globeCol = Color.Lerp(deepOcean * 0.4f, deepOcean * (0.8f + sphereZ * 0.5f), sphereZ);

                    // Multi-octave continental landmass simulation
                    float nx = (x - cx) * 0.0075f;
                    float ny = (y - cy) * 0.0075f;
                    float n1 = Mathf.PerlinNoise(nx + 10.5f, ny + 10.5f);
                    float n2 = Mathf.PerlinNoise(nx * 2.2f + 5.2f, ny * 2.2f + 5.2f) * 0.5f;
                    float n3 = Mathf.PerlinNoise(nx * 4.5f + 1.1f, ny * 4.5f + 1.1f) * 0.25f;
                    float continent = n1 + n2 + n3;

                    if (continent > 0.88f)
                    {
                        // Lush green lowland / forest
                        globeCol = Color.Lerp(new Color(0.06f, 0.48f, 0.24f), new Color(0.12f, 0.65f, 0.35f), (continent - 0.88f) * 3f);
                    }
                    if (continent > 1.12f)
                    {
                        // Highland plateau / mountain
                        globeCol = Color.Lerp(globeCol, new Color(0.72f, 0.60f, 0.32f), (continent - 1.12f) * 4f);
                    }
                    if (continent > 1.30f || Mathf.Abs(y - cy) > r * 0.82f)
                    {
                        // Alpine glaciers and polar icecaps
                        globeCol = Color.Lerp(globeCol, new Color(0.85f, 0.95f, 1.0f), 0.85f);
                    }

                    // Atmospheric limb glow at the horizon edge
                    if (normD > 0.90f)
                    {
                        float limb = (normD - 0.90f) / 0.10f;
                        globeCol = Color.Lerp(globeCol, cyanBright, limb * 0.75f);
                    }

                    // Concentric radar scan rings
                    if (Mathf.Abs(d - r) < 3.5f) globeCol = cyanBright;
                    else if (Mathf.Abs(d - r * 0.75f) < 2.0f) globeCol = cyanMuted * 0.8f;
                    else if (Mathf.Abs(d - r * 0.50f) < 2.0f) globeCol = cyanMuted * 0.6f;
                    else if (Mathf.Abs(d - r * 0.25f) < 2.0f) globeCol = cyanMuted * 0.4f;

                    // Targeting Crosshairs
                    if ((Mathf.Abs(x - cx) < 2f || Mathf.Abs(y - cy) < 2f) && d < r)
                        globeCol = Color.Lerp(globeCol, cyanBright, 0.7f);

                    tex.SetPixel(x, y, globeCol);
                }
            }
        }

        // Draw Landing Target Reticle on Earth
        int tx = cx + 80;
        int ty = cy + 110;
        DrawThickRing(tex, tx, ty, 32, 3, gold);
        DrawThickRing(tex, tx, ty, 16, 2, gold);
        DrawCrosshairTicks(tex, tx, ty, 44, 12, gold);

        // Draw Right Side: Atmospheric Stratification Gauges & Biosphere Vital Signs
        int rxStart = 1120;
        int barW = 820;

        Color[] layerColors = {
            new Color(0.12f, 0.80f, 1.00f), // Troposfer (Cyan-Blue)
            new Color(0.00f, 0.95f, 0.85f), // Stratosfer (Teal)
            new Color(1.00f, 0.55f, 0.10f), // Mesosfer (Plasma Orange)
            new Color(0.85f, 0.30f, 1.00f), // Termosfer (Violet)
            new Color(0.35f, 0.60f, 1.00f)  // Eksosfer (Deep Sky)
        };

        for (int i = 0; i < 5; i++)
        {
            int by = 740 - i * 120;
            float fillPct = 0.58f + i * 0.08f;
            DrawSegmentedGauge(tex, rxStart, by, barW, 55, fillPct, layerColors[i], cyanBright);
        }

        // Biosphere Spectrum Bars at Bottom Right
        int btmY = 110;
        for (int b = 0; b < 4; b++)
        {
            int bx = rxStart + b * 210;
            Color col = (b == 0) ? cyanBright : (b == 1 ? emerald : (b == 2 ? gold : new Color(0.9f, 0.3f, 0.8f)));
            DrawMiniBarChart(tex, bx, btmY, 180, 80, 0.6f + b * 0.12f, col);
        }

        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateRightEarthOrbitTexture(string path)
    {
        int w = 2048;
        int h = 1024;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color bg = new Color(0.015f, 0.035f, 0.065f, 1.0f);
        Color gridFine = new Color(0.03f, 0.14f, 0.18f, 0.5f);
        Color gridMajor = new Color(0.04f, 0.32f, 0.36f, 0.8f);
        Color greenBright = new Color(0f, 1f, 0.75f, 1f);
        Color goldBright = new Color(1f, 0.85f, 0.25f, 1f);
        Color cyanBright = new Color(0f, 0.92f, 1f, 1f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool isMajor = (x % 128 == 0) || (y % 128 == 0);
                bool isFine = (x % 32 == 0) || (y % 32 == 0);
                Color c = isMajor ? gridMajor : (isFine ? gridFine : bg);

                if (y > h - 70) c = new Color(0.02f, 0.12f, 0.10f, 1f);
                if (y == h - 70 || y == h - 69) c = greenBright;

                if (y < 60) c = new Color(0.01f, 0.10f, 0.08f, 1f);
                if (y == 60 || y == 61) c = greenBright;

                tex.SetPixel(x, y, c);
            }
        }

        // Draw Left: Sol-3 Solar System Orbital Dynamics
        int cx = 620;
        int cy = 500;
        float a = 460f; // Semi-major axis
        float b = 300f; // Semi-minor axis

        // Concentric inner planetary orbits (Mercury, Venus)
        DrawEllipse(tex, cx, cy, a * 0.38f, b * 0.38f, 2, greenBright * 0.35f);
        DrawEllipse(tex, cx, cy, a * 0.72f, b * 0.72f, 2, greenBright * 0.45f);

        // Earth Orbit Ellipse
        DrawEllipse(tex, cx, cy, a, b, 3, greenBright * 0.95f);

        // Mars Outer Orbit Ellipse
        DrawEllipse(tex, cx, cy, a * 1.35f, b * 1.35f, 2, new Color(0.9f, 0.4f, 0.2f, 0.45f));

        // Central Sol (Sun) Star with Corona Glow
        DrawThickDot(tex, cx - 40, cy, 28, goldBright);
        for (int rad = 29; rad < 85; rad++)
        {
            for (int deg = 0; deg < 360; deg += 3)
            {
                float angle = deg * Mathf.Deg2Rad;
                int gx = cx - 40 + Mathf.RoundToInt(Mathf.Cos(angle) * rad);
                int gy = cy + Mathf.RoundToInt(Mathf.Sin(angle) * rad);
                if (gx >= 0 && gx < w && gy >= 0 && gy < h)
                {
                    float fade = 1f - (float)(rad - 29) / 56f;
                    Color glow = Color.Lerp(Color.clear, goldBright, fade * fade);
                    tex.SetPixel(gx, gy, Color.Lerp(tex.GetPixel(gx, gy), glow, 0.65f));
                }
            }
        }

        // Earth on Orbit (at deg = 50°)
        float earthAngle = 50f * Mathf.Deg2Rad;
        int ex = cx + Mathf.RoundToInt(Mathf.Cos(earthAngle) * a);
        int ey = cy + Mathf.RoundToInt(Mathf.Sin(earthAngle) * b);

        // Moon Orbit Ring
        DrawThickRing(tex, ex, ey, 55, 2, cyanBright * 0.7f);
        DrawThickDot(tex, ex, ey, 18, cyanBright); // Earth
        DrawThickDot(tex, ex + 46, ey + 28, 6, new Color(0.95f, 0.95f, 1f)); // Moon

        // Flight Intercept Hyperbolic Approach Arc from Deep Space towards Earth
        for (float t = 0f; t <= 1f; t += 0.002f)
        {
            Vector3 p0 = new Vector3(cx - 520, cy - 260, 0);
            Vector3 p1 = new Vector3(cx - 100, cy + 320, 0);
            Vector3 p2 = new Vector3(ex, ey, 0);
            Vector3 pt = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
            DrawThickDot(tex, Mathf.RoundToInt(pt.x), Mathf.RoundToInt(pt.y), 3, new Color(1f, 0.9f, 0.2f, 0.85f));
        }

        // Draw Right Side: Dual-Band Radio & Hydrogen Spectral Analysis + Waveform Gauges
        int rxStart = 1200;
        int waveW = 760;

        // Waveform 1: 1420 MHz Hydrogen Line RF Signal
        for (int x = rxStart; x < rxStart + waveW; x++)
        {
            float t = (float)(x - rxStart) / waveW * 28f;
            float s = Mathf.Sin(t) * 70f + Mathf.Sin(t * 2.8f) * 35f + Mathf.Sin(t * 0.45f) * 20f;
            int wy = 720 + Mathf.RoundToInt(s);
            DrawThickDot(tex, x, wy, 3, greenBright);

            // Spectral waterfall audio visualizer bars
            if (x % 12 == 0)
            {
                int barH = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(s) * 1.4f) + 15, 15, 140);
                for (int y = 560; y < 560 + barH; y++)
                {
                    for (int bx = x - 3; bx <= x + 3; bx++)
                    {
                        if (bx >= 0 && bx < w && y >= 0 && y < h)
                            tex.SetPixel(bx, y, Color.Lerp(greenBright, cyanBright, (float)(y - 560) / barH));
                    }
                }
            }
        }

        // 3 Telemetry Data Sub-Panels at Bottom Right
        for (int row = 0; row < 3; row++)
        {
            int by = 380 - row * 110;
            float fillPct = 0.72f + row * 0.10f;
            Color barCol = (row == 0) ? cyanBright : (row == 1 ? greenBright : goldBright);
            DrawSegmentedGauge(tex, rxStart, by, waveW, 48, fillPct, barCol, greenBright);
        }

        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void DrawEllipse(Texture2D tex, int cx, int cy, float a, float b, int thick, Color col)
    {
        for (int deg = 0; deg < 360; deg++)
        {
            float rad = deg * Mathf.Deg2Rad;
            int px = cx + Mathf.RoundToInt(Mathf.Cos(rad) * a);
            int py = cy + Mathf.RoundToInt(Mathf.Sin(rad) * b);
            DrawThickDot(tex, px, py, thick, col);
        }
    }

    private static void DrawThickRing(Texture2D tex, int cx, int cy, int radius, int thick, Color col)
    {
        for (int y = cy - radius - thick; y <= cy + radius + thick; y++)
        {
            for (int x = cx - radius - thick; x <= cx + radius + thick; x++)
            {
                if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (Mathf.Abs(d - radius) <= thick * 0.5f)
                    tex.SetPixel(x, y, col);
            }
        }
    }

    private static void DrawCrosshairTicks(Texture2D tex, int cx, int cy, int length, int offset, Color col)
    {
        for (int i = offset; i < offset + length; i++)
        {
            DrawThickDot(tex, cx + i, cy, 2, col);
            DrawThickDot(tex, cx - i, cy, 2, col);
            DrawThickDot(tex, cx, cy + i, 2, col);
            DrawThickDot(tex, cx, cy - i, 2, col);
        }
    }

    private static void DrawSegmentedGauge(Texture2D tex, int startX, int startY, int totalW, int totalH, float fillPct, Color fillColor, Color borderCol)
    {
        int segCount = 20;
        int segGap = 4;
        int segW = (totalW - (segCount - 1) * segGap) / segCount;
        int filledSegs = Mathf.RoundToInt(fillPct * segCount);

        for (int s = 0; s < segCount; s++)
        {
            int sx = startX + s * (segW + segGap);
            bool isFilled = s < filledSegs;
            Color c = isFilled ? fillColor : new Color(0.02f, 0.09f, 0.14f, 0.8f);

            for (int y = startY; y < startY + totalH; y++)
            {
                for (int x = sx; x < sx + segW; x++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                    bool isBorder = (x == sx || x == sx + segW - 1 || y == startY || y == startY + totalH - 1);
                    tex.SetPixel(x, y, isBorder ? borderCol * 0.85f : c);
                }
            }
        }
    }

    private static void DrawMiniBarChart(Texture2D tex, int startX, int startY, int w, int h, float val, Color col)
    {
        for (int y = startY; y < startY + h; y++)
        {
            for (int x = startX; x < startX + w; x++)
            {
                if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                bool isBorder = (x == startX || x == startX + w - 1 || y == startY || y == startY + h - 1);
                float prog = (float)(y - startY) / h;
                Color c = prog <= val ? col : new Color(0.02f, 0.08f, 0.12f, 0.8f);
                tex.SetPixel(x, y, isBorder ? col : c);
            }
        }
    }

    private static void DrawThickDot(Texture2D tex, int cx, int cy, int radius, Color color)
    {
        for (int y = cy - radius; y <= cy + radius; y++)
        {
            for (int x = cx - radius; x <= cx + radius; x++)
            {
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                {
                    if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= radius * radius)
                    {
                        tex.SetPixel(x, y, color);
                    }
                }
            }
        }
    }

    private static void CreateInfogramMaterial(string texPath, string matPath, Color emissiveTint)
    {
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        // Universal Render Pipeline/Unlit ensures screens are 100% self-luminous, bright, and never dimmed or shadowed
        Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(unlitShader);
            AssetDatabase.CreateAsset(mat, matPath);
        }
        else
        {
            mat.shader = unlitShader;
        }

        mat.mainTexture = tex;
        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
        // HDR Base Color (1.4f intensity) makes the infogram pop with luminous neon glow
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white * 1.4f);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", Color.white * 1.4f);
        if (mat.HasProperty("_Cull")) mat.SetFloat("_Cull", 0f); // Double-sided
        mat.doubleSidedGI = true;

        EditorUtility.SetDirty(mat);
    }
}
