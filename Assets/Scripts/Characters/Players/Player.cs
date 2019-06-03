using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        CDebug.Log("PLAYERS TURN" + PlayersTurn);
        if (PlayersTurn)
        {
            if (SelectionTarget == null)
            {
                SelectionTarget = GameObject.FindGameObjectWithTag("SelectionTarget").GetComponent<SpriteRenderer>();
                SelectionTarget.enabled = true;
                //if (arrows == null)
                //{
                var ArrowObjects = GameObject.FindGameObjectsWithTag("Arrow");
                arrows = new Arrows(ArrowObjects.First(g => g.name == "Up Arrow"),
                    ArrowObjects.First(g => g.name == "Down Arrow"),
                    ArrowObjects.First(g => g.name == "Left Arrow"),
                    ArrowObjects.First(g => g.name == "Right Arrow"));
                foreach (var arrow in ArrowObjects)
                {
                    arrow.GetComponent<BoxCollider2D>().enabled = true;
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
                arrows.Host = this;
                // }
            }
            base.Select();
            CDebug.Log("Same Player = " + (ActivePlayer == this));
            if (ActivePlayer == this)
            {
                MainButtons.Attack.SetActive(true);
                MainButtons.FinishTurn.SetActive(true);
                //MainButtons.CancelAttack.SetActive(false);
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
        CDebug.Log("MOVING CHARACTER");
        Move(newPosition, 7f);
        arrows.Check(newPosition);
    }

    public sealed override void OnSpawn()
    {
        base.OnSpawn();
        players.Add(this);
    }

    protected override void TurnEnd()
    {
        //throw new System.NotImplementedException();
        CDebug.Log("FINISHED PLAYER TURN");
        ActivePlayersLeft.Remove(this);
        if (ActivePlayersLeft.Count > 0)
        {
            ActivePlayersLeft[0].Select();
        }
    }

    protected override void TurnPostpone()
    {
        // throw new System.NotImplementedException();
    }

    protected override void TurnStart()
    {
       // throw new System.NotImplementedException();
    }

    public async void InitiateAttack()
    {
        var (FoundEnemy, FoundTile) = await RequestToAttack();
        if (FoundEnemy != null)
        {
            AttackedEnemy = true;
            await FoundEnemy.Damage(AttackDamage);
            //arrows.Enable(true);
            //Pane.GetPane("Game").gameObject.SetActive(true);
            //Pane.GetPane("Cancel Buttons").gameObject.SetActive(false);
            MainButtons.Attack.SetActive(false);
            MainButtons.FinishTurn.SetActive(false);
            MainButtons.CancelAttack.SetActive(true);
            ReselectionEnabled = true;
            FinishTurn();
        }
        else
        {
            //Pane.GetPane("Game").gameObject.SetActive(true);
            //Pane.GetPane("Cancel Buttons").gameObject.SetActive(false);
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

    protected override async Task<(Character,Component)> RequestToAttack()
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

        //Pane.GetPane("Game").gameObject.SetActive(false);
        //Pane.GetPane("Cancel Buttons").gameObject.SetActive(true);
        MainButtons.Attack.SetActive(false);
        MainButtons.FinishTurn.SetActive(false);
        MainButtons.CancelAttack.SetActive(true);

        Character FoundEnemy = null;
        Component FoundTile = null;

        List<Target> Targets = new List<Target>();

        var PlayerTargetPrefab = Game.PlayerTarget.GetComponent<Target>();

        foreach (var enemy in Enemy.Enemies)
        {
            enemy.GetComponent<BoxCollider2D>().enabled = false;
        }
        
        for (int x = 0; x < Game.Width; x++)
        {
            for (int y = 0; y < Game.Height; y++)
            {
                if (Vector2.Distance(new Vector2(x,y),transform.position) <= AttackRange && Game.GetGameTile(x,y) != this)
                {
                    var NewTarget = GameObject.Instantiate(PlayerTargetPrefab.gameObject).GetComponent<Target>();
                    NewTarget.transform.position = new Vector3(x, y,transform.position.z);
                    Targets.Add(NewTarget);
                    var Coordinates = new Vector2Int(x, y);
                    var gameTile = Game.GameMap[x, y];
                    var renderer = NewTarget.GetComponent<SpriteRenderer>();
                    var Alpha = 0.3f;
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

                        /*if (gameTile is Trail trail && !(trail.Host is Enemy))
                        {
                            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, Alpha);
                        }
                        else if (!(gameTile is Enemy))
                        {
                            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, Alpha);
                        }*/
                        /*if (!(gameTile is Enemy))
                        {
                            
                        }*/

                    }
                    //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
                    NewTarget.TargetSelectEvent += () => {
                        //CDebug.Log("CCC");
                        if (Game.HasGameTile(Coordinates.x,Coordinates.y))
                        {
                            //CDebug.Log("BBB");
                            var GameTile = Game.GetGameTile(Coordinates.x, Coordinates.y);
                            if (GameTile is Trail trail)
                            {
                                if (trail.Host is Enemy)
                                {
                                    //CDebug.Log("Found ENEMY");
                                    FoundEnemy = trail.Host;
                                    FoundTile = trail;
                                }
                            }
                            else if (GameTile is Enemy enemy)
                            {
                               // CDebug.Log("Found ENEMY 2");
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
            while (FoundEnemy == null && CancelRequest == false) { }
        });
        foreach (var target in Targets)
        {
            GameObject.Destroy(target.gameObject);
        }
        foreach (var enemy in Enemy.Enemies)
        {
            enemy.GetComponent<BoxCollider2D>().enabled = true;
        }
        /*if (ReEnable)
        {
            
        }
        arrows.Enable(true);
        Pane.GetPane("Game").gameObject.SetActive(true);
        Pane.GetPane("Cancel Buttons").gameObject.SetActive(false);
        ReselectionEnabled = true;*/
        RequestingAttack = false;

        return (FoundEnemy,FoundTile);
    }


    protected override async Task Move(Vector2Int to, float Speed, bool spawnTrail = true)
    {
        await base.Move(to, Speed, spawnTrail);
        if (MovesDone == MovesMax)
        {
            arrows.Enable(false);
        }
    }
    public static void ResetTurns()
    {
        foreach (var player in Players)
        {
            player.ResetTurn();
        }
    }

    public static bool AutoSelect = true;

    public static async Task BeginTurns()
    {
        ResetTurns();
        foreach (var player in Players)
        {
            ActivePlayersLeft.Add(player);
        }
        PlayersTurn = true;
        //Pane.GetPane("Game").gameObject.SetActive(true);
        if (AutoSelect)
        {
            ActivePlayersLeft[0].Select();
        }
        await Tasker.Run(() => {
            while (ActivePlayersLeft.Count > 0) { }
        });
        foreach (var player in Player.players)
        {
            if (player.Check != null)
            {
                GameObject.Destroy(player.Check);
                player.Check = null;
            }
        }
        //Pane.GetPane("Game").gameObject.SetActive(false);
        PlayersTurn = false;
    }
}
