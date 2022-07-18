using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private static Dictionary<int, IHasID> TileMap = new Dictionary<int, IHasID>();
    private static Dictionary<Player, int> RegisteredPlayers = new Dictionary<Player, int>();
    public static Dictionary<Type, ToolSelector> ToolsPerType = new Dictionary<Type, ToolSelector>();
    private static Dictionary<int, TextAsset> Levels = new Dictionary<int, TextAsset>();
    private static Game manager;
    [SerializeField]
    private Text ResultsText;
    [SerializeField]
    private Text LevelUnlockedText;
    [SerializeField]
    private GameObject spawnTileSelector;
    [SerializeField]
    private GameObject characterCheck;
    public static GameObject CharacterCheck => manager.characterCheck;

    [SerializeField]
    private AudioSource primaryAudio;
    public static AudioSource PrimaryAudio => manager.primaryAudio;

    public static GameObject SpawnTileSelector => manager.spawnTileSelector;

    public static event Action GameStartEvent;
    public static bool GameStarted { get; private set; } = false;
    public static bool ZoomedOnGame { get; private set; } = false;

    //Game Properties
    public static float TilePosition => CameraTarget.PlaneDistance * 5f;


    public static Target EnemyTarget { get; private set; }
    public static Target PlayerTarget { get; private set; }
    public static Button StartGameButton { get; private set; }
    public static Trail Trail { get; private set; }
    public static GameObject SidePanel { get; private set; }
    public static GameObject ToolContainer { get; private set; }
    public static Button ToolSelector { get; private set; }
    public static TutorialArea Tutorial { get; private set; }


    public static int LevelNumber { get; private set; } = 0;

    private static bool MapSet = false;
    public static int Width { get; private set; } = 0;
    public static int Height { get; private set; } = 0;

    private static Tile[,] TileBoard;
    public static Component[,] GameMap;

    public static Component GetGameTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        return GameMap[x, y];
    }

    public static Component GetGameTile(Vector2Int position)
    {
        /*if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        return GameMap[x, y];*/
        return GetGameTile(position.x, position.y);
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

    public static bool WithinBounds(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= Width || y >= Height);
    }

    private static GameObject LevelArea;

    //public static ReadOnlyCollection<GameObject> Tiles => manager.TilePrefabs.AsReadOnly();
    //public static ReadOnlyCollection<GameObject> Tiles => 

    private static T GetPrefab<T>(string prefabName) where T : UnityEngine.Object
    {
        return (Resources.Load(prefabName) as GameObject).GetComponent<T>();
    }

    private void Start()
    {
        manager = this;
        //Pane.Disable();
        Pane.GetPane("Main Menu").gameObject.SetActive(true);
        Pane.GetPane("Game").gameObject.SetActive(false);
        /*playerTarget = GetPrefab<GameObject>("Player Target.prefab");
        
        enemyTarget = GetPrefab<GameObject>("Enemy Target.prefab");
        trailPrefab = Resources.Load<GameObject>("Characters/Other/Trail.prefab");*/
        //trailPrefab = (Resources.Load("Characters/Other/Trail") as GameObject).GetComponent<Trail>();
        StartGameButton = GameObject.FindGameObjectWithTag("StartGameButton").GetComponent<Button>();
        //StartGameButton.transform.
        //S
        SidePanel = GameObject.FindGameObjectWithTag("SidePanel");
        SidePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(105, 50);
        ToolContainer = GameObject.FindGameObjectWithTag("ToolContainer");
        Tutorial = GameObject.FindGameObjectWithTag("TutorialArea").GetComponent<TutorialArea>();
        Tutorial.Start();
        Tutorial.transform.parent.gameObject.SetActive(false);
        Pane.GetPane("Tutorial Screen").gameObject.SetActive(false);
        StartGameButton.transform.parent.gameObject.SetActive(false);
        StartGameButton.gameObject.SetActive(false);
        Trail = GetPrefab<Trail>("Characters/Other/Trail");
        PlayerTarget = GetPrefab<Target>("Prefabs/Player Target");
        EnemyTarget = GetPrefab<Target>("Prefabs/Enemy Target");
        ToolSelector = GetPrefab<Button>("Prefabs/Tool Selector");

        Debug.Log("Player Target " + PlayerTarget == null);
        Debug.Log("Enemy Target " + EnemyTarget == null);
        Debug.Log("Trail " + Trail == null);
        //StartGameButton.transform.parent
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");
        foreach (GameObject prefab in prefabs)
        {
            IHasID ID = prefab.GetComponent<IHasID>();
            if (ID != null)
            {
                TileMap.Add(ID.GetTileID(), ID);
                Player DummyPlayer = GetDummy(ID.GetType());
                if (ID is Player player)
                {
                    RegisteredPlayers.Add(player, DummyPlayer.GetStartingAmount());
                }
            }
        }
        TextAsset[] levelAssets = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset level in levelAssets)
        {
            Match reg = Regex.Match(level.name, @"Level\s(\d+)");
            /*Debug.Log("Groups = " + reg.Groups.Count);
            Debug.Log("Captures = " + reg.Captures.Count);
            Debug.Log("The Capture = " + reg.Groups[1]);*/
            if (reg.Captures.Count > 0 && int.TryParse(reg.Groups[1].Value, out int result))
            {
                //Debug.Log("Adding");
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

    public static IEnumerator LoadLevel(int levelNumber)
    {
        ZoomedOnGame = false;
        SetMap(levelNumber);
        LevelNumber = levelNumber;
        SidePanel.SetActive(false);
        //Task away = Pane.GetPane("Select Level").Move(Pane.Fade.Out, Pane.Direction.Towards,3f);
        //Task to = Pane.GetPane("Pre Game").Move(Pane.Fade.In,Pane.Direction.Towards,1f/2f,TilePosition - 10f);
        //Task camera = CameraTarget.Move(CameraTarget.Position, new Vector3((Width - 1) / 2f, (Height - 1) / 2f, TilePosition - 10f),1f/2f);
        StartGameButton.gameObject.SetActive(false);
        Coroutine panelSwitch = GlobalRoutine.Start(Pane.SwitchTo("Select Level", "Pre Game", 1f / 3f, 4.5f, false));
        Coroutine camMove = GlobalRoutine.Start(CameraTarget.Move(CameraTarget.Position, new Vector3(Width / 2f, Height / 2f, CameraTarget.PlaneDistance * 4.5f), 1f / 3f));

        yield return panelSwitch;
        yield return camMove;
        ZoomedOnGame = true;

        CameraTarget.Active = true;
        //CameraTarget.Movable = true;
        CameraTarget.MinZ = TilePosition - 20f;
        CameraTarget.MaxZ = TilePosition - 5f;
        CameraTarget.Zooming = true;
        float Offset = 5f;
        CameraTarget.Boundaries = new Rect(-Offset, -Offset, Width + 2 * Offset, Height + 2 * Offset);

        if (levelNumber == 1)
        {
            yield return TutorialRoutine.Tutorial();
            //GC.KeepAlive(Task);
            //Task.Run(() => TutorialRoutine.Tutorial().Wait());
        }
        /*Pane.GetPane("Select Level").gameObject.SetActive(false);
        CameraTarget.Active = true;
        CameraTarget.Movable = true;
        CameraTarget.WarpCamera(new Vector3((Width - 1) / 2f,(Height - 1) / 2f));*/
    }

    private static T GetDummy<T>() where T : class
    {
        return FormatterServices.GetUninitializedObject(typeof(T)) as T;
    }

    private static Player GetDummy(Type T)
    {
        return FormatterServices.GetUninitializedObject(T) as Player;
    }

    private static void SetMap(int levelNumber)
    {
        try
        {
            DeleteMap();
            Debug.Log("Index = " + levelNumber);
            foreach (KeyValuePair<int, TextAsset> level in Levels)
            {
                Debug.Log(level.Key.ToString() + " = " + level.Value);
            }
            foreach (KeyValuePair<int, IHasID> id in TileMap)
            {
                Debug.Log(id.Value.GetType().ToString() + " = " + id.Key);
            }
            for (int i = 0; i < ToolContainer.transform.childCount; i++)
            {
                GameObject.Destroy(ToolContainer.transform.GetChild(i).gameObject);
            }
            ToolsPerType.Clear();
            foreach (KeyValuePair<Player, int> pair in RegisteredPlayers)
            {
                Player player = pair.Key;
                int Amount = pair.Value;
                ToolSelector NewSelector = GameObject.Instantiate(ToolSelector.gameObject).GetComponent<ToolSelector>();
                NewSelector.Start();
                NewSelector.transform.SetParent(ToolContainer.transform, false);
                NewSelector.SetPlayer = player;
                ToolsPerType.Add(player.GetType(), NewSelector);
                NewSelector.CurrentAmount = Amount;
                NewSelector.Image = player.GetComponent<SpriteRenderer>().sprite.texture;
                /* NewSelector.transform.SetParent(ToolContainer.transform,false);
                 NewSelector.GetComponent<RawImage>().texture = player.GetComponent<SpriteRenderer>().sprite.texture;
                 Player ButtonPlayer = player;
                 NewSelector.GetComponent<Button>().onClick.AddListener(() => {
                     //CONTROLS FOR TOOL SELECTORS
                     if (global::SpawnTile.SelectedTile != null)
                     {
                         CharacterSelector.Selection = SpawnCharacter(ButtonPlayer.GetType(), CharacterSelector.SpawnPosition) as Player;
                         CharacterSelector.Cancel();
                     }
                 });*/
            }
            TextAsset levelFile = Levels[levelNumber];
            JObject json = JObject.Parse(levelFile.text);
            MapSet = true;
            Width = json["width"].ToObject<int>();
            Height = json["height"].ToObject<int>();
            int[] Data = json["layers"][0]["data"].ToObject<int[]>();
            //CDebug.Log($"Data = {Data}");
            TileBoard = new Tile[Width, Height];
            GameMap = new Component[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int IDNum = Data[x + (y * Width)] - 1;
                    if (IDNum >= 0)
                    {
                        IHasID obj = TileMap[IDNum];
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
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    public static IEnumerator ResetToSelectionScreen()
    {
        CameraTarget.Active = true;
        Game.DeleteMap();
        Coroutine panelSwitch = GlobalRoutine.Start(Pane.SwitchBackTo("Results Screen", "Main Menu", 1f / 3f, 4.5f, false));
        Coroutine cameraMove = GlobalRoutine.Start(CameraTarget.Move(CameraTarget.Position, CameraTarget.BasePosition, 1f / 3f));
        yield return panelSwitch;
        yield return cameraMove;
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
            foreach (Tile tile in TileBoard)
            {
                if (tile != null)
                {
                    DestroyTile(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y));
                }
            }
            Width = 0;
            Height = 0;
            TileBoard = new Tile[0, 0];
            GameMap = new Component[0, 0];
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
            Character character = GameObject.Instantiate(Prefab.gameObject).GetComponent<Character>();
            character.transform.position = new Vector3(position.x, position.y, TilePosition);
            character.transform.SetParent(manager.transform);
            character.gameObject.layer = 9;
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
        if (!(position.x < 0 || position.y < 0 || position.x >= Width || position.y >= Height) && TileBoard[position.x, position.y] != null)
        {
            Tile Tile = TileBoard[position.x, position.y];
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
            Tile tile = GameObject.Instantiate(Prefab.gameObject).GetComponent<Tile>();
            tile.transform.position = new Vector3(position.x, position.y, TilePosition);
            tile.gameObject.layer = 9;
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

    public static Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }
        return TileBoard[x, y];
    }

    public static bool TutorialEnemyWait = true;
    public static bool TutorialPlayerWait = false;

    public static IEnumerator StartGame()
    {
        if (!GameStarted)
        {
            Pane.GetPane("Pre Game").gameObject.SetActive(false);
            Pane.GetPane("Game").gameObject.SetActive(true);
            GameStarted = true;
            GameStartEvent?.Invoke();
            while (true)
            {
                yield return SwitchDisplay("Players Turn", new Color32(0, 95, 255, 255));
                if (TutorialRoutine.TutorialActive)
                {
                    Player.PlayersTurn = true;
                    yield return new WaitUntil(() => !TutorialPlayerWait);
                }
                yield return Player.BeginTurns();
                if (Enemy.Enemies.Count == 0)
                {
                    yield return Win();
                    break;
                }
                yield return SwitchDisplay("Enemies Turn", new Color32(255, 0, 0, 255));
                if (TutorialRoutine.TutorialActive)
                {
                    Enemy.EnemyTurn = true;
                    yield return new WaitUntil(() => !TutorialEnemyWait);
                }
                yield return Enemy.BeginTurns();
                if (Player.Players.Count == 0)
                {
                    yield return Lose();
                    break;
                }
            }
        }
    }

    private static IEnumerator SwitchDisplay(string text, Color color)
    {
        Vector2 EndVector = new Vector2(95.61502f, -186.2f);
        Vector2 BeginVector = new Vector2(-95.86499f, -186.2f);
        yield return Dragger.Drag(TurnDisplay.Instance.gameObject, null, BeginVector, 3f, EndIfEqual: true);
        TurnDisplay.SetText(text);
        TurnDisplay.Color = color;
        yield return Dragger.Drag(TurnDisplay.Instance.gameObject, null, EndVector, 3f);
    }

    public static void StopGame()
    {
        GameStarted = false;
        GameStartEvent = null;
        CameraTarget.Movable = false;
        CameraTarget.Zooming = false;
        Pane.GetPane("Game").gameObject.SetActive(false);
        foreach (Player player in Player.Players)
        {
            player.ResetTurn();
        }
        foreach (Enemy enemy in Enemy.Enemies)
        {
            enemy.ResetTurn();
        }
    }

    public static bool WaitWin = false;

    private static IEnumerator Win()
    {
        if (WaitWin)
        {
            yield return new WaitUntil(() => !WaitWin);
        }
        StopGame();
        yield return new WaitForSeconds(1f);
        Game.PrimaryAudio.PlayOneShot(Sounds.WinSound);
        if (LevelManager.UnlockLevel(LevelNumber + 1))
        {
            manager.LevelUnlockedText.gameObject.SetActive(true);
        }
        manager.ResultsText.text = "You Win";
        Pane.GetPane("Results Screen").Enable();
    }
    private static IEnumerator Lose()
    {
        StopGame();
        yield return new WaitForSeconds(1f);
        Game.PrimaryAudio.PlayOneShot(Sounds.LoseSound);
        manager.ResultsText.text = "You Lose";
        manager.LevelUnlockedText.gameObject.SetActive(false);
        Pane.GetPane("Results Screen").Enable();
    }
}
