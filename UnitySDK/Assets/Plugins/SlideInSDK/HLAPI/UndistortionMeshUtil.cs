using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Undistortion mesh utility.
    /// </summary>
    public static class UndistortionMeshUtil 
    {
        /// <summary>
        /// Build undistortion mesh parameter.
        /// </summary>
        public struct BuildUndistortionMeshParameter 
        {
            /// <summary>
            /// The mesh to hold the undistortion vertices data.
            /// </summary>
            public Mesh Mesh;

            /// <summary>
            /// if true, mesh is generated at double face
            /// </summary>
            public bool DoubleFace;

            /// <summary>
            /// if true, vertice is flip.
            /// </summary>
            public bool FlipVertices;

            /// <summary>
            /// The vertices.
            /// </summary>
            public Vector3[] Vertices;

            /// <summary>
            /// The grid-size of the undistortion.
            /// X = height (Row)
            /// Y = width (Column)
            /// </summary>
            public Vector2Int GridSize;
        }

        /// <summary>
        /// Builds the undistortion mesh.
        /// </summary>
        /// <param name="BuildParameter">Build parameter.</param>
        public static void BuildUndistortionMesh (BuildUndistortionMeshParameter BuildParameter)
        {
            List<Vector3> vertices = new List<Vector3>(BuildParameter.Vertices);
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

             
            Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity), 
            max = new Vector2(-Mathf.Infinity, -Mathf.Infinity);

            for (int i = 1; i < vertices.Count; i++)
            {
                min.x = Mathf.Min(min.x, vertices[i].x);
                min.y = Mathf.Min(min.y, vertices[i].y);
                max.x = Mathf.Max(max.x, vertices[i].x);
                max.y = Mathf.Max(max.y, vertices[i].y);
            }

            Debug.Log("Total vertices: " + vertices.Count);

            //Triangles:
            {
                triangles.Clear();
                List<int> tris = triangles;

                int Row = BuildParameter.GridSize.x;
                int Col = BuildParameter.GridSize.y;

                System.Func<int, int, int> GetVertexIndex = delegate(int row, int col) 
                {
                    int TotalCol = Col;
//                    int TotalRow = Row;
                    int index = row * TotalCol + col;
                    return index;
                };

                for (int row = 0; row < Row - 1; row++)
                {
                    for (int col = 0; col < Col - 1; col++)
                    {
                        //Odds: 逐行向上:
                        {
                            int t1 = GetVertexIndex(row + 1, col);
                            int t2 = GetVertexIndex(row, col + 1);
                            int t3 = GetVertexIndex(row, col);
                            if(!BuildParameter.FlipVertices)
                            { 
                                tris.Add(t3);
                                tris.Add(t2);
                                tris.Add(t1);
                                if(BuildParameter.DoubleFace)
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
                                if(BuildParameter.DoubleFace)
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

                            if(!BuildParameter.FlipVertices)
                            { 
                                tris.Add(t3);
                                tris.Add(t2);
                                tris.Add(t1);
                                if(BuildParameter.DoubleFace)
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
                                if(BuildParameter.DoubleFace)
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

            //UV:
            {
                int uvWidth = BuildParameter.GridSize.y;
                float uvHeight = BuildParameter.GridSize.x;
                for(int row = 0; row < uvHeight; row++)
                {
                    for(int col = 0; col < uvWidth; col++)
                    {
                        float u = (float)row / (uvWidth - 1);
                        float v = (float)col / (uvHeight - 1);
                        uvs.Add(new Vector2(u, v));
                    }
                }
            }

            BuildParameter.Mesh.Clear();
            BuildParameter.Mesh.vertices = vertices.ToArray();
            BuildParameter.Mesh.triangles = triangles.ToArray();
            BuildParameter.Mesh.uv = uvs.ToArray();
            BuildParameter.Mesh.UploadMeshData(false);

            return;
        }
    }
}