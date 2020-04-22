using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

struct Triangle
{
    #pragma warning disable 649 // disable unassigned variable warning
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Vector3 this [int i] {
        get {
            switch (i) {
                case 0:
                    return a;
                case 1:
                    return b;
                default:
                    return c;
            }
        }
    }
}

[ExecuteInEditMode]
public class DensityGenerator : MonoBehaviour
{
    float timeDuration, timeStart, totalTimeElapsed;
    public GameObject generatedMeshObjectsHolder;
    public bool autoUpdate;

    [Header("Compute shaders")]
    public Vector3Int numThreadGroup;
    public ComputeShader _marchingCubeShader;
    public ComputeShader _densityShader;

    [Header("Volume data parameter")]
    public bool isWorldSpaceNoise;
    public int noiseSeed;
    public float isoLevel;
    //[Range(0,1)]
    public float smoothingFactor;
    [Range(1,16)]
    public int octaves;
    [Range(0,1)]
    public float noiseIntensity;
    public float frequency;
    public float lacunarity;
    
    public float noiseScale;
    
    [Header("Generated meshes")]
    public Material material;
    public Vector3Int numberOfGeneratedMeshObject = Vector3Int.one;
    int generatedMeshCount;
    GeneratedMeshProperties _generatedMeshObject;
    public bool showGizmos, showVoxelDensity, showGeneratedVertexInfo;

    [HideInInspector]
    public ComputeBuffer _pointsBuffer, _linePropBuffer, _nodePositionBuffer;
    public LineGenerator _lineGenerator;
    public Voxel _voxel;
    
    bool isLoaded = false;
    Vector4[] _pointsDensity;
    [HideInInspector]
    
    const int numThreads = 8;
    ComputeBuffer _computeBuffer, _resultBuffer, _meshBuffer, _triCountBuffer, _noiseOffsetBuffer;
    Mesh _generatedMesh;
    
    void OnEnable()
    {
        
    }

    void Update()
    {

    }

    //Deklarasi variabel untuk compute shader
    //Variable declaration for the compute shader
    void SetGeneralInputs(ComputeShader shader)
    {
        shader.SetBuffer(0, "samplePoints", _pointsBuffer);
        shader.SetInt("numVertexX",_voxel.numVoxel.x+1);
        shader.SetInt("numVertexY",_voxel.numVoxel.y+1);
        shader.SetInt("numVertexZ",_voxel.numVoxel.z+1);
        shader.SetFloat("voxelSize", _voxel.size);
        shader.SetFloat("isoLevel", isoLevel);
    }

    //Tampilkan nilai density dan posisi dari titik sampel
    //Display density and position of a sample point
    public void ShowVoidVertex()
    {
        for(int i=0; i<_pointsDensity.Length; i++)
        {
            if(_pointsDensity[i].w>-1)//&&_pointsDensity[i].w!=1)
            {
                Debug.Log("Density at index "+i+" is "+_pointsDensity[i].w+ " at position "+_pointsDensity[i].x+", "+_pointsDensity[i].y+", "+_pointsDensity[i].z+".");
            }
        }
    }

    void InitGeneratedMesh()
    {
        GameObject newGameObject;
        int childCount = generatedMeshObjectsHolder.transform.childCount;
        int difference = generatedMeshCount - childCount;
        if(difference>0)
        {
            for(int i=0; i<difference; i++)
            {
                newGameObject = new GameObject();
                newGameObject.AddComponent<GeneratedMeshProperties>().Initiate(material);
                newGameObject.transform.SetParent(generatedMeshObjectsHolder.transform);
            }
        }
        else if(difference<0)
        {
            for(int i=0; i<Mathf.Abs(difference); i++)
            {
                DestroyImmediate(generatedMeshObjectsHolder.transform.GetChild(generatedMeshObjectsHolder.transform.childCount-1).gameObject);
            }
        }

    }

