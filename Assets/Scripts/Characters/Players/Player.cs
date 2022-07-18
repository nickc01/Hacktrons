using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public abstract class Player : Character
{
    public static Action TutorialEvent;


    private static List<Player> players = new List<Player>();
    public static ReadOnlyCollection<Player> Players => players.AsReadOnly();

    public static Player ActivePlayer => ActiveCharacter as Player;

    private static List<Player> ActivePlayersLeft = new List<Player>();

    public static bool PlayersTurn { get; set; } = false;

    private static SpriteRenderer SelectionTarget;
    private static Arrows arrows;

    private bool RequestingAttack = false;
    public bool CancelRequest = false;

    public abstract int GetStartingAmount();

    public override void Select()
    {
        if (PlayersTurn)
        {
            if (SelectionTarget == null)
            {
                SelectionTarget = GameObject.FindGameObjectWithTag("SelectionTarget").GetComponent<SpriteRenderer>();
                SelectionTarget.enabled = true;
                GameObject[] ArrowObjects = GameObject.FindGameObjectsWithTag("Arrow");
                arrows = new Arrows(ArrowObjects.First(g => g.name == "Up Arrow"),
                    ArrowObjects.First(g => g.name == "Down Arrow"),
                    ArrowObjects.First(g => g.name == "Left Arrow"),
                    ArrowObjects.First(g => g.name == "Right Arrow"));
                foreach (GameObject arrow in ArrowObjects)
                {
                    arrow.GetComponent<BoxCollider2D>().enabled = true;
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
                arrows.Host = this;
            }
            base.Select();
            if (ActivePlayer == this)
            {
                MainButtons.Attack.SetActive(true);
                MainButtons.FinishTurn.SetActive(true);
                CharacterStats.SetCharacter(this);
                SelectionTarget.gameObject.SetActive(true);
                Selection.SelectedCharacter = this;
                SelectionTarget.transform.position = transform.position;
                if (MovesDone == MovesMax)
                {
                    arrows.Enable(false);
                }
                else
                {
                    arrows.Enable(true);
                    arrows.Check(new Vector2Int((int)transform.position.x, (int)transform.position.y));
                }
            }
        }
    }

    public override void Deselect()
    {
        SelectionTarget.gameObject.SetActive(false);
        Selection.SelectedCharacter = null;
        MainButtons.Attack.SetActive(false);
        MainButtons.FinishTurn.SetActive(false);
        MainButtons.CancelAttack.SetActive(false);
        CharacterStats.Clear();
        base.Deselect();
    }

    public override void OnDestruction()
    {
        base.OnDestruction();
        players.Remove(this);
        ActivePlayersLeft.Remove(this);
    }

    public void ArrowMove(Vector2Int newPosition)
    {
        StartCoroutine(Move(newPosition, 7f));
        arrows.Check(newPosition);
    }

    public sealed override void OnSpawn()
    {
        base.OnSpawn();
        players.Add(this);
    }

    protected override void TurnEnd()
    {
        ActivePlayersLeft.Remove(this);
        if (ActivePlayersLeft.Count > 0)
        {
            ActivePlayersLeft[0].Select();
        }
    }

    protected override void TurnPostpone()
    {

    }

    protected override void TurnStart()
    {

    }

    public IEnumerator InitiateAttack()
    {
        yield return RequestToAttack();
        if (LastRequestedCharacter != null)
        {
            AttackedEnemy = true;
            yield return LastRequestedCharacter.Damage(AttackDamage);
            MainButtons.Attack.SetActive(false);
            MainButtons.FinishTurn.SetActive(false);
            MainButtons.CancelAttack.SetActive(true);
            ReselectionEnabled = true;
            FinishTurn();
        }
        else
        {
            MainButtons.Attack.SetActive(true);
            MainButtons.FinishTurn.SetActive(true);
            MainButtons.CancelAttack.SetActive(false);
            ReselectionEnabled = true;
            if (MovesDone != MovesMax)
            {
                arrows.Enable(true);
            }
        }
    }

    protected override IEnumerator RequestToAttack()
    {
        if (RequestingAttack == true)
        {
            CancelRequest = true;
            yield return new WaitUntil(() => !RequestingAttack);
        }
        CancelRequest = false;
        arrows.Enable(false);
        ReselectionEnabled = false;
        MainButtons.Attack.SetActive(false);
        MainButtons.FinishTurn.SetActive(false);
        MainButtons.CancelAttack.SetActive(true);

        Character FoundEnemy = null;
        Component FoundTile = null;

        List<Target> Targets = new List<Target>();

        Target PlayerTargetPrefab = Game.PlayerTarget.GetComponent<Target>();

        foreach (Enemy enemy in Enemy.Enemies)
        {
            enemy.GetComponent<BoxCollider2D>().enabled = false;
        }

        for (int x = 0; x < Game.Width; x++)
        {
            for (int y = 0; y < Game.Height; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), transform.position) <= AttackRange && Game.GetGameTile(x, y) != this)
                {
                    Target NewTarget = GameObject.Instantiate(PlayerTargetPrefab.gameObject).GetComponent<Target>();
                    NewTarget.transform.position = new Vector3(x, y, transform.position.z);
                    Targets.Add(NewTarget);
                    Vector2Int Coordinates = new Vector2Int(x, y);
                    Component gameTile = Game.GameMap[x, y];
                    SpriteRenderer renderer = NewTarget.GetComponent<SpriteRenderer>();
                    float Alpha = 0.3f;
                    if (gameTile == null)
                    {
                        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, Alpha);
                    }
                    else
                    {
                        if (gameTile is Trail trail && trail.Host is Enemy)
                        {

                        }
                        else
                        {
                            if (gameTile is Enemy)
                            {

                            }
                            else
                            {
                                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, Alpha);
                            }
                        }

                    }
                    NewTarget.TargetSelectEvent += () =>
                    {
                        if (Game.HasGameTile(Coordinates.x, Coordinates.y))
                        {
                            Component GameTile = Game.GetGameTile(Coordinates.x, Coordinates.y);
                            if (GameTile is Trail trail)
                            {
                                if (trail.Host is Enemy)
                                {
                                    FoundEnemy = trail.Host;
                                    FoundTile = trail;
                                }
                            }
                            else if (GameTile is Enemy enemy)
                            {
                                FoundEnemy = enemy;
                                FoundTile = enemy;
                            }
                        }
                    };
                }
            }
        }
        yield return new WaitUntil(() => !(FoundEnemy == null && CancelRequest == false));
        foreach (Target target in Targets)
        {
            GameObject.Destroy(target.gameObject);
        }
        foreach (Enemy enemy in Enemy.Enemies)
        {
            enemy.GetComponent<BoxCollider2D>().enabled = true;
        }
        RequestingAttack = false;

        LastRequestedCharacter = FoundEnemy;
        LastRequestedComponent = FoundTile;
    }


    protected override IEnumerator Move(Vector2Int to, float Speed, bool spawnTrail = true)
    {
        yield return base.Move(to, Speed, spawnTrail);
        if (MovesDone == MovesMax)
        {
            arrows.Enable(false);
        }
    }
    public static void ResetTurns()
    {
        foreach (Player player in Players)
        {
            player.ResetTurn();
        }
    }

    public static bool AutoSelect = true;

    public static IEnumerator BeginTurns()
    {
        ResetTurns();
        foreach (Player player in Players)
        {
            ActivePlayersLeft.Add(player);
        }
        PlayersTurn = true;
        if (AutoSelect)
        {
            ActivePlayersLeft[0].Select();
        }
        yield return new WaitUntil(() => ActivePlayersLeft.Count <= 0);
        foreach (Player player in Player.players)
        {
            if (player.Check != null)
            {
                GameObject.Destroy(player.Check);
                player.Check = null;
            }
        }
        PlayersTurn = false;
    }
}
