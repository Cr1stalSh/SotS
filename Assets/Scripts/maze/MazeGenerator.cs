using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeGenerator : MonoBehaviour
{
    public int rows = 35;
    public int cols = 35;

    public int spriteSize = 10; 

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;

    private int[,] maze;
    private const int WALL = 0;
    private const int PASS = 1;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[rows, cols];

        for (int y1 = 0; y1 < rows; y1++)
        {
            for (int x1 = 0; x1 < cols; x1++)
            {
                maze[y1, x1] = WALL;
            }
        }

        int x = 1, y = 1;
        maze[y, x] = PASS;

        for (int steps = 0; steps < 10000; steps++)
        {
            var directions = new (int dx, int dy)[]
            {
                (0, -2), // вверх
                (0, 2),  // вниз
                (-2, 0), // влево
                (2, 0)   // вправо
            };

            var validDirections = System.Array.FindAll(directions, dir =>
            {
                int nx = x + dir.dx;
                int ny = y + dir.dy;
                return nx > 0 && ny > 0 && nx < cols - 1 && ny < rows - 1 && maze[ny, nx] == WALL;
            });

            if (validDirections.Length == 0)
            {
                // Возвращаемся к случайной доступной клетке
                do
                {
                    x = UnityEngine.Random.Range(1, cols / 2) * 2 - 1;
                    y = UnityEngine.Random.Range(1, rows / 2) * 2 - 1;
                } while (maze[y, x] != PASS);

                continue;
            }

            var direction = validDirections[UnityEngine.Random.Range(0, validDirections.Length)];
            int nx = x + direction.dx;
            int ny = y + direction.dy;

            maze[y + direction.dy / 2, x + direction.dx / 2] = PASS;
            maze[ny, nx] = PASS;

            x = nx;
            y = ny;
        }
    }

    void DrawMaze()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3(x * spriteSize, -y * spriteSize, 0); 
                if (maze[y, x] == WALL)
                {
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
                }
            }
        }

        Instantiate(goalPrefab, new Vector3((cols - 2) * spriteSize, -(rows - 2) * spriteSize, 0), Quaternion.identity);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(1 * spriteSize, -1 * spriteSize, 0);
        }
        else
        {
            Debug.LogWarning("Player с тегом 'Player' не найден на сцене!");
        }
    }


}

