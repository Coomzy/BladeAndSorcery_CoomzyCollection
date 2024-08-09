using System.Collections.Generic;
using System.Reflection;
using ThunderRoad;
using UnityEngine;

namespace Sharingan
{
    public class TexturedSpellIcon
    {
        public string orbId = "Sharingan";
        public MeshRenderer spellWheelMeshRenderer;

        public static List<WheelMenu.Orb> orbCache = new List<WheelMenu.Orb>();

        public TexturedSpellIcon (string orbId, string texturePath, float planeScale = 0.5f)
        {
            this.orbId = orbId;
            spellWheelMeshRenderer = CreateSpellIcon(texturePath, planeScale);
        }

        public static MeshRenderer CreateSpellIcon(string texturePath, float planeScale = 0.5f)
        {
            // Create a new plane GameObject
            GameObject planeObject = new GameObject("DynamicPlane");

            // Add a MeshFilter component to the plane
            MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();
            meshFilter.mesh = GeneratePlaneMesh();

            // Add a MeshRenderer component to the plane
            MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();

            // Load the texture from the specified path
            Texture2D texture = LoadTextureFromFile(texturePath);

            //Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            Shader shader = Shader.Find("Sprites/Default");

            // Create a new material and assign the texture to it
            Material material = new Material(shader);
            material.mainTexture = texture;

            // Set the material on the MeshRenderer
            meshRenderer.material = material;

            // Set the plane's scale
            planeObject.transform.localScale = new Vector3(planeScale, planeScale, 1f);

            return meshRenderer;
        }

        public static Mesh GeneratePlaneMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3(0.5f, -0.5f, 0f),
            new Vector3(-0.5f, 0.5f, 0f),
            new Vector3(0.5f, 0.5f, 0f)
            };

            int[] triangles = new int[6]
            {
            0, 2, 1,
            2, 3, 1
            };

            Vector2[] uv = new Vector2[4]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();

            return mesh;
        }

        public static Texture2D LoadTextureFromFile(string path)
        {
            Texture2D texture = null;

            if (System.IO.File.Exists(path))
            {
                byte[] fileData = System.IO.File.ReadAllBytes(path);
                texture = new Texture2D(2, 2);
                texture.LoadRawTextureData(fileData);
                texture.Apply();
            }
            else
            {
                UnityEngine.Debug.LogError("[Sharingan] Texture file not found: " + path);
            }

            return texture;
        }

        public void Update(WheelMenuSpell wheel)
        {
            if (wheel == null || spellWheelMeshRenderer == null)
            {
                return;
            }

            spellWheelMeshRenderer.enabled = wheel.isShown;

            if (wheel.isShown)
            {
                return;
            }

            FieldInfo orbsField = typeof(WheelMenu).GetField("orbs", BindingFlags.NonPublic | BindingFlags.Instance);

            if (orbsField == null)
            {
                return;
            }

            orbCache = (List<WheelMenu.Orb>)orbsField.GetValue(wheel);

            if (orbCache == null)
            {
                return;
            }

            for (int i = 0; i < orbCache.Count; i++)
            {
                if (orbCache[i].id != orbId)
                {
                    continue;
                }

                spellWheelMeshRenderer.transform.position = orbCache[i].transform.position;
                spellWheelMeshRenderer.transform.rotation = orbCache[i].transform.rotation;
                break;
            }
        }
    }
}