    //Membuat titik sampel beserta nilai density-nya
    //Creates sample points along its density value
    public void GenerateDensity()
    {
        if(autoUpdate!=false)
        {
            return;
        }
        Debug.ClearDeveloperConsole();
        totalTimeElapsed=0;
        timeDuration=0;
        timeStart=0;
        generatedMeshCount = numberOfGeneratedMeshObject.x*numberOfGeneratedMeshObject.y*numberOfGeneratedMeshObject.z;

        var rng = new System.Random(noiseSeed);
        
        InitGeneratedMesh();
        Vector3 offset = Vector3.zero;
        int index;
        
        //Buffer for sample points. Value for second parameter is based on Vector4 size in byte
        _pointsBuffer = new ComputeBuffer(_voxel.totalVertex, 16);
        //Buffer for line data. Value for second parameter is based on LineProperties struct's size in byte
        _linePropBuffer = new ComputeBuffer(_lineGenerator.line.Count, 52);
        //Buffer for line's node data. Value for second parameter is based on NodeProperties struct's size in byte
        _nodePositionBuffer = new ComputeBuffer(_lineGenerator.node.Count, 20);

        Vector3[] octaveOffset = new Vector3[octaves];
        _pointsDensity = new Vector4[_voxel.totalVertex];

        for(int i=0; i<octaves; i++)
        {
            octaveOffset[i] = new Vector3((float)rng.NextDouble()*2-1, (float)rng.NextDouble()*2-1, (float)rng.NextDouble()*2-1);
        }
        _noiseOffsetBuffer = new ComputeBuffer(octaveOffset.Length, 12);

        _linePropBuffer.SetData(_lineGenerator.GetLineProps());
        _nodePositionBuffer.SetData(_lineGenerator.GetNodes());
        _noiseOffsetBuffer.SetData(octaveOffset);

        SetGeneralInputs(_densityShader);
        _densityShader.SetBuffer(0, "nodeProps", _nodePositionBuffer);
        _densityShader.SetBuffer(0, "lineProps", _linePropBuffer);
        _densityShader.SetBuffer(0,"noiseOffset", _noiseOffsetBuffer);
        _densityShader.SetBool("isWorldSpaceNoise", isWorldSpaceNoise);
        _densityShader.SetFloat("noiseIntensity", noiseIntensity);
        _densityShader.SetFloat("noiseScale", noiseScale);
        _densityShader.SetFloat("smoothingFactor", smoothingFactor);

        for(int i=0; i<_lineGenerator.line.Count; i++)
        {
            timeStart = (float)System.DateTime.Now.Second + ((float)System.DateTime.Now.Millisecond/1000);
            _densityShader.Dispatch(0,numThreadGroup.x,numThreadGroup.y,numThreadGroup.z);
            //_densityShader.Dispatch(0,_voxel.numVoxel.x+1,_voxel.numVoxel.y+1,_voxel.numVoxel.z+1);                
            _pointsBuffer.GetData(_pointsDensity);
            //generatedMeshObjectsHolder.transform.GetChild(index).GetComponent<GeneratedMeshProperties>().Initiate();
                    
        }

        /*for(int k=0; k<numberOfGeneratedMeshObject.z; k++)
        {
            for(int j=0; j<numberOfGeneratedMeshObject.y; j++)
            {
                for(int i=0; i<numberOfGeneratedMeshObject.x; i++)
                {
                    timeStart = (float)System.DateTime.Now.Second + ((float)System.DateTime.Now.Millisecond/1000);
                    
                    offset.x = i*_voxel.boundaryWorldPos.x;
                    offset.y = j*_voxel.boundaryWorldPos.y;
                    offset.z = k*_voxel.boundaryWorldPos.z;
                    index = i + (k*numberOfGeneratedMeshObject.x) + (j*(numberOfGeneratedMeshObject.x*numberOfGeneratedMeshObject.z));
                    
                    _densityShader.SetVector("offset", offset);

                    _densityShader.Dispatch(0,numThreadGroup.x,numThreadGroup.y,numThreadGroup.z);
                    //_densityShader.Dispatch(0,_voxel.numVoxel.x+1,_voxel.numVoxel.y+1,_voxel.numVoxel.z+1);
                    
                    _pointsBuffer.GetData(_pointsDensity);

                    //generatedMeshObjectsHolder.transform.GetChild(index).GetComponent<GeneratedMeshProperties>().Initiate();
                    
                }
            }
        }*/
        GenerateMesh(0);
        //Debug.Log("Total time elapsed: "+ totalTimeElapsed);
        if(showGeneratedVertexInfo) ShowVoidVertex();
        ReleaseBuffers();
    }

