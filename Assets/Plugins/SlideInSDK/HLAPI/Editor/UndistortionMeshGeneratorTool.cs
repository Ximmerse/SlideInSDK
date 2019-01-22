using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PolyEngine;
using Ximmerse.SlideInSDK;

namespace Ximmerse
{
    /// <summary>
    /// Generate undistortion mesh based on input source file.
    /// </summary>
    public class UndistortionMeshGeneratorTool : UnityEditor.EditorWindow
    {
        public bool testNewUndistortionMesh = true;

        public GameObject NewUndistortionMesh;

        /// <summary>
        /// X = Row, Y = Col, which must be matched to the points defined in [source] file.
        /// </summary>
        Vector2Int Grids = new Vector2Int(0,0);

        /// <summary>
        /// The source file in pattern : 
        /// 
        /// -520 -608 -476 -565
        /// -520 -547 -481 -535
        /// -520 -486 -486 -498
        /// 
        /// Each line denotes an undistortion pair , where first of two denotes the original point, and the latter two denotes transferred values.
        /// 
        /// </summary>
        public TextAsset source;

        public bool DoubleFace = false;

        public bool FlipVertices = false;

        List<Vector3> vertices = new List<Vector3>();

        List<int> triangles = new List<int>();

        List<Vector2> uvs = new List<Vector2>();

        Vector2 min, max;

        public Mesh mesh;

        /// <summary>
        /// The scale of the vertices.
        /// </summary>
        public float scale = 0.001f;

        static UndistortionMeshGeneratorTool instance;

        SerializedObject m_serializedObject = null;
        SerializedObject serializedObject { get{ if (m_serializedObject == null) m_serializedObject = new SerializedObject(this); return m_serializedObject; }}

        [MenuItem ("Tools/Slide in/Undistortation Mesh Generator")]
        static void ShowToolWindow()
        {
            instance = EditorWindow.GetWindow<UndistortionMeshGeneratorTool>(true);
            instance.ShowUtility();
        }

