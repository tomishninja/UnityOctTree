using UnityEngine;
using octTree;
using System.IO;

public class OctTreeTestManager : MonoBehaviour
{
    public int x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        int[] array = new int[x * y * z];
        for(int index = 0; index < array.Length; index++)
        {
            array[index] = 1;
        }

        OctTreeInt test = new OctTreeInt(array, x, y, z);
        //Debug.Log(test.ToString()); // very slow
        File.WriteAllText("octTreeData.txt", test.ToString());
    }
}