    public void GenerateMesh(int index)
    {
        //Simpan jumlah triangle dalam satu mesh
        _triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        //Simpan data posisi triangle dalam satu mesh
        _meshBuffer = new ComputeBuffer((int)_voxel.totalVoxel*5, 36, ComputeBufferType.Append); //36 = sizeof(float/int 4 byte) * 3(3 nilai float untuk data posisi vertex) * 3(3 nilai int untuk 3 vertex yang tersambung untuk membentuk triangle)

        SetGeneralInputs(_marchingCubeShader);
        _marchingCubeShader.SetBuffer(0, "triangles", _meshBuffer);
        _marchingCubeShader.SetFloat("isoLevel", isoLevel);
        _marchingCubeShader.Dispatch(0,numThreadGroup.x, numThreadGroup.y, numThreadGroup.z);
        //_marchingCubeShader.Dispatch(0,_voxel.numVoxel.x, _voxel.numVoxel.y, _voxel.numVoxel.z);

        ComputeBuffer.CopyCount (_meshBuffer, _triCountBuffer, 0);
        int[] triCountArray = { 0 };
        _triCountBuffer.GetData (triCountArray);
        int numTris = triCountArray[0];

        Triangle[] tris = new Triangle[numTris];
        _meshBuffer.GetData (tris, 0, 0, numTris);

        Mesh genMesh = generatedMeshObjectsHolder.transform.GetChild(index).GetComponent<GeneratedMeshProperties>()._generatedMesh;
        if(genMesh!=null)
        {
            genMesh.Clear();
        }
        else
        {
            genMesh = new Mesh();
        }

        Vector3[] vert = new Vector3[numTris * 3];
        int[] meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vert[i * 3 + j] = tris[i][j];
            }
        }
        genMesh.SetVertices(vert);
        genMesh.SetTriangles(meshTriangles,0);
        //genMesh.vertices = vert;
        //genMesh.triangles = meshTriangles;

        genMesh.RecalculateNormals();
        generatedMeshObjectsHolder.transform.GetChild(index).GetComponent<GeneratedMeshProperties>().RefreshMeshFilter(genMesh);
        //ReleaseBuffers();
        timeDuration = (float)System.DateTime.Now.Second + ((float)System.DateTime.Now.Millisecond/1000) - timeStart;
        totalTimeElapsed += timeDuration;
        //Debug.Log("Duration: "+ timeDuration);
        //Debug.Log("Mesh is generated successfully");
    }

    void GizmosDrawSquare(int index)
    {
        Gizmos.color = new Color(_pointsDensity[index].w,_pointsDensity[index].w,_pointsDensity[index].w);
        Gizmos.DrawSphere(new Vector3(_pointsDensity[index].x,_pointsDensity[index].y,_pointsDensity[index].z), _voxel.size/4);

        Gizmos.color = new Color(_pointsDensity[index+1].w,_pointsDensity[index+1].w,_pointsDensity[index+1].w);
        Gizmos.DrawSphere(new Vector3(_pointsDensity[index+1].x,_pointsDensity[index+1].y,_pointsDensity[index+1].z), _voxel.size/4);

        Gizmos.color = new Color(_pointsDensity[index+_voxel.numVertex.x+1].w,_pointsDensity[index+_voxel.numVertex.x+1].w,_pointsDensity[index+_voxel.numVertex.x+1].w);
        Gizmos.DrawSphere(new Vector3(_pointsDensity[index+_voxel.numVertex.x+1].x,_pointsDensity[index+_voxel.numVertex.x+1].y,_pointsDensity[index+_voxel.numVertex.x+1].z), _voxel.size/4);

        Gizmos.color = new Color(_pointsDensity[index+_voxel.numVertex.x].w,_pointsDensity[index+_voxel.numVertex.x].w,_pointsDensity[index+_voxel.numVertex.x].w);
        Gizmos.DrawSphere(new Vector3(_pointsDensity[index+_voxel.numVertex.x].x,_pointsDensity[index+_voxel.numVertex.x].y,_pointsDensity[index+_voxel.numVertex.x].z), _voxel.size/4);
    }

    public void ReleaseBuffers()
    {
        if(_triCountBuffer!=null)
        {
            _pointsBuffer.Release();
            _meshBuffer.Release();
            _triCountBuffer.Release();
            _nodePositionBuffer.Release();
            _noiseOffsetBuffer.Release();
        }
    }

    //Tampilkan titik sampel dengan warna berdasarkan nilai density
    //Display the sample points with color based on its density
    public void OnDrawGizmos()
    {
        Vector3 offset = new Vector3();
        Vector3 centre = _voxel.boundaryWorldPos/2;
        for(int k=0; k<numberOfGeneratedMeshObject.z; k++)
        {
            for(int j=0; j<numberOfGeneratedMeshObject.y; j++)
            {
                for(int i=0; i<numberOfGeneratedMeshObject.x; i++)
                {
                    offset.x = i*_voxel.boundaryWorldPos.x;
                    offset.y = j*_voxel.boundaryWorldPos.y;
                    offset.z = k*_voxel.boundaryWorldPos.z;
                    Gizmos.color = new Color(0,0,0);
                    Gizmos.DrawWireCube(centre+offset, centre*2);
                }
            }
        }

        for(int k=0; k<=numThreadGroup.z; k++)
        {
            for(int j=0; j<=numThreadGroup.y; j++)
            {
                for(int i=0; i<=numThreadGroup.x; i++)
                {
                    offset.x = i*_voxel.boundaryWorldPos.x;
                    offset.y = j*_voxel.boundaryWorldPos.y;
                    offset.z = k*_voxel.boundaryWorldPos.z;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(new Vector3(i*8*_voxel.size, 0, 0), new Vector3(i*8*_voxel.size,0,(Mathf.CeilToInt(_voxel.numVertex.z/8)+1)*_voxel.size*8));
                    Gizmos.DrawLine(new Vector3(0, 0, k*8*_voxel.size), new Vector3((Mathf.CeilToInt(_voxel.numVertex.x/8)+1)*_voxel.size*8, 0, k*8*_voxel.size));
                }
            }
        }
        
        if(_pointsDensity!=null && showGizmos)
        {
            if(showVoxelDensity)
            {
                int id = _voxel.voxelGrid.x + (_voxel.voxelGrid.z*(_voxel.numVoxel.x+1)) + (_voxel.voxelGrid.y*_voxel.totalVertexLayer);
                GizmosDrawSquare(id);

                id += _voxel.totalVertexLayer;
                GizmosDrawSquare(id);
            }
            else
            {
                for(int i=0; i<_pointsDensity.Length; i++)
                {
                    if(_pointsDensity[i].w >= isoLevel)
                    {
                        Gizmos.color = new Color(_pointsDensity[i].w,_pointsDensity[i].w,_pointsDensity[i].w);
                        Gizmos.DrawSphere(new Vector3(_pointsDensity[i].x,_pointsDensity[i].y,_pointsDensity[i].z), _voxel.size/8);
                    }
                }
            }
        }
    }

    public void OnValidate()
    {
        _lineGenerator = gameObject.GetComponent<LineGenerator>();
        _voxel = gameObject.GetComponent<Voxel>();
        if(isLoaded==false)
        {
            isLoaded=true;
            EditorApplication.quitting+=ReleaseBuffers;
        }
        octaves = Mathf.Clamp(octaves, 1, 16);
        //numVoxelPerThread = Mathf.Clamp(numVoxelPerThread, 1, _voxel.numVoxel.x);
        //numVoxelPerThread = Mathf.Clamp(numVoxelPerThread, 1, _voxel.numVoxel.y);
        //numVoxelPerThread = Mathf.Clamp(numVoxelPerThread, 1, _voxel.numVoxel.z);
        if(autoUpdate)
        {
            GenerateDensity();
        }
    }
}