		private void OnGUI()
		{
            var serializeObj = this.serializedObject;
            EditorGUILayout.PropertyField(serializeObj.FindProperty("source"));
            EditorGUILayout.PropertyField(serializeObj.FindProperty("FlipVertices"));
            EditorGUILayout.PropertyField(serializeObj.FindProperty("DoubleFace"));
            EditorGUILayout.PropertyField(serializeObj.FindProperty("scale"), new GUIContent ("Scale of the vertices", ""));
            serializeObj.ApplyModifiedProperties();

            if(source != null)
            {
                if(GUILayout.Button("Generate"))
                {
//                    RebuildTMesh();

                    var path = Path.GetFullPath(AssetDatabase.GetAssetPath(source));
                    var lines = File.ReadAllLines(path);
                    Vector3[] vertices;
                    Vector2Int gridSize;
                    if(!ParseMatrixFile(lines, this.scale, out vertices, out gridSize))
                    {
                        return;
                    }
                    mesh = new Mesh();
                    Grids = gridSize;
                    UndistortionMeshUtil.BuildUndistortionMesh(
                        new UndistortionMeshUtil.BuildUndistortionMeshParameter()
                    {
                        Mesh = mesh,
                        DoubleFace = this.DoubleFace,
                        FlipVertices = this.FlipVertices,
                        GridSize = this.Grids,
                        Vertices = vertices,
                    }
                    );
                }
            }

            if(mesh != null)
            {
                if(GUILayout.Button("Save Mesh"))
                {
                    string path = EditorUtility.SaveFilePanel("Save mesh to asset", "", "UndistortionMesh", "asset");
                    if(!string.IsNullOrEmpty(path))
                    {
                        path = FileUtil.GetProjectRelativePath(path);
                        AssetDatabase.CreateAsset(mesh, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        var prjMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        EditorGUIUtility.PingObject(prjMesh);
                    }
                }
            }
		}

		[InspectFunction]
        void RebuildTMesh()
        {
            var path = Path.GetFullPath(AssetDatabase.GetAssetPath(source));
            Debug.Log(path);
            var lines = File.ReadAllLines(path);
            ParseLinesAndRebuild(lines);
            Debug.Log("Undisortion mesh has been generated !");
        }
        [InspectFunction]
        void SaveMesh()
        {
            string saveFile = string.Empty;
            if (!string.IsNullOrEmpty(saveFile = UnityEditor.EditorUtility.SaveFilePanelInProject(string.Empty, "Distortion", "asset", string.Empty)))
            {
                UnityEditor.AssetDatabase.CreateAsset(this.mesh, saveFile);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        void ParseLinesAndRebuild(string[] lines)
        {
            vertices.Clear();

            //first line: [Row]X[Col]:
            var firstLine = lines[0];
            string[] firstLineCells = firstLine.Split( new char[] { 'X' }, System.StringSplitOptions.RemoveEmptyEntries);
            Grids.x = int.Parse(firstLineCells[0]);
            Grids.y = int.Parse(firstLineCells[1]);


            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var cells = line.Split(new char[] {
                    ' '
                });
                float x = float.Parse(cells[2]) * scale;
                float y = float.Parse(cells[3]) * scale;
                float z = 0;
                vertices.Add(new Vector3(x, y, z));
                min.x = Mathf.Min(min.x, x);
                min.y = Mathf.Min(min.y, y);
                max.x = Mathf.Max(max.x, x);
                max.y = Mathf.Max(max.y, y);
            }

            Debug.Log("Total vertices: " + vertices.Count);

            GetTriangles();

            GetUVs();

            mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = this.uvs.ToArray();
            mesh.UploadMeshData(false);

            return;
        }


        // Taken from https://github.com/googlevr/gvr-unity-sdk/blob/v1.40.0/GoogleVR/Legacy/Scripts/Internal/GvrPostRender.cs
        public void GetTriangles()
        {
            triangles.Clear();
            List<int> tris = triangles;

            int Col = Grids.y;
            int Row = Grids.x;

            for (int row = 0; row < Row - 1; row++)
            {
                for (int col = 0; col < Col - 1; col++)
                {
                    //Odds: 逐行向上:
                    {
                        int t1 = GetVertexIndex(row + 1, col);
                        int t2 = GetVertexIndex(row, col + 1);
                        int t3 = GetVertexIndex(row, col);
                        if(!FlipVertices)
                        { 
                            tris.Add(t3);
                            tris.Add(t2);
                            tris.Add(t1);
                            if(DoubleFace)
                            {
                                tris.Add(t1);
                                tris.Add(t2);
                                tris.Add(t3);
                            }
                        }
                        else 
                        {
                            tris.Add(t1);
                            tris.Add(t2);
                            tris.Add(t3);
                            if(DoubleFace)
                            {
                                tris.Add(t3);
                                tris.Add(t2);
                                tris.Add(t1);
                            }
                        }
                    }

                    //Evens: 逐行向下:
                    {
                        int t1 = GetVertexIndex(row + 1, col);
                        int t2 = GetVertexIndex(row + 1, col + 1);
                        int t3 = GetVertexIndex(row, col + 1);

                        if(!FlipVertices)
                        { 
                            tris.Add(t3);
                            tris.Add(t2);
                            tris.Add(t1);
                            if(DoubleFace)
                            {
                                tris.Add(t1);
                                tris.Add(t2);
                                tris.Add(t3);
                            }
                        }
                        else 
                        {
                            tris.Add(t1);
                            tris.Add(t2);
                            tris.Add(t3);
                            if(DoubleFace)
                            {
                                tris.Add(t3);
                                tris.Add(t2);
                                tris.Add(t1);
                            }
                        }
                    }
                }
            } 
        }

        void GetUVs()
        {
            this.uvs.Clear();
            int uvWidth = Grids.y;
            float uvHeight = Grids.x;
            for(int row = 0; row < Grids.x; row++)
            {
                for(int col = 0; col < Grids.y; col++)
                {
                    float u = (float)row / (uvWidth - 1);
                    float v = (float)col / (uvHeight - 1);
                    this.uvs.Add(new Vector2(u, v));
                }
            }
        }

        int GetVertexIndex(int row, int col)
        {
            int TotalCol = Grids.y;
            int TotalRow = Grids.x;

            int index = row * TotalCol + col;
            return index;
        }

        /// <summary>
        /// Parses the matrix file and output vertices and grid size.
        /// lines[0] should be in pattern like: 30X20, indicating the undistortion mesh width = 30, height = 20
        /// lines[1]..[N] is the vertice data.
        /// </summary>
        /// <returns><c>true</c>, if matrix file was parsed, <c>false</c> otherwise.</returns>
        /// <param name="lines">Lines.</param>
        /// <param name="Vertices">Vertices.</param>
        /// <param name="GridSize">Grid size.</param>
        static bool ParseMatrixFile(string[] lines, float verticeScale, out Vector3[] Vertices, out Vector2Int GridSize)
        {
            try
            {
                //first line: [Row]X[Col]:
                var firstLine = lines[0];
                Vertices = new Vector3[lines.Length - 1];
                string[] firstLineCells = firstLine.Split( new char[] { 'X' }, System.StringSplitOptions.RemoveEmptyEntries);
                GridSize = new Vector2Int(int.Parse(firstLineCells[0]), int.Parse(firstLineCells[1]));
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var cells = line.Split(new char[] {
                        ' '
                    });
                    float x = float.Parse(cells[2]) * verticeScale;
                    float y = float.Parse(cells[3]) * verticeScale;
                    float z = 0;
                    Vertices[i-1] = new Vector3(x, y, z);
                }
                return true;
            }
            catch(System.Exception exc)
            {
                Debug.LogException(exc);
                Vertices = new Vector3[] { };
                GridSize = Vector2Int.zero;
                return false;
            }
        }


    }
}