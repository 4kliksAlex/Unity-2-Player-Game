using UnityEngine;

public class GameObjectMatrix : MonoBehaviour
{
    public string playerID;
    public bool isActive = true;
    public bool matrixScrollDebug = false;
    public RemoveBlockManager multiplayerManager;
    public bool isWin;

    public int xLogicalSize = 6;
    public int yLogicalSize = 5;
    public float gridSize = 1.1f;
    public Vector2 upperBoundary;
    public Vector2 lowerBoundary;
    public float mRNAGoesDown = -0.5f;

    //[Range(0.0f, 1.0f)]
    //public float blockRemoveFactor;
    public int codonNumber;

    public Color selectionColor;
    public Color scrollingColor;
    public Color emptyBlockColor;
    public Color blockColor;

    public GameObject blockPrefab;
    public GameObject codonPrefab;

    private GameObject[,] gameObjectMatrix;
    //private GameObject[] codonArray;
    private int[,] objectTypeMatrix;

    private GameObject[] mRNA;
    private int[] mRNAType;

    private bool isSelectionMode = true;
    private Color[,] blockColorMatrix;
    private Color[] objectTypeColorArray;

    private Vector2Int logicalSelection;
    private Vector2Int positionalSelection;

    // Positional coordinate to Logical coordinate Mapping Matrix
    private Vector2Int[,] PCTLCMM;
    private Vector2Int[,] tempPCTLCMM;

    float horizontalInput;
    float verticalInput;
    private bool axisReleased = true;
    private bool xAxisReleased = true;
    private bool yAxisReleased = true;

    int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    int ModIncrement(int x, int m)
    {
        return Mod((x + 1), m);
    }
    int ModDecrement(int x, int m)
    {
        return Mod((x - 1), m);
    }

    private void Start()
    {
        Initiate();
        MakeBlocks();
        PlaceSomeCodons();

        RemoveSomeBlocks();
        MakeMRNA();

        MatrixRenderer(matrixScrollDebug);

        gameObject.SetActive(isActive);
    }
    private void Update()
    {
        CheckAxisReleased();
        CodonGravity();
        isWin = CheckWin();

        horizontalInput = Input.GetAxisRaw("Horizontal" + playerID);
        verticalInput = Input.GetAxisRaw("Vertical" + playerID);

        if (Input.GetButtonDown("Jump" + playerID))
        {
            isSelectionMode = !isSelectionMode;

            //MatrixDebugLog(tempPCTLCMM);
            //MatrixDebugLog(PCTLCMM);
            //MatrixDebugLog(isBlockMatrix);
            //MatrixDebugLog(objectTypeMatrix);
            //MatrixDebugLog(blockColorMatrix);
            //for (int pos_i = 0; pos_i < codonNumber + 1; pos_i++)
            //{
            //    Debug.Log(objectTypeColorArray[pos_i].ToString());
            //}
            //PositionalMatrixDebugLog(objectTypeMatrix);

        }

        if (axisReleased) // input movement
        {
            if (isSelectionMode)
            {
                PositionalCoordinateSelector((int)horizontalInput, (int)verticalInput);
            }
            else //not in selection mode
            {
                ScrollInRow((int)horizontalInput);
                ScrollInColumn((int)verticalInput);
            }
        }

        MatrixRenderer(matrixScrollDebug);

    }

    void Initiate()
    {
        blockPrefab.SetActive(false);
        codonPrefab.SetActive(false);

        upperBoundary = new Vector2(
            (xLogicalSize * gridSize) / 2,
            (yLogicalSize * gridSize) / 2
        );
        lowerBoundary = new Vector2(
            -(xLogicalSize * gridSize) / 2,
            -(yLogicalSize * gridSize) / 2
        );

        gameObjectMatrix = new GameObject[xLogicalSize, yLogicalSize];

        mRNA = new GameObject[xLogicalSize];
        mRNAType = new int[xLogicalSize];

        blockColorMatrix = new Color[xLogicalSize, yLogicalSize];
        objectTypeColorArray = new Color[codonNumber + 2];

        for (int i = 0; i < codonNumber + 2; i++)
        {
            objectTypeColorArray[i] = Color.HSVToRGB((float)i / (float)(codonNumber + 2), 1, 1);
        }

        objectTypeColorArray[0] = blockColor;
        objectTypeColorArray[codonNumber + 1] = emptyBlockColor;

        PCTLCMM = new Vector2Int[xLogicalSize, yLogicalSize];
        tempPCTLCMM = new Vector2Int[xLogicalSize, yLogicalSize];
        objectTypeMatrix = new int[xLogicalSize, yLogicalSize];
    }

