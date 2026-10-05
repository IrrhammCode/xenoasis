using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// XENOASIS — MuseumTextureGenerator.cs
/// Generates authentic, high-detail textures and PBR materials for the Museum of Humanity:
/// 1. Voyager Golden Record (Gold phonograph grooves, pulsar map, hydrogen transition)
/// 2. Rosetta Stone (Ancient hieroglyphic engravings on granodiorite slab)
/// 3. Human Masterpiece Artworks (Mona Lisa, Starry Night, Prehistoric Cave Art)
/// 4. Silicon Wafer (Microprocessor die matrix with iridescent diffraction)
/// 5. Apollo 11 Lunar Bootprint (Moon regolith tread imprint & Pioneer Plaque)
/// </summary>
public static class MuseumTextureGenerator
{
    private const string TextureDir = "Assets/Textures/Museum";
    private const string MaterialDir = "Assets/Materials/Museum";

    [MenuItem("XENOASIS/Generate Museum Textures & Materials")]
    public static void GenerateAllMuseumTextures()
    {
        Debug.Log("[XENOASIS] Generating Museum of Humanity textures and PBR materials...");

        if (!Directory.Exists(TextureDir)) Directory.CreateDirectory(TextureDir);
        if (!Directory.Exists(MaterialDir)) Directory.CreateDirectory(MaterialDir);

        GenerateGoldenRecordTexture();
        GenerateRosettaTexture();
        GenerateArtworksTextures();
        GenerateSiliconWaferTexture();
        GenerateLunarBootprintTexture();
        GenerateMesopotamianWheelTexture();
        GenerateHandaxeTexture();

        AssetDatabase.Refresh();

        CreateMuseumMaterials();
        Debug.Log("[XENOASIS] Successfully generated all Museum of Humanity textures and materials!");
    }

