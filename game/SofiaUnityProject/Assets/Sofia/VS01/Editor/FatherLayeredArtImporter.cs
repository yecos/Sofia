using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sofia.VS01.Editor
{
    /// <summary>
    /// Imports the separated Father cutouts as crisp, individually pivoted Unity sprites.
    /// The hand-authored atlas and current animated prefab are left untouched.
    /// </summary>
    public static class FatherLayeredArtImporter
    {
        private const string PartsFolder = "Assets/Sofia/VS01/Art/Layered/Father/Parts";
        private const float PixelsPerUnit = 520f;

        [MenuItem("SOFIA/VS01/Animation/Configure layered Father cutouts")]
        public static void ConfigureAll()
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { PartsFolder });
            var configured = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!(AssetImporter.GetAtPath(path) is TextureImporter importer))
                    continue;

                var pivot = GetPivot(Path.GetFileNameWithoutExtension(path));
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = pivot;
                importer.SetTextureSettings(settings);
                importer.spritePixelsPerUnit = PixelsPerUnit;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.SaveAndReimport();
                configured++;
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log($"[SOFIA] Configured {configured} Father cutouts as single sprites at {PixelsPerUnit} PPU.");
        }

        private static Vector2 GetPivot(string filename)
        {
            // Unity uses normalized bottom-left coordinates, matching the art manifest.
            var name = filename.Replace("SPR_Father_", "");
            switch (name)
            {
                case "Head_Profile": return new Vector2(0.74f, 0.12f);
                case "Hair_Back": return new Vector2(0.82f, 0.12f);
                case "Hair_Front": return new Vector2(0.78f, 0.16f);
                case "Scarf_Collar": return new Vector2(0.50f, 0.20f);
                case "Torso_Robe": return new Vector2(0.50f, 0.12f);
                case "Pelvis_Belt_Robe": return new Vector2(0.52f, 0.86f);
                case "Arm_Near_Upper":
                case "Arm_Far_Upper": return new Vector2(0.82f, 0.84f);
                case "Arm_Near_Forearm_Hand":
                case "Arm_Far_Forearm_Hand": return new Vector2(0.86f, 0.86f);
                case "Leg_Near_Pants":
                case "Leg_Far_Pants": return new Vector2(0.76f, 0.90f);
                case "Leg_Near_Boot":
                case "Leg_Far_Boot": return new Vector2(0.48f, 0.84f);
                case "Cape_Back_Upper":
                case "Cape_Mid_Upper":
                case "Cape_Back_Lower":
                case "Cape_Mid_Lower":
                case "Cape_Front_Upper":
                case "Cape_Front_Lower": return new Vector2(0.82f, 0.88f);
                default: return new Vector2(0.5f, 0.5f);
            }
        }
    }
}
