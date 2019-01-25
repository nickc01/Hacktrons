using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System;
using System.Reflection;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> TilePrefabs;
    [SerializeField]
    private List<TextAsset> LevelFiles;
    //private static Dictionary<Type, object> TileTypes = new Dictionary<Type, object>();
    private static Dictionary<Type, Character> CharacterList = new Dictionary<Type, Character>();
    private static Dictionary<Type, Tile> TileList = new Dictionary<Type, Tile>();
    private static TileManager manager;

    [SerializeField]
    private GameObject TrailPrefab;

    [SerializeField]
    public GameObject playerTarget;

    [SerializeField]
    public GameObject enemyTarget;

    public static GameObject EnemyTarget => manager.enemyTarget;
    public static GameObject PlayerTarget => manager.playerTarget;

    public static Trail Trail => manager.TrailPrefab.GetComponent<Trail>();

    static bool MapSet = false;
    public static int Width { get; private set; } = 0;
    public static int Height { get; private set; } = 0;
    static Tile[,] TileMap;
    public static Component[,] GameMap;

    public static Component GetGameTile(int x,int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        CDebug.Log($"Value at {x}, {y} = {GameMap[x, y]}");
        return GameMap[x, y];
    }

    public static bool HasGameTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return false;
        }
        //Print();
        return GameMap[x, y] != null;
    }

    /*public static void Print()
    {
        string final = "";
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var value = GameMap[x, y];
                if (value == null)
                {
                    final += "null, ";
                }
                else
                {
                    final += (GameMap[x, y].ToString()).ToString() + ", ";
    }
            }
        }
        CDebug.Log(final);
    }*/

    public static bool WithinBounds(int x,int y)
    {
        return !(x < 0 || y < 0 || x >= Width || y >= Height);
    }
    
    static GameObject LevelArea;

    public static ReadOnlyCollection<GameObject> Tiles => manager.TilePrefabs.AsReadOnly();

    void Start()
    {
        manager = this;
        foreach (var tile in TilePrefabs)
        {
            if (tile.GetComponent<Character>() != null)
            {
                var Component = tile.GetComponent<Character>();
                CDebug.Log($"Type = {Component.GetType()}");
                CharacterList.Add(Component.GetType(), Component);
            }
            else
            {
                var Component = tile.GetComponent<Tile>();
                CDebug.Log($"Type2 = {Component.GetType()}");
                TileList.Add(Component.GetType(), Component);
            }
        }
    }

    public static void LoadLevel(int levelNumber)
    {
        SetMap(levelNumber - 1);
        Pane.GetPane("Select Level").gameObject.SetActive(false);
        CameraTarget.Active = true;
        CameraTarget.Movable = true;
        CameraTarget.WarpCamera(new Vector3((Width - 1) / 2f,(Height - 1) / 2f));
    }

    private static void SetMap(int levelIndex)
    {
        DeleteMap();
        TextAsset levelFile = manager.LevelFiles[levelIndex];
        var json = JObject.Parse(levelFile.text);
        MapSet = true;
        Width = json["width"].ToObject<int>();
        Height = json["height"].ToObject<int>();
        var Data = json["layers"][0]["data"].ToObject<int[]>();
        CDebug.Log($"Data = {Data}");
        TileMap = new Tile[Width, Height];
        GameMap = new Component[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var obj = IndexToObject(Data[x + (y * Width)] - 1);
                if (obj is Character character)
                {
                    SpawnCharacter(character.GetType(), new Vector2Int(x, Height - 1 - y));
                    TileMap[x, Height - 1 - y] = SpawnTile<BasicTile>(new Vector2Int(x, Height - 1 - y));
                }
                else if (obj is Tile tile)
                {
                    TileMap[x,Height - 1 - y] = SpawnTile(tile.GetType(), new Vector2Int(x, Height - 1 - y));
                }
            }
        }
    }
    public static void DeleteMap()
    {
        if (MapSet)
        {
            MapSet = false;
            /*foreach (var player in Player.Players)
            {
                TileManager.DestroyCharacter(player);
            }
            foreach (var enemy in Enemy.Enemies)
            {
                TileManager.DestroyCharacter(enemy);
            }*/
            /*for (int i = 0; i < Enemy.Enemies.Count; i++)
            {
                TileManager.DestroyCharacter(Enemy.Enemies[i]);
            }
            for (int i = 0; i < Player.Players.Count; i++)
            {
                TileManager.DestroyCharacter(Player.Players[i]);
            }*/
            for (int i = Enemy.Enemies.Count - 1; i >= 0; i--)
            {
                Enemy.Enemies[i].DeleteAllTrails();
                DestroyCharacter(Enemy.Enemies[i]);
            }
            for (int i = Player.Players.Count - 1; i >= 0; i--)
            {
                Player.Players[i].DeleteAllTrails();
                DestroyCharacter(Player.Players[i]);
            }
            foreach (var tile in TileMap)
            {
                if (tile != null)
                {
                    DestroyTile(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y));
                }
            }
            Width = 0;
            Height = 0;
            TileMap = new Tile[0,0];
            GameMap = new Component[0,0];
        }
    }
    public static int GetLevelCount()
    {
        return manager.LevelFiles.Count;
    }
    //Returns either a character or a tile based on the index
    private static object IndexToObject(int index)
    {
        if (index < 0 || index >= manager.TilePrefabs.Count)
        {
            return null;
        }
        var Object = manager.TilePrefabs[index];
        if (Object.GetComponent<Character>() != null)
        {
            return Object.GetComponent<Character>();
        }
        else
        {
            return Object.GetComponent<Tile>();
        }
    }

    public static T SpawnCharacter<T>(Vector2Int position) where T : Character
    {
        return SpawnCharacter(typeof(T), position) as T;
    }
    public static Character SpawnCharacter(Type T, Vector2Int position)
    {
        var Prefab = CharacterList[T] as Character;
        var character = GameObject.Instantiate(Prefab.gameObject).GetComponent<Character>();
        character.transform.position = new Vector3(position.x,position.y,Prefab.transform.position.z);
        character.transform.SetParent(manager.transform);
        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        character.OnSpawn();
        return character;
    }
    public static T SpawnTile<T>(Vector2Int position) where T : Tile
    {
        return SpawnTile(typeof(T), position) as T;
    }
    public static void DestroyTile(Vector2Int position)
    {
        if (!(position.x < 0 || position.y < 0 || position.x >= Width || position.y >= Height) && TileMap[position.x,position.y] != null)
        {
            var Tile = TileMap[position.x,position.y];
            Tile.OnDestruction();
            GameObject.Destroy(Tile.gameObject);
            TileMap[position.x, position.y] = null;
        }
    }
    public static void DestroyCharacter(Character character)
    {
        character.OnDestruction();
        GameObject.Destroy(character.gameObject);
    }
    public static Tile SpawnTile(Type T, Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= Width || position.y >= Height)
        {
            throw new Exception("The Position is not within the map boundaries");
        }
        var Prefab = TileList[T] as Tile;
        var tile = GameObject.Instantiate(Prefab.gameObject).GetComponent<Tile>();
        tile.transform.position = new Vector3(position.x, position.y);
        TileMap[position.x, position.y] = tile;
        tile.transform.SetParent(manager.transform);
        tile.OnSpawn();
        return tile;
    }

    public static Tile GetTile(int x,int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        return TileMap[x, y];
    }



    //Spawns a new character
    /*public static T Spawn<T>() where T : Character
    {
        var Prefab = TileTypes[typeof(T)] as Character;
        var character = GameObject.Instantiate(Prefab.gameObject).GetComponent<Character>();
        character.transform.position = new Vector3(9999,9999);
        character.transform.SetParent(manager.transform);
        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        character.OnSpawn();
        return character as T;
    }
    public static Character Spawn(Type T)
    {
        var Prefab = TileTypes[T] as Character;
        var character = GameObject.Instantiate(Prefab.gameObject).GetComponent<Character>();
        character.transform.position = new Vector3(9999, 9999);
        character.transform.SetParent(manager.transform);
        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        character.OnSpawn();
        return character;
    }*/
}
