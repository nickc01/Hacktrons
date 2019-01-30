using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Linq;

public class TileManager : MonoBehaviour
{
    private static Dictionary<int, IHasID> TileMap = new Dictionary<int, IHasID>();
    private static Dictionary<int, TextAsset> Levels = new Dictionary<int, TextAsset>();
    private static TileManager manager;

    /*[SerializeField]
    private GameObject TrailPrefab;

    [SerializeField]
    public GameObject playerTarget;

    [SerializeField]
    public GameObject enemyTarget;*/
    private static Target playerTarget;
    private static Target enemyTarget;
    private static Trail trailPrefab;


    public static Target EnemyTarget => enemyTarget;
    public static Target PlayerTarget => playerTarget;

    public static Trail Trail => trailPrefab;

    static bool MapSet = false;
    public static int Width { get; private set; } = 0;
    public static int Height { get; private set; } = 0;
    static Tile[,] TileBoard;
    public static Component[,] GameMap;

    public static Component GetGameTile(int x,int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        //CDebug.Log($"Value at {x}, {y} = {GameMap[x, y]}");
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

    public static bool WithinBounds(int x,int y)
    {
        return !(x < 0 || y < 0 || x >= Width || y >= Height);
    }
    
    static GameObject LevelArea;

    //public static ReadOnlyCollection<GameObject> Tiles => manager.TilePrefabs.AsReadOnly();
    //public static ReadOnlyCollection<GameObject> Tiles => 


    void Start()
    {
        manager = this;
        var prefabs = Resources.LoadAll<GameObject>("");
        foreach (var prefab in prefabs)
        {
            IHasID ID = prefab.GetComponent<IHasID>();
            if (ID != null)
            {
                TileMap.Add(ID.GetTileID(), ID);
            }
        }
        var levelAssets = Resources.LoadAll<TextAsset>("Levels");
        foreach (var level in levelAssets)
        {
            var reg = Regex.Match(level.name, @"Level\s(\d+)");
            /*Debug.Log("Groups = " + reg.Groups.Count);
            Debug.Log("Captures = " + reg.Captures.Count);
            Debug.Log("The Capture = " + reg.Groups[1]);*/
            if (reg.Captures.Count > 0 && int.TryParse(reg.Groups[1].Value,out int result))
            {
                Debug.Log("Adding");
                Levels.Add(result, level);
            }
        }
        /*foreach (var tile in TilePrefabs)
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
        }*/
    }

    public static void LoadLevel(int levelNumber)
    {
        SetMap(levelNumber);
        Pane.GetPane("Select Level").gameObject.SetActive(false);
        CameraTarget.Active = true;
        CameraTarget.Movable = true;
        CameraTarget.WarpCamera(new Vector3((Width - 1) / 2f,(Height - 1) / 2f));
    }

    private static void SetMap(int levelNumber)
    {
        DeleteMap();
        Debug.Log("Index = " + levelNumber);
        foreach (var level in Levels)
        {
            Debug.Log(level.Key.ToString() + " = " + level.Value);
        }
        foreach (var id in TileMap)
        {
            Debug.Log(id.Value.GetType().ToString() + " = " + id.Key);
        }
        TextAsset levelFile = Levels[levelNumber];
        var json = JObject.Parse(levelFile.text);
        MapSet = true;
        Width = json["width"].ToObject<int>();
        Height = json["height"].ToObject<int>();
        var Data = json["layers"][0]["data"].ToObject<int[]>();
        //CDebug.Log($"Data = {Data}");
        TileBoard = new Tile[Width, Height];
        GameMap = new Component[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var IDNum = Data[x + (y * Width)] - 1;
                if (IDNum >= 0)
                {
                    var obj = TileMap[IDNum];
                    if (obj is Character character)
                    {
                        SpawnCharacter(character.GetType(), new Vector2Int(x, Height - 1 - y));
                        TileBoard[x, Height - 1 - y] = SpawnTile<BasicTile>(new Vector2Int(x, Height - 1 - y));
                    }
                    else if (obj is Tile tile)
                    {
                        TileBoard[x, Height - 1 - y] = SpawnTile(tile.GetType(), new Vector2Int(x, Height - 1 - y));
                    }
                }
            }
        }
    }
    public static void DeleteMap()
    {
        if (MapSet)
        {
            MapSet = false;
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
            foreach (var tile in TileBoard)
            {
                if (tile != null)
                {
                    DestroyTile(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y));
                }
            }
            Width = 0;
            Height = 0;
            TileBoard = new Tile[0,0];
            GameMap = new Component[0,0];
        }
    }
    public static int GetLevelCount()
    {
        return Levels.Count;
    }

    public static T SpawnCharacter<T>(Vector2Int position) where T : Character
    {
        return SpawnCharacter(typeof(T), position) as T;
    }
    public static Character SpawnCharacter(Type T, Vector2Int position)
    {
        if (FormatterServices.GetUninitializedObject(T) is IHasID id && TileMap[id.GetTileID()] is Character Prefab)
        {
            var character = GameObject.Instantiate(Prefab.gameObject).GetComponent<Character>();
            character.transform.position = new Vector3(position.x, position.y, Prefab.transform.position.z);
            character.transform.SetParent(manager.transform);
            character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
            character.OnSpawn();
            return character;
        }
        else
        {
            throw new Exception(T.ToString() + " is not a character");
        }
    }
    public static T SpawnTile<T>(Vector2Int position) where T : Tile
    {
        return SpawnTile(typeof(T), position) as T;
    }
    public static void DestroyTile(Vector2Int position)
    {
        if (!(position.x < 0 || position.y < 0 || position.x >= Width || position.y >= Height) && TileBoard[position.x,position.y] != null)
        {
            var Tile = TileBoard[position.x,position.y];
            Tile.OnDestruction();
            GameObject.Destroy(Tile.gameObject);
            TileBoard[position.x, position.y] = null;
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
        if (FormatterServices.GetUninitializedObject(T) is IHasID id && TileMap[id.GetTileID()] is Tile Prefab)
        {
            var tile = GameObject.Instantiate(Prefab.gameObject).GetComponent<Tile>();
            tile.transform.position = new Vector3(position.x, position.y);
            TileBoard[position.x, position.y] = tile;
            tile.transform.SetParent(manager.transform);
            tile.OnSpawn();
            return tile;
        }
        else
        {
            throw new Exception(T.ToString() + " Is not a tile");
        }
    }

    public static Tile GetTile(int x,int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        return TileBoard[x, y];
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
