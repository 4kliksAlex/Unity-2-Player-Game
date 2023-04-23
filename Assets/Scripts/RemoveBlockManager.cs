
using UnityEngine;

public class RemoveBlockManager : MonoBehaviour
{
    public GameObjectMatrix matrixPrefab;

    [Range(0.0f, 1.0f)]
    public float blockRemoveFactor;
    public bool[,] blockToBeRemoved;

    public int gamerCount = 2;

    public Vector3 positionPlayer0 = new(-5, 0, 0);
    public Vector3 positionPlayer1 = new(5, 0, 0);



    private void Start()
    {
        blockToBeRemoved = new bool[matrixPrefab.xLogicalSize,matrixPrefab.yLogicalSize];

        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 1;
        RemoveSomeBlocks();
    }

    void RemoveSomeBlocks()
    {
        double removeBlockAmount = (blockRemoveFactor * (double)(matrixPrefab.xLogicalSize) * (double)(matrixPrefab.yLogicalSize - 2));
        // save the first row of blocks
        if (removeBlockAmount < 1)
            removeBlockAmount = 1;
        if (removeBlockAmount >= (matrixPrefab.xLogicalSize) * (matrixPrefab.yLogicalSize - 2))
            removeBlockAmount = (matrixPrefab.xLogicalSize) * (matrixPrefab.yLogicalSize - 2) - 1;
        Vector2Int toBeRemoved;

        for (int removeBlockCount = 0; removeBlockCount < removeBlockAmount; removeBlockCount++)
        { // 可能会重复移除。昨天凌晨改到2点改不好，暂时不改了 ——2023年4月20日 14点10分
            toBeRemoved = new Vector2Int(Random.Range(0, matrixPrefab.xLogicalSize), Random.Range(0, matrixPrefab.yLogicalSize - 2));
            blockToBeRemoved[toBeRemoved.x, toBeRemoved.y] = true;
        }
    }

    //void Start()
    //{

    //    GameObjectMatrix[] matrices = new GameObjectMatrix[2];

    //    for (int i = 0; i < gamerCount; i++)
    //    {
    //        matrices[i] = Instantiate(matrixPrefab, transform);
    //        matrices[i].isActive = true;
    //        matrices[i].playerID = i.ToString();
    //    }

    //    matrices[0].transform.position = positionPlayer0;
    //    matrices[1].transform.position = positionPlayer1;
    //    matrixPrefab.isActive = false;

    //}
}