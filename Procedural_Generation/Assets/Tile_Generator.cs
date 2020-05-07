using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Generator : MonoBehaviour
{
    [SerializeField]
    private TerrainType[] terrainTypes;

    [SerializeField]
    Noise_Map_Generation noiseMapGen;
    

    [SerializeField]
    private MeshRenderer tileRender;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private float heightMultiplier;


    [SerializeField]
    private AnimationCurve heightCurve;

    void Start() 
    {
        GenerateTile();    
    }

    void GenerateTile()
    {
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        //getting mesh vertices from the plane       
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);//getting number of vertices on a side of plane
        int tileWidth = tileDepth;//since plane is square
    
        float[,] heightMap = this.noiseMapGen.GenerateNoiseMap(tileDepth,tileWidth,this.mapScale);//gets noise map values from generator

        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRender.material.mainTexture = tileTexture;

    }

    private Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);


        Color[] colormap = new Color[tileDepth*tileWidth];
        for(int z=0;z<tileDepth;z++)
        {
            for(int x=0;x<tileWidth;x++)
            {
                //transform the 2D map index as an Array Index
                int ColorIndex = z * tileWidth + x;
                float height = heightMap[z,x];
                TerrainType terrainType = ChooseTerrainType(height);
                //assign height with a given color
                colormap[ColorIndex] = terrainType.color;
                //lerp helps in giving a value between black and white(different shades of grey) and multiplies with height
            }
        }

        Texture2D tileTexture = new Texture2D(tileWidth,tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colormap);
        tileTexture.Apply();

        return tileTexture;
    }
    TerrainType ChooseTerrainType(float height)
    {
        foreach(TerrainType terrainType in terrainTypes)
        {
            if(height<terrainType.height)
            {
                return terrainType;
            }

        }   
        return terrainTypes [terrainTypes.Length-1];
    }
      private void UpdateMeshVertices(float [,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        //go through all height map vertices(coordinates)
        int vertexIndex = 0;
        for(int z =0;z<tileDepth;z++)
        {
            for(int x=0;x<tileWidth;x++)
            {
                float height = heightMap[z,x];

                Vector3 vertex = meshVertices[vertexIndex];

                meshVertices[vertexIndex] = new Vector3(vertex.x,this.heightCurve.Evaluate(height)*this.heightMultiplier ,vertex.z);

                vertexIndex++;
                  
                this.meshFilter.mesh.vertices = meshVertices;
                this.meshFilter.mesh.RecalculateBounds();
                this.meshFilter.mesh.RecalculateNormals();
                this.meshCollider.sharedMesh = this.meshFilter.mesh;
            }
        }
        
        /*this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        //Update the mesh Collider
        //this.meshFilter.mesh.Optimize();
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
*/
    }




}

/*TerrainType ChooseTerrainType(float height)
{
    foreach(TerrainType terrainType in terrainTypes)
    {
        if(height<terrainType.height)
        {
            return terrainType;
        }

    }
    return terrainTypes [terrainTypes.Length-1];
}*/ 




[System.Serializable]
public class TerrainType
{
    public string name;
    public float height;
    public Color color;
}

