using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise_Map_Generation : MonoBehaviour
{
    public float[,] GenerateNoiseMap(int mapDepth,int mapWidth,float scale)
    {
        float[,] noiseMap = new float[mapDepth,mapWidth];


        for(int z=0;z<mapDepth;z++)
        {
            for(int x=0;x<mapWidth;x++)
            {
                float sampleX = x/scale;
                float sampleZ = z/scale;

                float noise = Mathf.PerlinNoise(sampleX,sampleZ);
                noiseMap[z,x] = noise;
            }
        }
        return noiseMap;
    }
}
