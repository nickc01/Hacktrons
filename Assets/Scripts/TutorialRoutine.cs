using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class TutorialRoutine
{
    private static int LastClickedButton { get; set; } = 0;
    private static bool PlayerInRange { get; set; } = false;

    public static Task WaitWhile(Func<bool> Condition)
    {
        return Task.Run(() => { while (Condition()) { } });
    }


    public static bool TutorialActive { get; private set; } = false;

    private static float WaitTime = 30f / 1000f;
    private static Vector2 BasePosition = new Vector2(-220, -323);
    private static Vector2 ShownPosition = new Vector2(216f, -323);
    private static RectTransform TutorialTransform;
    private static IEnumerator Move(Vector2 position)
    {
        return Dragger.Drag(Game.Tutorial.gameObject, null, position, 3f, Interrupt: true);
    }

    private static IEnumerator PrintText(string text)
    {
        char[] characters = text.ToCharArray();
        TutorialArea.TextArea = "";
        Game.PrimaryAudio.clip = TutorialArea.TutorialSound;
        Game.PrimaryAudio.Play();
        foreach (char character in characters)
        {
            TutorialArea.TextArea += character;
            yield return new WaitForSeconds(WaitTime);
        }
        Game.PrimaryAudio.Stop();
    }

    private static IEnumerator ShowButtons(string button1, string button2 = null)
    {
        int buttonClicked = 0;
        Button FirstButton = TutorialArea.ButtonArea.transform.GetChild(0).GetComponent<Button>();
        FirstButton.GetComponentInChildren<TextMeshProUGUI>().text = button1;
        FirstButton.gameObject.SetActive(true);
        UnityEngine.Events.UnityAction func = () =>
        {
            if (buttonClicked == 0)
            {
                buttonClicked = 1;
            }
        };
        FirstButton.onClick.AddListener(func);
        UnityEngine.Events.UnityAction func2 = () =>
        {
            if (buttonClicked == 0)
            {
                buttonClicked = 2;
            }
        };
        Button SecondButton = null;
        if (button2 != null)
        {
            SecondButton = TutorialArea.ButtonArea.transform.GetChild(1).GetComponent<Button>();
            SecondButton.GetComponentInChildren<TextMeshProUGUI>().text = button2;
            SecondButton.gameObject.SetActive(true);
            SecondButton.onClick.AddListener(func2);
        }
        yield return new WaitUntil(() => buttonClicked != 0);
        FirstButton.onClick.RemoveAllListeners();
        if (button2 != null)
        {
            SecondButton.onClick.RemoveAllListeners();
            SecondButton.gameObject.SetActive(false);
        }
        FirstButton.gameObject.SetActive(false);
        LastClickedButton = buttonClicked;
    }

    public static void ActivateBool(ref bool value)
    {
        value = true;
    }

    private static bool eventClicked = false;
    private static Action currentFunc = null;

    public static void QueueEvent(ref Action e)
    {
        eventClicked = false;
        currentFunc = null;
        currentFunc = () =>
        {
            eventClicked = true;
        };
        e += currentFunc;
    }

    public static void FinishEvent(ref Action e)
    {
        e -= currentFunc;
        currentFunc = null;
    }

    public static IEnumerator AwaitEvent(Action eve)
    {
        yield return new WaitUntil(() => eventClicked);
    }

    private static IEnumerator AwaitPlayerDistance(Player player, Enemy enemy)
    {
        bool Done = false;
        Action func = () =>
        {
            if (Vector2.Distance(player.transform.position, enemy.transform.position) <= player.AttackRange)
            {
                Done = true;
            }
        };
        StaticUpdate.Updates += func;
        yield return new WaitUntil(() => !(Player.PlayersTurn == true && Done == false));
        StaticUpdate.Updates -= func;

        PlayerInRange = Done;

    }

    private static IEnumerator PrintMessage(string message, string buttonText = "Next")
    {
        yield return PrintText(message);
        yield return ShowButtons(buttonText);
    }

    public static IEnumerator Tutorial()
    {
        TutorialActive = true;
        Player.AutoSelect = false;
        TutorialTransform = Game.Tutorial.GetComponent<RectTransform>();
        TutorialTransform.anchoredPosition = BasePosition;
        Pane TutorialPane = Pane.GetPane("Tutorial Screen");
        TutorialPane.gameObject.SetActive(true);
        yield return Move(ShownPosition);
        yield return PrintText("Welcome new recruit, I am sure that you are new to this, would you like a quick walkthrough?");
        yield return ShowButtons("No", "Yes");
        if (LastClickedButton == 1)
        {
            yield return EndTutorial();
            yield break;
        }
        yield return PrintMessage("Excellent. Lets begin.");
        Highlighter.HighlightWorldRegion(new Rect(Vector2.zero, new Vector2(Game.Width, Game.Height)), SpawnTile.SpawnTiles[0].transform.position.z);
        float PreviousZoom = CameraTarget.GetZoom();
        CameraTarget.SetZoom(CameraTarget.MinZ);
        yield return PrintMessage("This is the map of the network node you are trying to hack.");
        yield return PrintMessage("Your goal is to destroy all the enemy programs to claim the node as your own.");
        Highlighter.HighlightSprite(SpawnTile.SpawnTiles[0].gameObject);
        yield return PrintMessage("Before we begin, you first need to upload your programs to counter the enemy ones, via an upload block like this one.");
        yield return PrintText("To upload a program, first click on the upload block.");
        SpawnTile spawnTile = SpawnTile.SpawnTiles[0];
        QueueEvent(ref spawnTile.TutorialOnClick);
        yield return AwaitEvent(spawnTile.TutorialOnClick);
        FinishEvent(ref spawnTile.TutorialOnClick);
        Highlighter.HighlightUI(Game.ToolContainer.transform.GetChild(0).gameObject);
        yield return PrintText("Now Click on the program you want to upload from the side panel.");
        QueueEvent(ref ToolSelector.TutorialOnClick);
        yield return AwaitEvent(ToolSelector.TutorialOnClick);
        FinishEvent(ref ToolSelector.TutorialOnClick);
        Highlighter.HighlightSprite(spawnTile.gameObject);
        yield return PrintMessage("Excellent, you have now sucessfully uploaded a program.");
        Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("StartGameButton"));
        yield return PrintText("Now that we uploaded a program, the battle can now begin, press the \"Start\" button to begin.");
        QueueEvent(ref Buttons.StartButtonTutClick);
        yield return AwaitEvent(Buttons.StartButtonTutClick);
        FinishEvent(ref Buttons.StartButtonTutClick);
        Buttons.DisableAttack = true;
        Buttons.DisableFinish = true;
        Highlighter.DisableAll();
        Highlighter.HighlightSprite(Player.Players[0].gameObject);
        yield return PrintText("The game has now begun. Select your character by clicking on it to begin moving. Once selected, click on the arrows to move the character around.");
        QueueEvent(ref Arrow.TutorialEvent);
        yield return AwaitEvent(Arrow.TutorialEvent);
        FinishEvent(ref Arrow.TutorialEvent);
        Arrows.EnableArrows(false, false);
        Highlighter.HighlightUI(CharacterStats.Instance.MovesLeft.gameObject);
        yield return PrintMessage("As your program moves, the amount of moves it has left decreases, so use your moves wisely.");
        yield return PrintText("Continue moving your program.");
        Arrows.EnableArrows(true, false);
        QueueEvent(ref Arrow.TutorialEvent);
        yield return AwaitEvent(Arrow.TutorialEvent);
        FinishEvent(ref Arrow.TutorialEvent);
        Arrows.EnableArrows(false, false);
        Highlighter.HighlightSprite(Player.Players[0].Trails[1].gameObject);
        yield return PrintMessage("Also note that when your program moves, it will leave behind a trail.");
        yield return PrintMessage("Trails represent a programs health. The more trails a program has, the more damage it can withstand.");
        Highlighter.HighlightUI(CharacterStats.Instance.TrailLength.gameObject);
        yield return PrintMessage("However, programs do have a limit on how long their trails can be.");
        Highlighter.DisableAll();
        yield return PrintText("Continue moving your program until all of it's moves have been used up.");
        Arrows.EnableArrows(true, false);
        yield return new WaitUntil(() => !((Player.Players[0].MovesMax - Player.Players[0].MovesDone) != 0));
        yield return PrintMessage("Now that all of the program's moves have been used up, the program's turn can now be finished.");
        Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("FinishButton"));
        yield return PrintText("Click on the \"Finish\" button to finish the program's turn.");
        Buttons.DisableFinish = false;
        QueueEvent(ref Buttons.FinishButtonTutClick);
        yield return AwaitEvent(Buttons.FinishButtonTutClick);
        FinishEvent(ref Buttons.FinishButtonTutClick);
        Highlighter.DisableAll();
        yield return PrintMessage("Now that the program's finished, the player's turn now ends.");
        Player player = Player.Players[0];
        Enemy enemy = Enemy.Enemies[0];
        Player.AutoSelect = true;
        while (true)
        {
            yield return PrintMessage("Now it's the enemy's turn. They will now have a chance to do their moves.");
            Game.TutorialEnemyWait = false;
            Game.TutorialPlayerWait = true;
            yield return new WaitUntil(() => !(Player.PlayersTurn == false));
            Game.TutorialEnemyWait = true;
            if (Player.Players.Count == 0)
            {
                yield return EndTutorial();
                yield break;
            }
            bool Attackable = false;
            if (Vector2.Distance(player.transform.position, enemy.transform.position) > player.AttackRange)
            {
                yield return PrintText("Now it's your turn. Advance towards the enemy!");
                Buttons.DisableFinish = false;
                Game.TutorialPlayerWait = false;
                yield return AwaitPlayerDistance(player, enemy);
                if (PlayerInRange)
                {
                    Arrows.EnableArrows(false, false);
                    Buttons.DisableCancel = true;
                    Buttons.DisableFinish = true;
                    yield return PrintMessage("You are now close enough to the enemy to attack it.");
                    Attackable = true;
                }
            }
            else
            {
                Arrows.EnableArrows(false, false);
                Buttons.DisableCancel = true;
                Buttons.DisableFinish = true;
                yield return PrintMessage("Now it's your turn and you are now close enough to the enemy to attack it.");
                Attackable = true;
            }
            if (Attackable)
            {
                Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("AttackButton"));
                yield return PrintText("Click on the \"Attack\" Button to initiate an attack.");
                Buttons.DisableAttack = false;
                Target.TargetEnabled = false;
                QueueEvent(ref Buttons.AttackButtonTutClick);
                yield return AwaitEvent(Buttons.AttackButtonTutClick);
                FinishEvent(ref Buttons.AttackButtonTutClick);
                Highlighter.HighlightSprite(Enemy.Enemies[0].gameObject);
                yield return PrintText("Now Click on the target with the enemy over it to attack it, the player will deal damage to it.");
                Target.TargetEnabled = true;
                Game.TutorialEnemyWait = true;
                QueueEvent(ref Target.TutorialTargetEvent);
                yield return AwaitEvent(Target.TutorialTargetEvent);
                FinishEvent(ref Target.TutorialTargetEvent);
                Target.TargetEnabled = false;
                yield return new WaitUntil(() => !Player.PlayersTurn);
                yield return PrintMessage("Excellent, the enemy has now been damaged.");
                yield return PrintMessage("Notice that the amount of trails the enemy has has gone down.");
                yield return PrintMessage("Once the enemy has no more trails, it takes just one more hit and then it's destroyed.");
                yield return PrintMessage("Repeat this process until the enemy has been destroyed.");
                break;

            }
        }
        Highlighter.DisableAll();
        Game.TutorialEnemyWait = false;
        Game.TutorialPlayerWait = false;
        Buttons.DisableAttack = false;
        Game.WaitWin = true;
        Buttons.DisableCancel = false;
        Buttons.DisableFinish = false;
        Target.TargetEnabled = true;
        yield return new WaitUntil(() => !(Enemy.Enemies.Count != 0 && Player.Players.Count != 0));
        if (Player.Players.Count == 0)
        {
            yield return EndTutorial();
            yield break;
        }
        else
        {
            yield return PrintMessage("Congrats, you have conquered the node!");
            yield return PrintMessage("This concludes the tutorial, thanks for your time, and good luck.");
            Game.WaitWin = false;
            yield return EndTutorial();
            yield break;
        }
    }

    private static IEnumerator EndTutorial()
    {
        Pane TutorialPane = Pane.GetPane("Tutorial Screen");
        yield return Move(BasePosition);
        TutorialPane.gameObject.SetActive(false);
        TutorialActive = false;
        Game.TutorialEnemyWait = true;
        Player.AutoSelect = true;
        Game.TutorialPlayerWait = false;
        Buttons.DisableAttack = false;
        Game.WaitWin = false;
        Buttons.DisableCancel = false;
        Buttons.DisableFinish = false;
        Target.TargetEnabled = true;
    }
}