    public bool CheckWin()
    {
        int j = 0;
        for (int i = xLogicalSize - codonNumber; i < xLogicalSize; i++)
        {
            if (objectTypeMatrix[PCTLCMM[i, j].x, PCTLCMM[i, j].y] != mRNAType[i])
            {
                return false;
            }
        }
        return true;
    }

    void MakeBlocks()
    {
        for (int i = 0; i < xLogicalSize; i++)
        {
            for (int j = 0; j < yLogicalSize; j++)
            {
                gameObjectMatrix[i, j] = Instantiate(blockPrefab, transform);

                Vector3 positionShift = new((-(float)xLogicalSize / 2 + (float)i + 0.5f) * gridSize, (-(float)yLogicalSize / 2 + (float)j + 0.5f) * gridSize);

                gameObjectMatrix[i, j].transform.position += positionShift;

                blockColorMatrix[i, j] =
                    Color.HSVToRGB((i + j * xLogicalSize) /
                    (float)(xLogicalSize * yLogicalSize), 1, 1);

                gameObjectMatrix[i, j].SetActive(true);
                objectTypeMatrix[i, j] = 0;
                PCTLCMM[i, j] = new Vector2Int(i, j);
            }
        }

    }

    //void MakeCodons()
    //{


    //    if (codonNumber < 1)
    //        codonNumber = 1;
    //    if (codonNumber > xLogicalSize)
    //        codonNumber = xLogicalSize;

    //    for (int pos_i = 0; pos_i < codonNumber; pos_i++)
    //    {

    //        codonArray[pos_i] = Instantiate(codonPrefab, transform);
    //        Vector3 positionShift = new(
    //            (-(float)xLogicalSize / 2 + (float)pos_i + 0.5f) * gridSize,
    //            (-(float)yLogicalSize / 2 + (float)yLogicalSize + 0.5f) * gridSize
    //            );

    //        codonArray[pos_i].transform.position =
    //            transform.position + positionShift;

    //        codonColorArray[pos_i] =
    //            Color.HSVToRGB(pos_i / (float)codonNumber, 1, 1);

    //        codonArray[pos_i].GetComponent<SpriteRenderer>().color = codonColorArray[pos_i];

    //        codonArray[pos_i].SetActive(true);
    //    }
    //}

    void PlaceSomeCodons()
    {
        int j = yLogicalSize - 1;
        for (int i = 0; i < xLogicalSize; i++)
        {
            if (i < codonNumber)
            {
                objectTypeMatrix[i, j] = i + 1;// pos_j 表示密码子的idientity

                //gameObjectMatrix[i_reverse, j].GetComponent<SpriteRenderer>().sprite = codonPrefab.GetComponent<SpriteRenderer>().sprite;

            }
            else
            {
                objectTypeMatrix[i, j] = -1;
            }
        }
    }

    void RemoveSomeBlocks()
    {
        for (int i = 0; i < xLogicalSize; i++)
        {
            for (int j = 0; j < yLogicalSize; j++)
            {
                if (multiplayerManager.blockToBeRemoved[i, j])
                {
                    objectTypeMatrix[i, j] = -1;
                }
            }
        }
    }

    //output the PCTLCM matrix for debugging:

