using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshSaverEditor {

	[MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
	public static void SaveMeshInPlace (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh m = mf.sharedMesh;
		SaveMesh(m, m.name, false, true);
	}

	[MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
	public static void SaveMeshNewInstanceItem (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh m = mf.sharedMesh;
		SaveMesh(m, m.name, true, true);
	}

    ///<summary>
	/// P: My Modification2 // Works for Multi material Made my Pavel:
	/// Better to apply 2 times if Transform has non defaut TRS
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("CONTEXT/MeshFilter/Save MultiMat Transformed Mesh As New Instance...")]
    public static void SaveTransformedMeshNewInstanceItem(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = CloneMesh(mf.sharedMesh);
		var vertices = m.vertices;
		TransformPoints(mf.transform, vertices);
		m.vertices = vertices;
        SaveMesh(m, m.name, true, true);
    }
    ///<summary>
    /// P: My Modification3 // Works for Multi material Made my Pavel, drops material info and mesh becomes single material:
    /// No vertice velding done on single material mesh
    /// Better to apply 2 times if Transform has non defaut TRS
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("CONTEXT/MeshFilter/Save SingleMat Transformed Mesh As New Instance...")]
    public static void SaveTransformedMeshNewInstanceSingleMaterial(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = CloneMesh(mf.sharedMesh, false);
        var vertices = m.vertices;
        TransformPoints(mf.transform, vertices);
        m.vertices = vertices;
        SaveMesh(m, m.name, true, true);
    }

    private static Mesh CloneMesh(Mesh mesh, bool cloneSubmeshInfo = true)
	{
		var newMesh = new Mesh();
		newMesh.vertices = mesh.vertices;
        newMesh.triangles = mesh.triangles;
        newMesh.normals = mesh.normals;
		newMesh.colors = mesh.colors;
		newMesh.tangents = mesh.tangents;
		newMesh.uv = mesh.uv;

		if (cloneSubmeshInfo)
		{
            int subMeshCount = mesh.subMeshCount;
            newMesh.subMeshCount = subMeshCount;
            for (int i = 0; i < subMeshCount; i++)
            {
                newMesh.SetSubMesh(i, mesh.GetSubMesh(i));
            }
        }

        newMesh.RecalculateBounds();

		return newMesh;
    }
	// end of made by pavel

    public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
		string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
		if (string.IsNullOrEmpty(path)) return;
        
		path = FileUtil.GetProjectRelativePath(path);

		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;
		
		if (optimizeMesh)
		     MeshUtility.Optimize(meshToSave);
        
		AssetDatabase.CreateAsset(meshToSave, path);
		AssetDatabase.SaveAssets();
	}
	
    public static void TransformPoints(Transform t, System.Span<Vector3> points)
    {
        for (int i = 0;i < points.Length; i++)
        {
            points[i] = t.TransformPoint(points[i]);
        }
    }

}
