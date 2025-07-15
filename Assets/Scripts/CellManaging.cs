using UnityEngine;

public class CellManaging : MonoBehaviour
{
    public static int mapSizeX = 300, mapSizeY = 300;

    public int[,] cellStates = new int[mapSizeX, mapSizeY];
    private int[,] startingCellStates = new int[mapSizeX, mapSizeY];
    public string[] rules;

    public Texture2D texture;
    public Renderer meshRenderer;

    public int spawnRarity1 = 10;
    public int spawnRarity2 = 0;
    public int spawnRarity3 = 0;
    public int spawnRarity4 = 0;
    public double updateInterval = 1d;
    private float timer = 0f;
    private bool paused = true;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        texture = new Texture2D(mapSizeX, mapSizeY);
        texture.filterMode = FilterMode.Point; // Ensures pixel-perfect rendering

        // Initialize rules
        rules = new string[] {
        "010",
        "000",
        "110",
        "100",
        "211",
        "200",
        "311",
        "301",
        "410",
        "400",
        "510",
        "500",
        "610",
        "600",
        "710",
        "700",
        "810",
        "800"
        };

        // Randomize initial state
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                int state = 0;
                if (spawnRarity1 > 0 && Random.Range(0, spawnRarity1) == 0) state = 1;
                else if (spawnRarity2 > 0 && Random.Range(0, spawnRarity2) == 0) state = 2;
                else if (spawnRarity3 > 0 && Random.Range(0, spawnRarity3) == 0) state = 3;
                else if (spawnRarity4 > 0 && Random.Range(0, spawnRarity4) == 0) state = 4;

                startingCellStates[x, y] = state;
                cellStates[x, y] = state;
            }
        }

        UpdateTexture();
    }

    void Update()
    {
        Draw();
        UpdateTexture();
        if (!paused)
        {
            timer += Time.deltaTime;
        }

        if (timer >= updateInterval)
        {
            timer = 0;

            int[,] newStates = new int[mapSizeX, mapSizeY];

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    int livingNeighbors = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0) continue;

                            int nx = x + i;
                            int ny = y + j;

                            if (nx >= 0 && nx < mapSizeX && ny >= 0 && ny < mapSizeY)
                            {
                                if (cellStates[nx, ny] == 1)
                                {
                                    livingNeighbors++;
                                }
                            }
                        }
                    }

                    int currentState = cellStates[x, y];

                    foreach (string rule in rules)
                    {
                        int ruleNeighbors = rule[0] - '0';
                        int ruleCurrent = rule[1] - '0';
                        int ruleNext = rule[2] - '0';

                        if (currentState == ruleCurrent && livingNeighbors == ruleNeighbors)
                        {
                            newStates[x, y] = ruleNext;
                            goto NextCell; // Skip further rules once matched
                        }
                    }

                    newStates[x, y] = currentState; // Default: retain current state

                NextCell:;
                }
            }
            cellStates = newStates;
        }
        GetInput();
    }

    void UpdateTexture()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (cellStates[x, y] == 1)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else if (cellStates[x, y] == 2)
                {
                    texture.SetPixel(x, y, Color.red);
                }
                else if (cellStates[x, y] == 3)
                {
                    texture.SetPixel(x, y, Color.blue);
                }
                else if (cellStates[x, y] == 4)
                {
                    texture.SetPixel(x, y, Color.yellow);
                }
                else
                {
                    texture.SetPixel(x, y, Color.black);
                }
            }
        }

        texture.Apply();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Randomize initial state
            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    int state = 0;
                    if (spawnRarity1 > 0 && Random.Range(0, spawnRarity1) == 0) state = 1;
                    else if (spawnRarity2 > 0 && Random.Range(0, spawnRarity2) == 0) state = 2;
                    else if (spawnRarity3 > 0 && Random.Range(0, spawnRarity3) == 0) state = 3;
                    else if (spawnRarity4 > 0 && Random.Range(0, spawnRarity4) == 0) state = 4;

                    startingCellStates[x, y] = state;
                    cellStates[x, y] = state;
                }
            }
            paused = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Randomize initial state
            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    cellStates[x, y] = startingCellStates[x, y];
                }
            }
            paused = false;
        }

        if (Input.GetKey(KeyCode.Period))
        {
            updateInterval += 0.01d;
        }
        else if (Input.GetKey(KeyCode.Comma))
        {
            updateInterval -= 0.01d;
            if (updateInterval < 0.0001f)
            {
                updateInterval = 0.0001f;
            }
        }
    }
    void Draw()
    {
        Vector2 coordinates;
        coordinates = mainCam.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y, 10));

        // shift the coordinates to have an origin in the bottom left
        coordinates.x -= 5;
        coordinates.y -= 5;

        // multiply make the x and y range from 0 to 300, just like the two-dimensional array storing the cell states
        coordinates.x *= -30;
        coordinates.y *= -30;

        if (Input.GetKey(KeyCode.Y))
        {
            cellStates[(int)coordinates.x, (int)coordinates.y] = 0;
        }
        else if (Input.GetKey(KeyCode.U))
        {
            cellStates[(int)coordinates.x, (int)coordinates.y] = 1;
        }
        else if (Input.GetKey(KeyCode.I))
        {
            cellStates[(int)coordinates.x, (int)coordinates.y] = 2;
        }
        else if (Input.GetKey(KeyCode.O))
        {
            cellStates[(int)coordinates.x, (int)coordinates.y] = 3;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            cellStates[(int)coordinates.x, (int)coordinates.y] = 4;
        }
        
    }
}