    void MakeMRNA()
    {
        for (int i = 0; i < xLogicalSize; i++)
        {
            int i_reverse = xLogicalSize - 1 - i;

            if (i_reverse < codonNumber)
            {
                mRNAType[i] = i_reverse + 1;
                mRNA[i] = Instantiate(codonPrefab, transform);
            }
            else
            {
                mRNAType[i] = 0;
                mRNA[i] = Instantiate(blockPrefab, transform);
            }

            mRNA[i].GetComponent<SpriteRenderer>().color = objectTypeColorArray[mRNAType[i]];
        }
        for (int i = 0; i < xLogicalSize; i++)
        {
            Vector3 positionShift = new((-(float)xLogicalSize / 2 + (float)i + 0.5f) * gridSize, (-(float)yLogicalSize / 2 + (float)(-mRNAGoesDown) + 0.5f + -1.0f) * gridSize);

            mRNA[i].SetActive(true);

            mRNA[i].transform.position += (positionShift);
        }
    }


    void MatrixDebugLog<T>(T[,] debugMatrix)
    {
        for (int j = yLogicalSize - 1; j >= 0; j--)
        {
            string debugOutput = "";
            for (int i = 0; i < xLogicalSize; i++)
            {
                debugOutput += debugMatrix[i, j].ToString() + " ";
            }
            Debug.Log(debugOutput);
        }
    }
    void PositionalMatrixDebugLog<T>(T[,] debugMatrix)
    {
        Vector2Int positionalV;
        for (int j = yLogicalSize - 1; j >= 0; j--)
        {
            string debugOutput = "";
            for (int i = 0; i < xLogicalSize; i++)
            {
                positionalV = PCTLCMM[i, j];
                debugOutput += debugMatrix[positionalV.x, positionalV.y].ToString() + " ";
            }
            Debug.Log(debugOutput);
        }
    }


    //Positional coordinate To Logical coordinate
    Vector2Int PVTLV(Vector2Int positionalVector)
    {
        // return it's coordinate in PCTLCMM
        return PCTLCMM[positionalVector.x, positionalVector.y];
    }

    void PositionalCoordinateSelector(int xDirection, int yDirection)
    {
        positionalSelection.x += xDirection;
        positionalSelection.y += yDirection;
        positionalSelection.x = Mod(positionalSelection.x, xLogicalSize);
        positionalSelection.y = Mod(positionalSelection.y, yLogicalSize-1);
        //Debug.Log(playerID + "positionalSelection: " + positionalSelection.ToString());

        logicalSelection = PVTLV(positionalSelection);

    }

    private void CheckAxisReleased()
    {
        if (horizontalInput == 0f)
        {
            xAxisReleased = true;
        }
        else
        {
            xAxisReleased = false;
        }
        if (verticalInput == 0f)
        {
            yAxisReleased = true;
        }
        else
        {
            yAxisReleased = false;
        }

        if (xAxisReleased && yAxisReleased)
        {
            axisReleased = true;
        }
        else
        {
            axisReleased = false;
        }
    }

    //private void MRNARenderer()
    //{
    //    for (int i_reverse = 0; i_reverse < xLogicalSize; i_reverse++)
    //    {

    //    }
    //}

    private void MatrixRenderer(bool matrixScrollDebug)
    {
        for (int i = 0; i < xLogicalSize; i++)
        {
            for (int j = 0; j < yLogicalSize; j++)
            {
                if (objectTypeMatrix[i, j] >= 1) // is codon
                {
                    gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().sprite = codonPrefab.GetComponent<SpriteRenderer>().sprite;

                    gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().color = objectTypeColorArray[objectTypeMatrix[i,j]];
                }
                else // is block
                {
                    gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().sprite = blockPrefab.GetComponent<SpriteRenderer>().sprite;
                    if (objectTypeMatrix[i, j] == 0) // block
                    {

                        gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().color = matrixScrollDebug? blockColorMatrix[i, j] : blockColor;

                    }
                    else if (objectTypeMatrix[i, j] == -1) // empty
                    {
                        gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().color = objectTypeColorArray[codonNumber + 1];
                    }
                }

                if (i == logicalSelection.x && j == logicalSelection.y)
                {
                    gameObjectMatrix[i, j].GetComponent<SpriteRenderer>().color = isSelectionMode ? selectionColor : scrollingColor;
                }
            }
        }
    }

