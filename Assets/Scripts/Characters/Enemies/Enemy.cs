using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using System.Linq;
using System;

public abstract class Enemy : Character
{
    private static List<Enemy> enemies = new List<Enemy>();
    public static ReadOnlyCollection<Enemy> Enemies => enemies.AsReadOnly();

    public static Enemy ActiveEnemy => ActiveCharacter as Enemy;

    private static List<Enemy> ActiveEnemiesLeft = new List<Enemy>();

    public static bool EnemyTurn { get; set; } = false;

    public override void OnDestruction()
    {
        base.OnDestruction();
        enemies.Remove(this);
        ActiveEnemiesLeft.Remove(this);
    }

    public sealed override void OnSpawn()
    {
        base.OnSpawn();
        enemies.Add(this);
    }

    public override void Select()
    {
        if (EnemyTurn)
        {
            base.Select();
            CharacterStats.SetCharacter(this);
        }
        else
        {
            if (Character.ActiveCharacter != null)
            {
                Character.ActiveCharacter.Deselect();
            }
            CharacterStats.SetCharacter(this);
        }
    }

    public override void Deselect()
    {
        if (EnemyTurn)
        {
            base.Deselect();
            CharacterStats.SetCharacter(null);
        }
    }

    protected override void TurnEnd()
    {
        // throw new System.NotImplementedException();
        ActiveEnemiesLeft.Remove(this);
        if (ActiveEnemiesLeft.Count > 0)
        {
            ActiveEnemiesLeft[0].Select();
        }
    }

    protected override void TurnPostpone()
    {
       // throw new System.NotImplementedException();
    }

