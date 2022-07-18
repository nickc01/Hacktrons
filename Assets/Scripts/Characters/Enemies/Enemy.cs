using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

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
        ActiveEnemiesLeft.Remove(this);
        if (ActiveEnemiesLeft.Count > 0)
        {
            ActiveEnemiesLeft[0].Select();
        }
    }

    protected override void TurnPostpone()
    {

    }

    protected override IEnumerator RequestToAttack()
    {
        (Player FoundPlayer, Component FoundTrail) = FindNearestPlayer();
        if (FoundPlayer == null)
        {
            yield return base.RequestToAttack();
        }
        List<Target> Targets = new List<Target>();
        Target EnemyTargetPrefab = Game.EnemyTarget.GetComponent<Target>();

        for (int x = 0; x < Game.Width; x++)
        {
            for (int y = 0; y < Game.Height; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), transform.position) <= AttackRange && Game.GetGameTile(x, y) != this)
                {
                    Target NewTarget = GameObject.Instantiate(EnemyTargetPrefab.gameObject).GetComponent<Target>();
                    NewTarget.transform.position = new Vector3(x, y, transform.position.z);
                    Targets.Add(NewTarget);
                    Component GameTile = Game.GameMap[x, y];
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
                    SpriteRenderer renderer = NewTarget.GetComponent<SpriteRenderer>();
                    renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.3f);
                }
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (Target target in Targets)
        {
            GameObject.Destroy(target.gameObject);
        }
        if (Vector2.Distance(FoundTrail.transform.position, transform.position) <= AttackRange)
        {
            LastRequestedCharacter = FoundPlayer;
            LastRequestedComponent = FoundTrail;
        }
        else
        {
            yield return base.RequestToAttack();
        }
    }

    private (Player, Component) FindNearestPlayer()
    {
        float? Distance = null;
        (Player, Component) SelectedPair = (null, null);
        foreach (Player player in Player.Players)
        {
            foreach (Trail trail in player.Trails)
            {
                float distance = Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int((int)trail.transform.position.x, (int)trail.transform.position.y));
                if (Distance == null || distance < Distance.Value)
                {
                    Distance = distance;
                    SelectedPair = (player, trail);
                }
            }
            float dist = Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y));
            if (Distance == null || dist < Distance.Value)
            {
                Distance = dist;
                SelectedPair = (player, player);
            }
        }
        return SelectedPair;
    }

    private Vector2Int? FindNextMove(Component target, Vector2Int? Position = null)
    {
        if (Position == null)
        {
            Position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        }
        Vector2Int position = Position.Value;
        if (Vector2.Distance(target.transform.position, transform.position) <= 1)
        {
            return null;
        }
        else
        {
            Vector2Int nextPosition = new Vector2Int(position.x, position.y + 1);
            float Distance = GetDistance(nextPosition, target);

            Vector2Int newPosition = new Vector2Int(position.x, position.y - 1);
            float newDistance = GetDistance(newPosition, target);
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
            if (Game.WithinBounds(nextPosition.x, nextPosition.y) && Game.GetTile(nextPosition.x, nextPosition.y) != null && Game.HasGameTile(nextPosition.x, nextPosition.y) == false)
            {
                return nextPosition;
            }
            return null;
        }
    }

    private float GetDistance(Vector2Int pos, Component target)
    {
        return Vector2Int.Distance(pos, new Vector2Int((int)target.transform.position.x, (int)target.transform.position.y));
    }

    protected override void TurnStart()
    {
        StartCoroutine(EnemyRoutine());
    }

    private class PathSpace
    {
        public PathSpace PreviousPath;
        public Vector2Int CurrentSpace;
        public PathSpace(PathSpace previousSpace, Vector2Int currentSpace)
        {
            PreviousPath = previousSpace;
            CurrentSpace = currentSpace;
        }
        public override bool Equals(object obj)
        {

            if (!(obj is null) && obj is PathSpace path)
            {
                return path.CurrentSpace == CurrentSpace;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -1567871253;
            hashCode = hashCode * -1521134295 + EqualityComparer<PathSpace>.Default.GetHashCode(PreviousPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(CurrentSpace);
            return hashCode;
        }

        public static bool operator ==(PathSpace A, PathSpace B)
        {
            if (A is null && B is null)
            {
                return true;
            }
            else if (!(A is null))
            {
                return A.Equals(B);
            }
            else if (!(B is null))
            {
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
        Component tile = Game.GetGameTile(position.x, position.y);
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
        return new Vector2Int((int)pos.x, (int)pos.y);
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

    public class NextSpacesResult
    {
        public Player Player;
        public Component Component;
        public List<Vector2Int> spaces;
    }

    private IEnumerator GetNextSpaces(NextSpacesResult result)
    {
        List<PathSpace> PathsToCalculate = new List<PathSpace>() { new PathSpace(null, Convert(transform.position)) };
        List<PathSpace> NextPathsToCalc = new List<PathSpace>();
        List<Vector2Int> UsedPositions = new List<Vector2Int>();
        V2IComparer Comparer = new V2IComparer();
        List<(PathSpace, Player, Component)> PlayersFound = new List<(PathSpace, Player, Component)>();
        do
        {
            foreach (PathSpace path in PathsToCalculate)
            {
                UsedPositions.Add(path.CurrentSpace);
                List<Vector2Int> cardinals = GetCardinalDirections(path.CurrentSpace);
                foreach (Vector2Int cardinal in cardinals)
                {
                    if (Game.WithinBounds(cardinal.x, cardinal.y) && !UsedPositions.Contains(cardinal) && !NextPathsToCalc.Any(setPath => setPath.CurrentSpace == cardinal))
                    {
                        if (IsPathPlayer(cardinal) && !PlayersFound.Exists(space => space.Item1.Equals(new PathSpace(path, cardinal))))
                        {
                            Component tile = Game.GameMap[cardinal.x, cardinal.y];
                            Player player = null;
                            Component target = tile;
                            if (tile is Player)
                            {
                                player = tile as Player;
                            }
                            else if (tile is Trail trail && trail.Host is Player p)
                            {
                                player = p;
                            }

                            PlayersFound.Add((new PathSpace(path, cardinal), player, target));
                        }
                        else if (IsPathValid(cardinal))
                        {
                            NextPathsToCalc.Add(new PathSpace(path, cardinal));
                        }
                    }
                }
            }
            yield return null;


            PathsToCalculate = NextPathsToCalc;
            NextPathsToCalc = new List<PathSpace>();
        } while (PlayersFound.Count == 0 && PathsToCalculate.Count > 0);
        if (PlayersFound.Count > 0)
        {
            Player CurrentPlayer = PlayersFound[0].Item2;
            Component CurrentTarget = PlayersFound[0].Item3;
            PathSpace CurrentPath = PlayersFound[0].Item1;
            List<Vector2Int> PathToTarget = new List<Vector2Int>();
            while (CurrentPath != null)
            {
                PathToTarget.Add(CurrentPath.CurrentSpace);
                CurrentPath = CurrentPath.PreviousPath;
            }
            PathToTarget.Reverse();
            PathToTarget.Remove(PlayersFound[0].Item1.CurrentSpace);
            result.Player = CurrentPlayer;
            result.Component = CurrentTarget;
            result.spaces = PathToTarget;
            yield break;
        }
        else
        {
            (Player FoundPlayer, Component FoundTrail) = FindNearestPlayer();
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
                    result.Player = FoundPlayer;
                    result.Component = FoundTrail;
                    result.spaces = PathToTarget;
                    yield break;
                }
            }
            yield break;
        }
    }

    private IEnumerator EnemyRoutine()
    {
        NextSpacesResult result = new NextSpacesResult();
        yield return GetNextSpaces(result);

        if (result.Player != null)
        {
            for (int i = 1; i < MovesMax + 1; i++)
            {
                if (i > result.spaces.Count - 1)
                {
                    break;
                }
                else
                {
                    yield return Move(result.spaces[i], 7f);
                }
            }
        }



        yield return RequestToAttack();
        if (LastRequestedComponent != null)
        {
            AttackedEnemy = true;
            yield return LastRequestedCharacter.Damage(AttackDamage);
        }
        FinishTurn();
    }

    public static void ResetTurns()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.ResetTurn();
        }
    }

    public static IEnumerator BeginTurns()
    {
        ResetTurns();
        foreach (Enemy enemy in Enemies)
        {
            ActiveEnemiesLeft.Add(enemy);
        }
        EnemyTurn = true;
        ActiveEnemiesLeft[0].Select();
        yield return new WaitUntil(() => ActiveEnemiesLeft.Count <= 0);
        foreach (Enemy enemy in enemies)
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
