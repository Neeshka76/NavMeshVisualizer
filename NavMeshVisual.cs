using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NavMeshVisualizer
{
	// Source : https://blog.terresquall.com/2020/07/showing-unitys-navmesh-in-game/
	public class NavMeshVisual : MonoBehaviour
	{
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
		private List<Material> materials;
		public bool hideNavMesh = false;
		public bool increaseIntensity = false;
		private float lowIntensity = 0.25f;
		private float highIntensity = 0.75f;

		private void Awake()
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			materials = new List<Material>();
		}
		void Start()
		{
			meshRenderer.sharedMaterials = materials.ToArray();
			ShowMesh();
		}

		// Generates the NavMesh shape and assigns it to the MeshFilter component.
		void ShowMesh()
		{
			// NavMesh.CalculateTriangulation returns a NavMeshTriangulation object.
			NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();


			// Organise the NavMeshTriangulation data into Dictionary key-value pairs.
			// In this Dictionary, each area in the NavMesh will have its own list of triangles
			// that belong to the area. Each of these lists of triangles will be rendered as
			// a submesh later below.
			Dictionary<int, List<int>> submeshIndices = new Dictionary<int, List<int>>();

			// <meshData.areas> is an int[] that contains an entry for every triangle in
			// <meshData.indices>. The entry helps to identify which Navigation Area each
			// triangle belongs to.
			for (int i = 0; i < meshData.areas.Length; i++)
			{
				// If a list hasn't already been created for this area index, do so.
				if (!submeshIndices.ContainsKey(meshData.areas[i]))
				{
					submeshIndices.Add(meshData.areas[i], new List<int>());
					materials.Add(CreateUIMat(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)), "Area" + i));
					Debug.Log($"Area name : {NavMesh.GetSettingsNameFromID(meshData.areas[i])}");
				}
				// Because a triangle contains 3 points, <meshData.indices> will always be exactly 3 times
				// the size of <meshData.areas>. Each index on <meshData.areas> identifies 3 items in <meshData.indices<,
				// so we have to identify the 3 items that each <meshData.areas< item is referring to.
				submeshIndices[meshData.areas[i]].Add(meshData.indices[3 * i]);
				submeshIndices[meshData.areas[i]].Add(meshData.indices[3 * i + 1]);
				submeshIndices[meshData.areas[i]].Add(meshData.indices[3 * i + 2]);
			}


			meshRenderer.sharedMaterials = materials.ToArray();

			// Create a new mesh and chuck in the NavMesh's vertex and triangle data to form the mesh.
			Mesh mesh = new Mesh();
			mesh.vertices = meshData.vertices;
			//mesh.triangles = meshData.indices;

			// Tell our mesh how many submeshes it contains, and use SetTriangles() to assign 
			// triangles to the different submeshes in the mesh object.
			mesh.subMeshCount = submeshIndices.Count;
			int index = 0;
			foreach (KeyValuePair<int, List<int>> entry in submeshIndices)
			{
				mesh.SetTriangles(entry.Value.ToArray(), index++);
			}

			// Assigns the newly-created mesh to the MeshFilter on the same GameObject.
			meshFilter.mesh = mesh;
		}

		public void Update()
		{
			if (hideNavMesh && meshRenderer.enabled)
			{
				meshRenderer.enabled = false;
			}
			if (!hideNavMesh && !meshRenderer.enabled)
			{
				meshRenderer.enabled = true;
			}
			if (meshRenderer.sharedMaterials[0].color.a != highIntensity && increaseIntensity)
			{
				meshRenderer.sharedMaterials[0].color = new Color(meshRenderer.sharedMaterials[0].color.r, meshRenderer.sharedMaterials[0].color.g, meshRenderer.sharedMaterials[0].color.b, highIntensity);
			}
			if (meshRenderer.sharedMaterials[0].color.a != lowIntensity && !increaseIntensity)
			{
				meshRenderer.sharedMaterials[0].color = new Color(meshRenderer.sharedMaterials[0].color.r, meshRenderer.sharedMaterials[0].color.g, meshRenderer.sharedMaterials[0].color.b, lowIntensity);
			}

		}


		Material CreateUIMat(Color color, string name = "Mat")
		{
			Material mat = new Material(Shader.Find("UI/Default"));
			mat.color = new Color(color.r, color.b, color.g, lowIntensity);
			mat.name = name;
			return mat;
		}
	}
}