    protected override async Task<(Character, Component)> RequestToAttack()
    {
        var (FoundPlayer, FoundTrail) = FindNearestPlayer();
        if (FoundPlayer == null)
        {
            return (null, null);
        }
        List<Target> Targets = new List<Target>();
        var EnemyTargetPrefab = Game.EnemyTarget.GetComponent<Target>();

        for (int x = 0; x < Game.Width; x++)
        {
            for (int y = 0; y < Game.Height; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), transform.position) <= AttackRange && Game.GetGameTile(x, y) != this)
                {
                    var NewTarget = GameObject.Instantiate(EnemyTargetPrefab.gameObject).GetComponent<Target>();
                    NewTarget.transform.position = new Vector3(x, y, transform.position.z);
                    Targets.Add(NewTarget);
                    var GameTile = Game.GameMap[x, y];
                    if (GameTile != null)
                    {
                        if (GameTile is Trail trail && trail.Host is Player)
                        {
                            continue;
                        }
                        if (GameTile is Player)
                        {
                            continue;
                        }
                    }
                    var renderer = NewTarget.GetComponent<SpriteRenderer>();
                    renderer.color = new Color(renderer.color.r,renderer.color.g,renderer.color.b,0.3f);
                }
            }
        }
        await Tasker.Run(() => Thread.Sleep(1000));
        foreach (var target in Targets)
        {
            GameObject.Destroy(target.gameObject);
        }
        if (Vector2.Distance(FoundTrail.transform.position,transform.position) <= AttackRange)
        {
            return (FoundPlayer, FoundTrail);
        }
        else
        {
            return (null, null);
        }
    }

    /*protected override async Task<(Character, Component)> RequestToAttack()
    {
        if (RequestingAttack == true)
        {
            CancelRequest = true;
            await Tasker.Run(() => {
                while (RequestingAttack) { }
            });
        }
        CancelRequest = false;
        arrows.Enable(false);
        ReselectionEnabled = false;

        Pane.GetPane("Game").gameObject.SetActive(false);
        Pane.GetPane("Cancel Buttons").gameObject.SetActive(true);

        Character FoundEnemy = null;
        Component FoundTile = null;

        List<Target> Targets = new List<Target>();

        var PlayerTargetPrefab = TileManager.PlayerTarget.GetComponent<Target>();

        for (int x = 0; x < TileManager.Width; x++)
        {
            for (int y = 0; y < TileManager.Height; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), transform.position) <= AttackRange && TileManager.GetGameTile(x, y) != this)
                {
                    var NewTarget = GameObject.Instantiate(PlayerTargetPrefab.gameObject).GetComponent<Target>();
                    NewTarget.transform.position = new Vector3(x, y, transform.position.z - 0.3f);
                    Targets.Add(NewTarget);
                    var Coordinates = new Vector2Int(x, y);

                    NewTarget.TargetSelectEvent += () => {
                        CDebug.Log("CCC");
                        if (TileManager.HasGameTile(Coordinates.x, Coordinates.y))
                        {
                            CDebug.Log("BBB");
                            var GameTile = TileManager.GetGameTile(Coordinates.x, Coordinates.y);
                            if (GameTile is Trail trail)
                            {
                                if (trail.Host is Enemy)
                                {
                                    CDebug.Log("Found ENEMY");
                                    FoundEnemy = trail.Host;
                                    FoundTile = trail;
                                }
                            }
                            else if (GameTile is Enemy enemy)
                            {
                                //CDebug.Log("Found ENEMY 2");
                                FoundEnemy = enemy;
                                FoundTile = enemy;
                            }
                        }
                    };
                }
            }
        }
        await Tasker.Run(() =>
        {
            while (FoundEnemy == null) { }
        });
        foreach (var target in Targets)
        {
            GameObject.Destroy(target.gameObject);
        }
        //RequestingAttack = false;

        return (FoundEnemy, FoundTile);
    }*/

    private (Player,Component) FindNearestPlayer()
    {
        float? Distance = null;
        (Player, Component) SelectedPair = (null, null);
        foreach (var player in Player.Players)
        {
            foreach (var trail in player.Trails)
            {
                var distance = Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int((int)trail.transform.position.x, (int)trail.transform.position.y));
                if (Distance == null || distance < Distance.Value)
                {
                    Distance = distance;
                    SelectedPair = (player, trail);
                }
            }
            var dist = Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y));
            if (Distance == null || dist < Distance.Value)
            {
                Distance = dist;
                SelectedPair = (player, player);
            }
        }
        return SelectedPair;
    }

    private Vector2Int? FindNextMove(Component target,Vector2Int? Position = null)
    {
        if (Position == null)
        {
            Position = new Vector2Int((int)transform.position.x,(int)transform.position.y);
        }
        Vector2Int position = Position.Value;
        if (Vector2.Distance(target.transform.position,transform.position) <= 1)
        {
            return null;
        }
        else
        {
            Vector2Int nextPosition = new Vector2Int(position.x,position.y + 1);
            var Distance = GetDistance(nextPosition, target);

            Vector2Int newPosition = new Vector2Int(position.x, position.y - 1);
            var newDistance = GetDistance(newPosition, target);
            if (newDistance < Distance)
            {
                Distance = newDistance;
                nextPosition = newPosition;
            }

            newPosition = new Vector2Int(position.x - 1, position.y);
            newDistance = GetDistance(newPosition, target);
            if (newDistance < Distance)
            {
                Distance = newDistance;
                nextPosition = newPosition;
            }

            newPosition = new Vector2Int(position.x + 1, position.y);
            newDistance = GetDistance(newPosition, target);
            if (newDistance < Distance)
            {
                Distance = newDistance;
                nextPosition = newPosition;
            }
            if (Game.WithinBounds(nextPosition.x,nextPosition.y) && Game.GetTile(nextPosition.x,nextPosition.y) != null && Game.HasGameTile(nextPosition.x, nextPosition.y) == false)
            {
                return nextPosition;
            }
            return null;
        }
    }

    private float GetDistance(Vector2Int pos,Component target)
    {
        return Vector2Int.Distance(pos, new Vector2Int((int)target.transform.position.x, (int)target.transform.position.y));
    }

    protected override void TurnStart()
    {
        //throw new System.NotImplementedException();
        EnemyRoutine();
    }

    private class PathSpace
    {
        public PathSpace PreviousPath;
        public Vector2Int CurrentSpace;
        public PathSpace(PathSpace previousSpace,Vector2Int currentSpace)
        {
            PreviousPath = previousSpace;
            CurrentSpace = currentSpace;
        }
        public override bool Equals(object obj)
        {

            if (!(obj is null) && obj is PathSpace path)
            {
                //Debug.Log("Paths Are Equal = " + (path.CurrentSpace == CurrentSpace));
                return path.CurrentSpace == CurrentSpace;
            }
            //Debug.Log("END2");
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1567871253;
            hashCode = hashCode * -1521134295 + EqualityComparer<PathSpace>.Default.GetHashCode(PreviousPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(CurrentSpace);
            return hashCode;
        }

        public static bool operator==(PathSpace A, PathSpace B)
        {
            if (A is null && B is null)
            {
                //Debug.Log("NULLS");
                return true;
            }
            else if (!(A is null))
            {
                //Debug.Log("AEQUALS");
                return A.Equals(B);
            }
            else if (!(B is null))
            {
                //Debug.Log("BEQUALS");
                return B.Equals(A);
            }
            return false;
        }

        public static bool operator !=(PathSpace A, PathSpace B)
        {
            return !(A == B);
        }
    }

    private bool IsPathPlayer(Vector2Int position)
    {
        /*if (position.x < 0 || position.y < 0 || position.x > Game.Width - 1 || position.y > Game.Height - 1)
        {
            return false;
        }*/
        var tile = Game.GetGameTile(position.x, position.y);
        if (tile is Player)
        {
            return true;
        }
        else if (tile is Trail trail && trail.Host is Player)
        {
            return true;
        }
        return false;
    }

    private bool IsPathValid(Vector2Int position)
    {
        return Game.GetTile(position.x, position.y) != null && Game.GetGameTile(position.x, position.y) == null;
    }

    private static List<Vector2Int> GetCardinalDirections(Vector2Int position)
    {
        return new List<Vector2Int>() { position + Vector2Int.up, position + Vector2Int.down, position + Vector2Int.left, position + Vector2Int.right };
    }

    private static Vector2Int Convert(Vector2 pos)
    {
        return new Vector2Int((int)pos.x,(int)pos.y);
    }

    private static Task Wait(int time)
    {
        return Task.Run(() => Thread.Sleep(time));
        
    }

    private class V2IComparer : IEqualityComparer<Vector2Int>
    {
        public bool Equals(Vector2Int x, Vector2Int y)
        {
            return x.x == y.x && x.y == y.y;
        }

        public int GetHashCode(Vector2Int obj)
        {
            return obj.GetHashCode();
        }
    }

    private async Task<(Player, Component, List<Vector2Int>)> GetNextSpaces()
    {
        try
        {
            //Debug.LogWarning("START");
            List<PathSpace> PathsToCalculate = new List<PathSpace>() { new PathSpace(null, Convert(transform.position)) };
            List<PathSpace> NextPathsToCalc = new List<PathSpace>();
            List<Vector2Int> UsedPositions = new List<Vector2Int>();
            var Comparer = new V2IComparer();
            List<(PathSpace, Player, Component)> PlayersFound = new List<(PathSpace, Player, Component)>();
            do
            {
                //Debug.Log("Doing Paths");
                foreach (var path in PathsToCalculate)
                {
                    UsedPositions.Add(path.CurrentSpace);
                    var cardinals = GetCardinalDirections(path.CurrentSpace);
                    foreach (var cardinal in cardinals)
                    {
                        //Debug.Log("G");
                        if (Game.WithinBounds(cardinal.x,cardinal.y) && !UsedPositions.Contains(cardinal) && !NextPathsToCalc.Any(setPath => setPath.CurrentSpace == cardinal))
                        {
                            //Debug.Log("H");
                            if (IsPathPlayer(cardinal) && !PlayersFound.Exists(space => space.Item1.Equals(new PathSpace(path, cardinal))))
                            {
                                //Debug.Log("I");
                                var tile = Game.GameMap[cardinal.x, cardinal.y];
                                Player player = null;
                                Component target = tile;
                                if (tile is Player)
                                {
                                    Debug.LogWarning("Added Player");
                                    player = tile as Player;
                                }
                                else if (tile is Trail trail && trail.Host is Player p)
                                {
                                    //player = (tile as Trail).Host as Player;
                                    Debug.LogWarning("Added Trail Based Player = " + p);
                                    player = p;
                                }
                                
                                PlayersFound.Add((new PathSpace(path, cardinal), player, target));
                                //UsedPositions.Add(cardinal);
                            }
                            else if (IsPathValid(cardinal))
                            {
                                //Debug.Log("J");
                                NextPathsToCalc.Add(new PathSpace(path, cardinal));
                                //Debug.Log("Adding New Path");
                                //UsedPositions
                            }
                            //Debug.Log("1");
                            //await Wait(1);
                            //await Task.Run(() => { Thread.Sleep(10); });
                            //else
                            //{
                            //    Debug.Log("K");
                            //}
                        }
                        else
                        {
                            //Debug.Log("XXX");
                        }

                    }
                    //Debug.Log("3");
                    //await Wait(1);
                }
                Debug.Log("Used Positions = " + UsedPositions.Count);
                Debug.Log("Next Points = " + NextPathsToCalc.Count);
                Debug.Log("Paths to Calc = " + PathsToCalculate.Count);
                Debug.Log("2");
                await Wait(1);
                //await Task.Run(() => { Thread.Sleep(10); });
                //await Wait(1);
                //var Temp = PathsToCalculate;
                //PathsToCalculate = NextPathsToCalc;
                //NextPathsToCalc = Temp;
                //NextPathsToCalc.Clear();


                PathsToCalculate = NextPathsToCalc;
                NextPathsToCalc = new List<PathSpace>();
                //var Temp = NextPathsToCalc;
                //PathsToCalculate = NextPathsToCalc;
                //var Temp = PathsToCalculate;
                //PathsToCalculate = NextPathsToCalc;
                //NextPathsToCalc = Temp;
                //NextPathsToCalc.Clear();

                //Debug.Log("Paths to calculate = " + PathsToCalculate.Count);
            } while (PlayersFound.Count == 0 && PathsToCalculate.Count > 0);
            // Debug.Log("Used Spaces = " + UsedPositions.Count);
            // Debug.Log("Players Found = " + PlayersFound.Count);
            if (PlayersFound.Count > 0)
            {
                var CurrentPlayer = PlayersFound[0].Item2;
                var CurrentTarget = PlayersFound[0].Item3;
                PathSpace CurrentPath = PlayersFound[0].Item1;
                List<Vector2Int> PathToTarget = new List<Vector2Int>();
                //Debug.Log("LOOP START");
                while (CurrentPath != null)
                {
                    //Debug.Log("Next = " + CurrentPath);
                    //Debug.Log("Null = " + CurrentPath == null);
                    PathToTarget.Add(CurrentPath.CurrentSpace);
                    CurrentPath = CurrentPath.PreviousPath;
                }
                //Debug.LogWarning("Path Start");
                foreach (var path in PathToTarget)
                {
                    Debug.Log("Path = " + path);
                }
                //Debug.LogWarning("Path End");
                //Debug.Log("Path = " + PathToTarget.Count);
                PathToTarget.Reverse();
                PathToTarget.Remove(PlayersFound[0].Item1.CurrentSpace);
                return (CurrentPlayer, CurrentTarget, PathToTarget);

            }
            else
            {
                 //Debug.Log("WORST CASE SCENARIO FOUND");
                var (FoundPlayer, FoundTrail) = FindNearestPlayer();
                if (FoundPlayer != null)
                {
                    List<Vector2Int> PathToTarget = new List<Vector2Int>() { Convert(transform.position) };
                    Vector2Int? NextPos = Convert(transform.position);
                    while (NextPos != null)
                    {
                        NextPos = FindNextMove(FoundTrail, NextPos.Value);
                        if (NextPos != null)
                        {
                            PathToTarget.Add(NextPos.Value);
                        }
                    }
                    if (PathToTarget.Count > 0)
                    {
                        return (FoundPlayer, FoundTrail, PathToTarget);
                    }
                    //var NextPos = FindNextMove(FoundTrail,);
                }
                return (null, null, null);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return (null, null, null);
        }
    }

    private async Task EnemyRoutine()
    {
        //var (FoundPlayer, FoundTrail) = FindNearestPlayer();
        try
        {
            //Debug.Log("A");
            var (FoundPlayer, FoundTrail, Spaces) = await GetNextSpaces();

            //Debug.Log("Spaces = " + Spaces.Count);
           // Debug.Log("B");
            if (FoundPlayer != null)
            {
                //Debug.Log("Spaces = " + Spaces.Count);
                for (int i = 1; i < MovesMax + 1; i++)
                {
                    //var nextMove = FindNextMove(FoundTrail);
                    if (i > Spaces.Count - 1)
                    {
                        Debug.Log("Breaking");
                        break;
                    }
                    else
                    {
                        Debug.Log("Moving To " + Spaces[i]);
                        await Move(Spaces[i], 7f);
                    }
                }
            }
            else
            {
                Debug.Log("NULL");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }



        //TEMPORARY
        var (AttackPlayer, AttackTrail) = await RequestToAttack();
        if (AttackTrail != null)
        {
            AttackedEnemy = true;
            await AttackPlayer.Damage(AttackDamage);
        }
        CDebug.Log("FINISHING ENEMY TURN");
        FinishTurn();
    }

    private async Task ActionRoutine()
    {

    }

    public static void ResetTurns()
    {
        foreach (var enemy in Enemies)
        {
            enemy.ResetTurn();
        }
    }

    public static async Task BeginTurns()
    {
        ResetTurns();
        foreach (var enemy in Enemies)
        {
            ActiveEnemiesLeft.Add(enemy);
        }
        EnemyTurn = true;
        ActiveEnemiesLeft[0].Select();
        await Tasker.Run(() => {
            while (ActiveEnemiesLeft.Count > 0) { }
        });
        foreach (var enemy in enemies)
        {
            if (enemy.Check != null)
            {
                GameObject.Destroy(enemy.Check);
                enemy.Check = null;
            }
        }
        EnemyTurn = false;
    }
}