    private static void GenerateGoldenRecordTexture()
    {
        int size = 1024;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color goldBase = new Color(0.88f, 0.72f, 0.22f);
        Color goldGroove = new Color(0.68f, 0.52f, 0.12f);
        Color goldHighlight = new Color(1.0f, 0.90f, 0.50f);
        Color centerLabelCol = new Color(0.08f, 0.08f, 0.08f);

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.48f;
        float labelRadius = size * 0.18f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                if (d > radius)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                if (d < labelRadius)
                {
                    // Center spindle hole & label diagram
                    if (d < size * 0.03f)
                    {
                        tex.SetPixel(x, y, new Color(0.02f, 0.02f, 0.02f, 1f));
                    }
                    else
                    {
                        // Pulsar map center diagram
                        float angle = Mathf.Atan2(y - center.y, x - center.x) * Mathf.Rad2Deg;
                        bool isPulsarRay = false;
                        for (int p = 0; p < 14; p++)
                        {
                            float pAngle = p * 25.7f;
                            if (Mathf.Abs(Mathf.DeltaAngle(angle, pAngle)) < 1.2f && d > size * 0.05f)
                            {
                                isPulsarRay = true;
                                break;
                            }
                        }
                        tex.SetPixel(x, y, isPulsarRay ? goldHighlight : centerLabelCol);
                    }
                }
                else
                {
                    // Concentric phonograph vinyl sound grooves
                    float groove = Mathf.Sin(d * 1.8f);
                    Color c = Color.Lerp(goldGroove, goldBase, (groove + 1f) * 0.5f);

                    // Anisotropic radial shine
                    float angle = Mathf.Atan2(y - center.y, x - center.x);
                    float shine = Mathf.Pow(Mathf.Abs(Mathf.Sin(angle * 2f)), 4f);
                    c = Color.Lerp(c, goldHighlight, shine * 0.45f);

                    tex.SetPixel(x, y, c);
                }
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_Voyager_GoldenRecord.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateRosettaTexture()
    {
        int w = 512, h = 1024;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color stoneDark = new Color(0.12f, 0.13f, 0.15f);
        Color stoneSpeckle = new Color(0.24f, 0.25f, 0.28f);
        Color glyphGold = new Color(0.92f, 0.82f, 0.45f);
        Color cyanTranslate = new Color(0f, 0.9f, 1f, 0.8f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // Granodiorite stone texture
                float noise = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                Color c = Color.Lerp(stoneDark, stoneSpeckle, noise);

                // Three script registers (Hieroglyphs top, Demotic middle, Greek bottom)
                bool isRegisterDivider = (Mathf.Abs(y - h * 0.66f) < 2) || (Mathf.Abs(y - h * 0.33f) < 2);
                if (isRegisterDivider) c = new Color(0.35f, 0.38f, 0.42f);

                // Engraved script lines
                int lineInterval = (y > h * 0.66f) ? 28 : (y > h * 0.33f ? 18 : 22);
                int lineY = y % lineInterval;
                if (lineY == 0 || lineY == 1)
                {
                    float glyphNoise = Mathf.PerlinNoise(x * 0.25f, y);
                    if (glyphNoise > 0.40f)
                    {
                        c = (y > h * 0.66f) ? glyphGold : Color.Lerp(glyphGold, cyanTranslate, 0.3f);
                    }
                }

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_RosettaStone.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateArtworksTextures()
    {
        int w = 512, h = 512;

        // 1. Mona Lisa Stylized Canvas
        Texture2D monaTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color monaBg = new Color(0.18f, 0.22f, 0.16f);
        Color skinTone = new Color(0.85f, 0.70f, 0.55f);
        Color hairTone = new Color(0.15f, 0.12f, 0.08f);
        Color veilTone = new Color(0.28f, 0.24f, 0.18f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = (x - w * 0.5f) / (w * 0.5f);
                float dy = (y - h * 0.45f) / (h * 0.5f);
                float distHead = (dx * dx * 1.5f + (dy - 0.25f) * (dy - 0.25f));

                Color c = monaBg;
                if (distHead < 0.22f) c = skinTone;
                else if (distHead < 0.38f && dy > 0.05f) c = hairTone;
                else if (dy < 0.05f && Mathf.Abs(dx) < 0.65f) c = veilTone;

                // Canvas craquelure & chiaroscuro shading
                float brush = Mathf.PerlinNoise(x * 0.04f, y * 0.04f);
                c *= (0.85f + brush * 0.3f);
                monaTex.SetPixel(x, y, c);
            }
        }
        monaTex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_Artwork_MonaLisa.png"), monaTex.EncodeToPNG());
        Object.DestroyImmediate(monaTex);

        // 2. Starry Night Swirls Canvas
        Texture2D starryTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color nightDeep = new Color(0.04f, 0.10f, 0.32f);
        Color swirlCyan = new Color(0.15f, 0.65f, 0.85f);
        Color starYellow = new Color(1.0f, 0.88f, 0.25f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float swirl = Mathf.Sin(x * 0.025f + Mathf.Cos(y * 0.03f) * 4f) * Mathf.Cos(y * 0.02f);
                Color c = Color.Lerp(nightDeep, swirlCyan, (swirl + 1f) * 0.5f);

                // Add swirling yellow stars
                Vector2 s1 = new Vector2(w * 0.78f, h * 0.75f);
                Vector2 s2 = new Vector2(w * 0.35f, h * 0.62f);
                float d1 = Vector2.Distance(new Vector2(x, y), s1);
                float d2 = Vector2.Distance(new Vector2(x, y), s2);

                if (d1 < 45f) c = Color.Lerp(c, starYellow, (1f - d1 / 45f));
                if (d2 < 35f) c = Color.Lerp(c, starYellow, (1f - d2 / 35f));

                starryTex.SetPixel(x, y, c);
            }
        }
        starryTex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_Artwork_StarryNight.png"), starryTex.EncodeToPNG());
        Object.DestroyImmediate(starryTex);

        // 3. Prehistoric Cave Art (Lascaux / Maros)
        Texture2D caveTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color rockOchre = new Color(0.55f, 0.32f, 0.18f);
        Color rockDark = new Color(0.35f, 0.20f, 0.12f);
        Color charcoalBlack = new Color(0.08f, 0.08f, 0.08f);
        Color redPigment = new Color(0.72f, 0.15f, 0.12f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float rockGrain = Mathf.PerlinNoise(x * 0.06f, y * 0.06f);
                Color c = Color.Lerp(rockDark, rockOchre, rockGrain);

                // Hand stencil & bull silhouette
                float dHand = Vector2.Distance(new Vector2(x, y), new Vector2(w * 0.5f, h * 0.5f));
                if (dHand < 120f && dHand > 70f)
                {
                    float angle = Mathf.Atan2(y - h * 0.5f, x - w * 0.5f);
                    float fingers = Mathf.Cos(angle * 5f);
                    if (fingers > 0.2f) c = Color.Lerp(c, redPigment, 0.65f);
                }

                caveTex.SetPixel(x, y, c);
            }
        }
        caveTex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_Artwork_Lascaux.png"), caveTex.EncodeToPNG());
        Object.DestroyImmediate(caveTex);
    }

