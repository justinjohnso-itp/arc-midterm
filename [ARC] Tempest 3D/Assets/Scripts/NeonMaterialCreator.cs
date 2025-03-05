using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Renderer))]
public class NeonMaterialCreator : MonoBehaviour
{
    public Material yellowNeonMaterial;
    private Renderer objectRenderer;
    private string materialsFolder = "Assets/Materials";
    
    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }
    
    private void Start()
    {
        if (yellowNeonMaterial == null)
        {
            CreateNeonMaterial();
        }
        
        ApplyMaterialToSelf();
    }
    
    [ContextMenu("Create Neon Material")]
    void CreateNeonMaterial()
    {
        // Create glowing yellow neon material
        yellowNeonMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        yellowNeonMaterial.name = "YellowNeon";
        yellowNeonMaterial.EnableKeyword("_EMISSION");
        yellowNeonMaterial.SetColor("_EmissionColor", new Color(1f, 1f, 0.1f, 1f) * 3.0f); // Bright yellow with intensity
        yellowNeonMaterial.SetFloat("_Smoothness", 1.0f);
        
        // Save material to Assets folder
        SaveMaterialToAssets();
    }
    
    [ContextMenu("Save Material To Assets")]
    void SaveMaterialToAssets()
    {
        #if UNITY_EDITOR
        // Create Materials directory if it doesn't exist
        if (!Directory.Exists(materialsFolder))
        {
            Directory.CreateDirectory(materialsFolder);
            AssetDatabase.Refresh();
        }
        
        // Save Yellow Neon Material
        if (yellowNeonMaterial != null)
        {
            string yellowPath = materialsFolder + "/YellowNeon.mat";
            
            // Check if material already exists
            Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(yellowPath);
            if (existingMaterial != null)
            {
                // Update existing material
                EditorUtility.CopySerialized(yellowNeonMaterial, existingMaterial);
                AssetDatabase.SaveAssets();
            }
            else
            {
                // Create new material asset
                AssetDatabase.CreateAsset(yellowNeonMaterial, yellowPath);
            }
        }
        
        // Refresh the AssetDatabase
        AssetDatabase.Refresh();
        #else
        Debug.LogWarning("Material saving is only available in the Unity Editor.");
        #endif
    }
    
    [ContextMenu("Apply Material To Self")]
    void ApplyMaterialToSelf()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
            
            if (objectRenderer == null)
            {
                Debug.LogError("This GameObject doesn't have a Renderer component!");
                return;
            }
        }
        
        // Apply the material
        if (yellowNeonMaterial != null)
        {
            objectRenderer.material = yellowNeonMaterial;
        }
        else
        {
            Debug.LogWarning("Yellow neon material not available. Please create it first.");
        }
    }
}