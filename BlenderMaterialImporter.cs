using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class BlenderMaterialImporter : MonoBehaviour
{   
    class BlenderMaterial_Importer: AssetPostprocessor
    {
        [MenuItem("Blender/Import Materials")]
        private static void ImportMaterials()
        {
            string path = "D:/Venus_test/export_all_materials.json"; // Adjust the path to your exported materials file

            string json = File.ReadAllText(path);

            // Print the JSON string to the console for debugging
            Debug.Log("JSON String: " + json);

            string correctedJson = "{\"materialsData\":" + json + "}";
            MaterialList materialList = JsonUtility.FromJson<MaterialList>(correctedJson);

            foreach (MaterialData materialData in materialList.materialsData)
            {
                // Create a new material instance
                Material material = new Material(Shader.Find("Standard")); // Adjust the shader

                // Set material properties
                material.name = materialData.name;

                if (materialData.diffuse_color != null && materialData.diffuse_color.Length >= 3)
                {
                    material.color = new Color(materialData.diffuse_color[0], materialData.diffuse_color[1], materialData.diffuse_color[2]);
                }
                else
                {
                    Debug.LogError("Diffuse color data is missing or incomplete for material: " + materialData.name);
                    continue; // Skip this material and proceed to the next one
                }

                material.SetColor("_SpecColor", new Color(materialData.specular_color[0], materialData.specular_color[1], materialData.specular_color[2]));
                material.SetFloat("_Shininess", materialData.specular_intensity);

                // Check if "emit" property exists
                if (materialData.emit != null && materialData.emit.Length >= 3)
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", new Color(materialData.emit[0], materialData.emit[1], materialData.emit[2]));
                }

                // Handle transparency property
                if (materialData.use_transparency)
                {
                    material.SetFloat("_Mode", 3); // Transparent mode
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000; // Adjust render queue as needed
                }

                // Save the material instance as an asset
                string materialPath = "Assets/Materials/" + material.name + ".mat"; // Adjust the materials directory
                AssetDatabase.CreateAsset(material, materialPath);
            }

                AssetDatabase.Refresh();
        }
        

        [System.Serializable]
        private class MaterialList
        {
            public List<MaterialData> materialsData;
        }

        [System.Serializable]
        private class MaterialData
        {
            public string name;
            public float[] diffuse_color;
            public float[] specular_color;
            public float specular_intensity;

            // Optional properties
            public float[] emit;
            public bool use_transparency;

            // Add other optional properties as needed
        }


        [MenuItem("Blender/Documentation")]
        private static void Documentation()
        {
            Debug.Log("Testing, testing, 1...2...3...!");
        }
    }
}