    void CodonGravity()
    {
        Vector2Int logicalV;
        Vector2Int logicalVDown;
        for (int pos_i = 0; pos_i < xLogicalSize; pos_i++)
        {
            for (int pos_j = 1; pos_j < yLogicalSize; pos_j++)
            {
                logicalV = PCTLCMM[pos_i, pos_j];

                logicalVDown = PCTLCMM[pos_i, pos_j - 1];
                Debug.Log(objectTypeMatrix[logicalV.x, logicalV.y]);
                if ( // 如果自己是codon且下面是empty block
                    objectTypeMatrix[logicalV.x, logicalV.y] != 0 &&
                    objectTypeMatrix[logicalVDown.x, logicalVDown.y] == -1
                    )
                {
                    objectTypeMatrix[logicalVDown.x, logicalVDown.y] =
                        objectTypeMatrix[logicalV.x, logicalV.y];
                    objectTypeMatrix[logicalV.x, logicalV.y] = -1;
                }

            }

        }
    }

    private void ScrollInRow(int direction)
    {
        int j = positionalSelection.y; //fix y value
        tempPCTLCMM = (Vector2Int[,])PCTLCMM.Clone();

        for (int i = 0; i < xLogicalSize; i++)
        {
            GameObject square =
                gameObjectMatrix[PCTLCMM[i, j].x, PCTLCMM[i, j].y];
            //GameObject codon = 
            //    codonArray[PCTLCMM[pos_i, pos_j].x, PCTLCMM[pos_i, pos_j].y];

            square.transform.position += new Vector3(direction * gridSize, 0, 0);
            //codon.transform.position += new Vector3(direction * gridSize, 0, 0);

            PCTLCMM[i, j] = tempPCTLCMM[Mod(i - direction, xLogicalSize), j];

            if (square.transform.position.x <
                transform.position.x + lowerBoundary.x)
            {
                square.transform.position +=
                    new Vector3(xLogicalSize * gridSize, 0, 0);
            }
            else if (square.transform.position.x >
                transform.position.x + upperBoundary.x)
            {
                square.transform.position +=
                    new Vector3(-xLogicalSize * gridSize, 0, 0);
            }

            //if (codon.transform.position.x < transform.position.x + lowerBoundary.x)
            //{
            //    codon.transform.position +=
            //        new Vector3(xLogicalSize * gridSize, 0, 0);
            //}
            //else if (codon.transform.position.x >
            //    transform.position.x + upperBoundary.x)
            //{
            //    codon.transform.position +=
            //        new Vector3(-xLogicalSize * gridSize, 0, 0);
            //}

        }
        positionalSelection += new Vector2Int(direction, 0);
        positionalSelection.x = Mod(positionalSelection.x, xLogicalSize);
    }


    private void ScrollInColumn(int direction)
    {
        int i = positionalSelection.x;// fix x value
        tempPCTLCMM = (Vector2Int[,])PCTLCMM.Clone();

        for (int j = 0; j < yLogicalSize; j++) //不许滚codon
            if (objectTypeMatrix[PCTLCMM[i, j].x, PCTLCMM[i, j].y] > 0)
                return;

        for (int j = 0; j < yLogicalSize; j++)
        {
            GameObject square =
                gameObjectMatrix[PCTLCMM[i, j].x, PCTLCMM[i, j].y];

            square.transform.position +=
                new Vector3(0, direction * gridSize, 0);

            PCTLCMM[i, j] = tempPCTLCMM[i, Mod(j - direction, yLogicalSize)];

            if (square.transform.position.y <
                transform.position.y + lowerBoundary.y)
            {
                square.transform.position +=
                    new Vector3(0, yLogicalSize * gridSize, 0);
            }
            else if (square.transform.position.y > transform.position.y + upperBoundary.y)
            {
                square.transform.position +=
                    new Vector3(0, -yLogicalSize * gridSize, 0);
            }

        }
        positionalSelection += new Vector2Int(0, direction);
        positionalSelection.y = Mod(positionalSelection.y, yLogicalSize);
    }
}

