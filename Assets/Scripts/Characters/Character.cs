﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Character : MonoBehaviour, IHasID
{
    protected GameObject Check;
    new private AudioSource audio;
    protected bool AttackedEnemy = false;
    public static bool ReselectionEnabled { get; protected set; } = true;
    public virtual void OnSpawn()
    {
        Game.GameMap[(int)transform.position.x, (int)transform.position.y] = this;
        audio = GetComponent<AudioSource>();
    }
    public virtual void OnGameStart()
    {

    }
    public virtual void OnDestruction()
    {
        /*for (int i = 0; i < Trails.Count; i++)
        {
            RemoveLastTrail();
        }*/
        Game.GameMap[(int)transform.position.x, (int)transform.position.y] = null;
    }

    protected abstract void TurnStart();
    protected abstract void TurnPostpone();
    protected abstract void TurnEnd();

    public virtual string Name => GetType().Name;
    public abstract string Info { get; }

    public virtual void ResetTurn()
    {
        MovesDone = 0;
        AttackedEnemy = false;
        FinishedTurn = false;
    }

    private bool FinishedTurn = false;
    protected bool Moving = false;

    public virtual void Select()
    {
        //Deselect if the this is the active character
        if (FinishedTurn == false)
        {
            if (ActiveCharacter == this)
            {
                ActiveCharacter.Deselect();
                return;
            }
            if (ActiveCharacter != null)
            {
                ActiveCharacter.Deselect();
            }
            audio.clip = Sounds.RiseSound;
            audio.Play();
            ActiveCharacter = this;
            if (Trails.Count == MaxTrailLength)
            {
                Trails[Trails.Count - 1].Flash = true;
            }
            TurnStart();
        }
    }

    public virtual void Deselect()
    {
        if (this == ActiveCharacter)
        {
            if (!AttackedEnemy)
            {
                audio.clip = Sounds.LowerSound;
                audio.Play();
            }
            ActiveCharacter = null;
            if (Trails.Count > 0)
            {
                Trails[Trails.Count - 1].Flash = false;
            }
            if (FinishedTurn)
            {
                TurnEnd();
            }
            else
            {
                //var check = GameObject.Instantiate(Game.CharacterCheck, transform.position, Quaternion.identity);
                //check.SetActive(true);
                //Check = check;
                TurnPostpone();
            }
        }
    }

    public void FinishTurn()
    {
        var check = GameObject.Instantiate(Game.CharacterCheck, transform.position, Quaternion.identity);
        check.SetActive(true);
        Check = check;
        FinishedTurn = true;
        Deselect();
    }

    void OnMouseDown()
    {
        if (Game.GameStarted)
        {
            if (ReselectionEnabled)
            {
                Select();
            }
        }
        else
        {
            (Game.GetTile((int)transform.position.x, (int)transform.position.y) as SpawnTile)?.OnMouseDown();
        }
    }

    public static Character ActiveCharacter { get; private set; }

    public virtual int MovesMax => 4;
    public virtual int AttackRange => 4;
    public virtual int MaxTrailLength => 4;
    public virtual int AttackDamage => 2;
    public int MovesDone { get; protected set; }
    [SerializeField]
    protected Color characterColor;
    public Color CharacterColor => characterColor;
    bool Done = false;

    protected List<Trail> trails = new List<Trail>();

    public ReadOnlyCollection<Trail> Trails => trails.AsReadOnly();

    protected virtual async Task Move(Vector2Int to,float Speed,bool spawnTrail = true)
    {
        //Done = false;
        if (MovesDone == MovesMax)
        {
            return;
        }
        if (Moving)
        {
            Moving = false;
            await Tasker.Run(() => {
                while (!Done) { }
            });
        }
        Done = false;
        float T = 0;
        audio.PlayOneShot(Sounds.MovementSound);
        Vector3 From = transform.position;
        Vector3 To = new Vector3(to.x,to.y,transform.position.z);
        Action Update = () =>
        {
            T += Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(From,To,1 - Mathf.Pow(1 - T,4));
            if (T >= 1)
            {
                Done = true;
            }
        };
        Game.GameMap[(int)From.x, (int)From.y] = null;
        Game.GameMap[(int)To.x, (int)To.y] = this;
        MovesDone++;
        if (spawnTrail)
        {
            Debug.Log("Spawning Trail");
            try
            {
                SpawnTrail();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
        }
        Moving = true;
        CharacterStats.Refresh();
        StaticUpdate.Updates += Update;
        await Tasker.Run(() => {
            while (!Done && Moving) { }
        });
        transform.position = To;
        Done = true;
        Moving = false;
        StaticUpdate.Updates -= Update;
    }

    public async Task Damage(int damage)
    {
        audio.PlayOneShot(Sounds.DamageSound);
        for (int i = 0; i < damage; i++)
        {
            if (trails.Count > 0)
            {
                RemoveLastTrail();
                await Task.Run(() => Thread.Sleep(100));
            }
            else
            {
                Game.DestroyCharacter(this);
                await Task.Run(() => Thread.Sleep(100));
                break;
            }
        }
    }
    //Requests an enemy to attack
    protected async virtual Task<(Character,Component)> RequestToAttack()
    {
        return (null,null);
    }

    protected Trail SpawnTrail(bool ToEnd = false)
    {
        //System.Span<int> test;
        Debug.Log("A");
        var NewTrail = GameObject.Instantiate(Game.Trail.gameObject).GetComponent<Trail>();
        Debug.Log("B");
        NewTrail.transform.position = transform.position; //new Vector3(transform.position.x,transform.position.y,transform.position.z - 2);
        NewTrail.GetComponent<SpriteRenderer>().color = CharacterColor;
        NewTrail.Host = this;
        Game.GameMap[(int)transform.position.x, (int)transform.position.y] = NewTrail;
        trails.Insert(0, NewTrail);
        if (Trails.Count > MaxTrailLength)
        {
            RemoveLastTrail();
        }
        if (Trails.Count == MaxTrailLength)
        {
            Trails[Trails.Count - 1].Flash = true;
        }
        return NewTrail;
    }
    protected void RemoveLastTrail()
    {
        var LastTrail = trails[trails.Count - 1];
        trails.Remove(LastTrail);
        Game.GameMap[(int)LastTrail.transform.position.x, (int)LastTrail.transform.position.y] = null;
        GameObject.Destroy(LastTrail.gameObject);
    }

    public void DeleteAllTrails()
    {
        foreach (var trail in trails)
        {
            Game.GameMap[(int)trail.transform.position.x, (int)trail.transform.position.y] = null;
            GameObject.Destroy(trail.gameObject);
        }
        trails.Clear();
    }

    public static T Spawn<T>(Vector2Int Position) where T : Character
    {
        return Game.SpawnCharacter<T>(Position);
    }

    public abstract int GetTileID();
}
