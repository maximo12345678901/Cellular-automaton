using UnityEngine;
using System.Threading.Tasks;
public class CellManaging : MonoBehaviour
{
    public static int mapSizeX = 400, mapSizeY = 400;

    public int[,] cellStates = new int[mapSizeX, mapSizeY];
    private int[,] startingCellStates = new int[mapSizeX, mapSizeY];
    public string[] rules;

    public Texture2D texture;
    public Renderer meshRenderer;

    public int spawnRarity1 = 0;
    public int spawnRarity2 = 0;
    public int spawnRarity3 = 0;
    public int spawnRarity4 = 0;
    public int drawingRadius = 2;
    public int erasingRadius = 4;
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
        if (!paused)
        {
            timer += Time.deltaTime;
        }

        GetInput();

        if (timer >= updateInterval)
        {
            timer = 0;

            int[,] newStates = new int[mapSizeX, mapSizeY];
            Color[,] colorBuffer = new Color[mapSizeX, mapSizeY];

            Parallel.For(0, mapSizeX, x =>
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
                    int newState = currentState;

                    foreach (string rule in rules)
                    {
                        int ruleNeighbors = rule[0] - '0';
                        int ruleCurrent = rule[1] - '0';
                        int ruleNext = rule[2] - '0';

                        if (currentState == ruleCurrent && livingNeighbors == ruleNeighbors)
                        {
                            newState = ruleNext;
                            break;
                        }
                    }

                    newStates[x, y] = newState;

                    switch (newState)
                    {
                        case 1: colorBuffer[x, y] = Color.white; break;
                        case 2: colorBuffer[x, y] = Color.red; break;
                        case 3: colorBuffer[x, y] = Color.blue; break;
                        case 4: colorBuffer[x, y] = Color.yellow; break;
                        default: colorBuffer[x, y] = Color.black; break;
                    }
                }
            });

            // Now update cell states and apply colors on main thread
            cellStates = newStates;

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    texture.SetPixel(x, y, colorBuffer[x, y]);
                }
            }

            texture.Apply();
            meshRenderer.sharedMaterial.mainTexture = texture;
        }
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
        if (Input.GetKey(KeyCode.Y))
        {
            Draw(0, erasingRadius);
        }
        if (Input.GetKey(KeyCode.U))
        {
            Draw(1, drawingRadius);
        }
        if (Input.GetKey(KeyCode.I))
        {
            Draw(2, drawingRadius);
        }
        if (Input.GetKey(KeyCode.O))
        {
            Draw(3, drawingRadius);
        }
        if (Input.GetKey(KeyCode.P))
        {
            Draw(4, drawingRadius);
        }
        
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
    void Draw(int drawingCellState, int radius)
    {
        Vector2 coordinates;
        coordinates = mainCam.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y, 10));

        // shift the coordinates to have an origin in the bottom left
        coordinates.x -= 5;
        coordinates.y -= 5;

        // multiply make the x and y range from 0 to 300, just like the two-dimensional array storing the cell states
        coordinates.x *= -mapSizeX/ 10;
        coordinates.y *= -mapSizeY / 10;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                try
                {
                    cellStates[(int)coordinates.x + x, (int)coordinates.y + y] = drawingCellState;
                }
                catch { }
            }
        }
    }
}