    private static void GenerateSiliconWaferTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color siliconGrey = new Color(0.15f, 0.18f, 0.22f);
        Color metallicTrim = new Color(0.75f, 0.80f, 0.85f);

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.46f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                if (d > radius)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                // Grid matrix of square CPU processor dies
                int dieSize = 42;
                bool isDieEdge = (x % dieSize == 0) || (y % dieSize == 0);

                // Iridescent diffraction rainbow shimmer across silicon surface
                float hue = Mathf.Repeat((x * 0.003f + y * 0.004f + d * 0.005f), 1f);
                Color iridescence = Color.HSVToRGB(hue, 0.65f, 0.90f);

                Color c = isDieEdge ? metallicTrim : Color.Lerp(siliconGrey, iridescence, 0.45f);

                // Rim bevel
                if (Mathf.Abs(d - radius) < 3f) c = Color.white;

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_SiliconWafer.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateLunarBootprintTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color regolithLight = new Color(0.48f, 0.50f, 0.52f);
        Color regolithShadow = new Color(0.18f, 0.19f, 0.21f);
        Color treadCyanAccent = new Color(0f, 0.85f, 1f, 0.8f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Fine lunar soil grain
                float noise = Mathf.PerlinNoise(x * 0.12f, y * 0.12f);
                Color c = Color.Lerp(regolithShadow, regolithLight, noise);

                // Astronaut boot horizontal treads (Armstrong Apollo 11 sole)
                float normX = (x - size * 0.5f) / (size * 0.32f);
                float normY = (y - size * 0.5f) / (size * 0.44f);
                float bootDist = normX * normX + normY * normY;

                if (bootDist < 1.0f)
                {
                    // Ribbed sole treads
                    int treadLine = y % 22;
                    if (treadLine < 10)
                    {
                        c = regolithShadow * 0.75f;
                    }
                    else
                    {
                        c = Color.Lerp(regolithLight * 1.15f, treadCyanAccent, 0.15f);
                    }
                }

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_Apollo11_Bootprint.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateMesopotamianWheelTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color woodLight = new Color(0.55f, 0.35f, 0.18f);
        Color woodDark = new Color(0.32f, 0.18f, 0.08f);
        Color bronzeRim = new Color(0.65f, 0.45f, 0.22f);
        Color bronzeRivet = new Color(0.85f, 0.70f, 0.35f);
        Color ironAxle = new Color(0.12f, 0.12f, 0.14f);

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float rMax = size * 0.48f;
        float rRimInner = size * 0.40f;
        float rHubOuter = size * 0.15f;
        float rAxleHole = size * 0.05f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                if (d > rMax)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                if (d < rAxleHole)
                {
                    tex.SetPixel(x, y, ironAxle);
                    continue;
                }

                if (d < rHubOuter)
                {
                    // Center bronze hub ring with wood texture
                    float hubNoise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    tex.SetPixel(x, y, Color.Lerp(bronzeRim, bronzeRivet, hubNoise));
                    continue;
                }

                if (d > rRimInner)
                {
                    // Outer bronze tire / rim with rivets
                    float angle = Mathf.Atan2(y - center.y, x - center.x);
                    float rivetAngle = Mathf.Repeat(angle * 12f / Mathf.PI, 1.0f);
                    Color rimCol = bronzeRim;
                    if (rivetAngle < 0.15f && d > (rRimInner + rMax) * 0.5f - 8f && d < (rRimInner + rMax) * 0.5f + 8f)
                    {
                        rimCol = bronzeRivet;
                    }
                    tex.SetPixel(x, y, rimCol);
                    continue;
                }

                // 3-part ancient wooden plank construction
                float woodNoise = Mathf.PerlinNoise(x * 0.035f, y * 0.12f);
                Color c = Color.Lerp(woodDark, woodLight, woodNoise);

                // Plank separation lines
                float normX = Mathf.Abs((x - center.x) / (size * 0.32f));
                if (Mathf.Abs(normX - 0.5f) < 0.02f)
                {
                    c = woodDark * 0.6f;
                }

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_MesopotamianWheel.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void GenerateHandaxeTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color flintDark = new Color(0.18f, 0.20f, 0.22f);
        Color flintMid = new Color(0.32f, 0.34f, 0.36f);
        Color flintChipped = new Color(0.52f, 0.55f, 0.58f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float stoneNoise = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                float chipFacets = Mathf.PerlinNoise(x * 0.25f, y * 0.25f);
                Color c = Color.Lerp(flintDark, flintMid, stoneNoise);
                if (chipFacets > 0.65f)
                {
                    c = Color.Lerp(c, flintChipped, (chipFacets - 0.65f) / 0.35f);
                }
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(Path.Combine(TextureDir, "Tex_PaleolithicHandaxe.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    private static void CreateMuseumMaterials()
    {
        Shader pbrShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

        CreateMaterialWithTexture("Tex_Voyager_GoldenRecord.png", "Mat_Voyager_GoldenRecord.mat", pbrShader, 0.95f, 0.88f, new Color(1f, 0.85f, 0.3f), 0.8f);
        CreateMaterialWithTexture("Tex_RosettaStone.png", "Mat_RosettaStone.mat", pbrShader, 0.2f, 0.4f, new Color(0f, 0.9f, 1f), 0.5f);
        CreateMaterialWithTexture("Tex_Artwork_MonaLisa.png", "Mat_Artwork_MonaLisa.mat", pbrShader, 0.05f, 0.3f, Color.white, 0.2f);
        CreateMaterialWithTexture("Tex_Artwork_StarryNight.png", "Mat_Artwork_StarryNight.mat", pbrShader, 0.1f, 0.4f, new Color(0.2f, 0.7f, 1f), 0.6f);
        CreateMaterialWithTexture("Tex_Artwork_Lascaux.png", "Mat_Artwork_Lascaux.mat", pbrShader, 0.05f, 0.2f, new Color(0.8f, 0.3f, 0.1f), 0.3f);
        CreateMaterialWithTexture("Tex_SiliconWafer.png", "Mat_SiliconWafer.mat", pbrShader, 0.9f, 0.95f, new Color(0f, 1f, 0.8f), 0.9f);
        CreateMaterialWithTexture("Tex_Apollo11_Bootprint.png", "Mat_Apollo11_Bootprint.mat", pbrShader, 0.1f, 0.3f, new Color(0f, 0.85f, 1f), 0.4f);
        CreateMaterialWithTexture("Tex_MesopotamianWheel.png", "Mat_MesopotamianWheel.mat", pbrShader, 0.35f, 0.3f, Color.black, 0f);
        CreateMaterialWithTexture("Tex_PaleolithicHandaxe.png", "Mat_PaleolithicHandaxe.mat", pbrShader, 0.15f, 0.35f, Color.black, 0f);
    }

    private static void CreateMaterialWithTexture(string texName, string matName, Shader shader, float metallic, float smoothness, Color emTint, float emIntensity)
    {
        string texPath = Path.Combine(TextureDir, texName);
        string matPath = Path.Combine(MaterialDir, matName);

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, matPath);
        }

        mat.mainTexture = tex;
        mat.SetColor("_BaseColor", Color.white);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Cull", 0f);

        if (emIntensity > 0f)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetTexture("_EmissionMap", tex);
            mat.SetColor("_EmissionColor", emTint * emIntensity);
        }

        EditorUtility.SetDirty(mat);
    }
}
