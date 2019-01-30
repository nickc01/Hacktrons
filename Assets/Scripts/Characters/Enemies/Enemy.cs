using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Enemy : Character
{
    private static List<Enemy> enemies = new List<Enemy>();
    public static ReadOnlyCollection<Enemy> Enemies => enemies.AsReadOnly();

    public static Enemy ActiveEnemy => ActiveCharacter as Enemy;

    private static List<Enemy> ActiveEnemiesLeft = new List<Enemy>();

    public static bool EnemyTurn { get; private set; } = false;

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
        }
    }

    public override void Deselect()
    {
        if (EnemyTurn)
        {
            base.Deselect();
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
                    NewTarget.transform.position = new Vector3(x, y, transform.position.z - 0.3f);
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
        await Task.Run(() => Thread.Sleep(1000));
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
            await Task.Run(() => {
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
        await Task.Run(() =>
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

    private Vector2Int? FindNextMove(Component target)
    {
        if (Vector2.Distance(target.transform.position,transform.position) <= 1)
        {
            return null;
        }
        else
        {
            Vector2Int nextPosition = new Vector2Int((int)transform.position.x,(int)transform.position.y + 1);
            var Distance = GetDistance(nextPosition, target);

            Vector2Int newPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y - 1);
            var newDistance = GetDistance(newPosition, target);
            if (newDistance < Distance)
            {
                Distance = newDistance;
                nextPosition = newPosition;
            }

            newPosition = new Vector2Int((int)transform.position.x - 1, (int)transform.position.y);
            newDistance = GetDistance(newPosition, target);
            if (newDistance < Distance)
            {
                Distance = newDistance;
                nextPosition = newPosition;
            }

            newPosition = new Vector2Int((int)transform.position.x + 1, (int)transform.position.y);
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

    private async Task EnemyRoutine()
    {
        var (FoundPlayer, FoundTrail) = FindNearestPlayer();
        if (FoundPlayer != null)
        {
            for (int i = 0; i < MovesMax; i++)
            {
                var nextMove = FindNextMove(FoundTrail);
                if (nextMove == null)
                {
                    break;
                }
                else
                {
                    await Move(nextMove.Value, 7f);
                }
            }
        }
        //TEMPORARY
        var (AttackPlayer, AttackTrail) = await RequestToAttack();
        if (AttackTrail != null)
        {
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
        await Task.Run(() => {
            while (ActiveEnemiesLeft.Count > 0) { }
        });
        EnemyTurn = false;
    }
}
