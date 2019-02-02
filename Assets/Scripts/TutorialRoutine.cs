using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public static class TutorialRoutine
{
    public static Task WaitWhile(Func<bool> Condition)
    {
        return Task.Run(() => { while (Condition()) { } });
    }


    public static bool TutorialActive { get; private set; } = false;
    
    private static int WaitTime = 0;
    private static Vector2 BasePosition = new Vector2(-220, -323);
    private static Vector2 ShownPosition = new Vector2(216f, -323);
    private static RectTransform TutorialTransform;
    private static Task Move(Vector2 position)
    {
        return Dragger.Drag(Game.Tutorial.gameObject, null, position, 3f, Interrupt: true);
    }

    private static async Task PrintText(string text)
    {
        char[] characters = text.ToCharArray();
        TutorialArea.TextArea = "";
        foreach (var character in characters)
        {
            TutorialArea.TextArea += character;
            //await Task.Run(() => Thread.Sleep(WaitTime));
        }
    }

    private static async Task<int> ShowButtons(string button1,string button2 = null)
    {
        int buttonClicked = 0;
        var FirstButton = TutorialArea.ButtonArea.transform.GetChild(0).GetComponent<Button>();
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
        await Task.Run(() => { while (buttonClicked == 0) { } });
        FirstButton.onClick.RemoveAllListeners();
        if (button2 != null)
        {
            SecondButton.onClick.RemoveAllListeners();
            SecondButton.gameObject.SetActive(false);
        }
        FirstButton.gameObject.SetActive(false);
        return buttonClicked;
    }

    public static void ActivateBool(ref bool value)
    {
        value = true;
    }
    public static Task AwaitEvent(ref Action eve)
    {
        var Clicked = false;
        //Buttons.StartButtonAFT = true;
        //Action temp = null;
        Action func = null;
        func = () =>
        {
            Clicked = true;
        };
        eve += func;
        //await Task.Run(() => { while (!Clicked) { } });
        return Task.Run(() => { while (!Clicked) { } });
        //eve -= func;
        //Buttons.StartButtonAFT = false;
    }

    private static async Task<bool> AwaitPlayerDistance(Player player, Enemy enemy)
    {
        //Player.PlayersTurn == true && Vector2.Distance(player.transform.position, enemy.transform.position) > player.AttackRange
        bool Done = false;
        Action func = () =>
        {
            if (Vector2.Distance(player.transform.position, enemy.transform.position) <= player.AttackRange)
            {
                Done = true;
            }
        };
        StaticUpdate.Updates += func;
        await WaitWhile(() => Player.PlayersTurn == true && Done == false);
        StaticUpdate.Updates -= func;
        if (Done)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private static async Task PrintMessage(string message, string buttonText = "Next")
    {
        await PrintText(message);
        await ShowButtons(buttonText);
    }

    public static async Task Tutorial()
    {
        try
        {
            //await AwaitClick(Arrow.TutorialEvent);
            //Highlighter.HighlightUI(Game.Tutorial.gameObject);
            //Highlighter.HighlightSprite(SpawnTile.SpawnTiles[0].gameObject);
            TutorialActive = true;
            Player.AutoSelect = false;
            TutorialTransform = Game.Tutorial.GetComponent<RectTransform>();
            TutorialTransform.anchoredPosition = BasePosition;
            Pane TutorialPane = Pane.GetPane("Tutorial Screen");
            TutorialPane.gameObject.SetActive(true);
            await Move(ShownPosition);
            await PrintText("welcome new recuit, i am sure that you are new to this, would you like a quick walkthrough");
            var buttonPressed = await ShowButtons("No", "Yes");
            if (buttonPressed == 1)
            {
                /*await Move(BasePosition);
                TutorialPane.gameObject.SetActive(false);
                TutorialActive = false;
                return;*/
                await EndTutorial();
                return;
            }
            await PrintMessage("excellent. Lets begin");
            Highlighter.HighlightWorldRegion(new Rect(Vector2.zero, new Vector2(Game.Width, Game.Height)),SpawnTile.SpawnTiles[0].transform.position.z);
            var PreviousZoom = CameraTarget.GetZoom();
            CameraTarget.SetZoom(CameraTarget.MinZ);
            await PrintMessage("This is the map of the network node you are trying to hack");
            await PrintMessage("Your goal is to destroy all the enemy programs to claim the node as your own");
            //CameraTarget.SetZoom(PreviousZoom);
            Highlighter.HighlightSprite(SpawnTile.SpawnTiles[0].gameObject);
            await PrintMessage("Before we begin, you first need to upload your programs to counter the enemy ones, via an upload block like this one");
            await PrintText("To upload a program, first click on the upload block");
            SpawnTile spawnTile = SpawnTile.SpawnTiles[0];
            await AwaitEvent(ref spawnTile.TutorialOnClick);
            Highlighter.HighlightUI(Game.ToolContainer.transform.GetChild(0).gameObject);
            await PrintText("Now Click on the program you want to upload. In this case, you can only select from one. You will be able to unlock new ones later");
            await AwaitEvent(ref ToolSelector.TutorialOnClick);
            Highlighter.HighlightSprite(spawnTile.gameObject);
            await PrintMessage("Excellent, you have now sucessfully uploaded a program");
            Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("StartGameButton"));
            await PrintText("Now that we uploaded a program, the battle can now begin, press the \"Start Battle\" button to begin");
            await AwaitEvent(ref Buttons.StartButtonTutClick);
            Buttons.DisableAttack = true;
            Buttons.DisableFinish = true;
            Highlighter.DisableAll();
            Highlighter.HighlightSprite(Player.Players[0].gameObject);
            await PrintText("The game has now begun. Select your character by clicking on it to begin moving. Once selected, click on the arrows to move the character around");
            await AwaitEvent(ref Arrow.TutorialEvent);
            Arrows.EnableArrows(false, false);
            Highlighter.HighlightUI(CharacterStats.Instance.MovesLeft.gameObject);
            await PrintMessage("As your program moves, the amount of moves it has left decreases, so use your moves wisely");
            await PrintText("Continue moving your program");
            Arrows.EnableArrows(true, false);
            await AwaitEvent(ref Arrow.TutorialEvent);
            Arrows.EnableArrows(false, false);
            Highlighter.HighlightSprite(Player.Players[0].Trails[1].gameObject);
            await PrintMessage("Also not that when your program moves, it will leave behind a trail");
            await PrintMessage("Trails represent a programs health. The more trails a program has, the more damage it can withstand");
            Highlighter.HighlightUI(CharacterStats.Instance.TrailLength.gameObject);
            await PrintMessage("However, programs do have a limit on how long their trails can be");
            Highlighter.DisableAll();
            await PrintText("Continue moving your program until all of it's moves have been used up");
            Arrows.EnableArrows(true, false);
            await Task.Run(() => { while ((Player.Players[0].MovesMax - Player.Players[0].MovesDone) != 0) { } });
            await PrintMessage("Now that all of the program's moves have been used up, the program's turn can now be finished");
            Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("FinishButton"));
            await PrintText("Click on the \"Finish\" button to finish the program's turn");
            Buttons.DisableFinish = false;
            await AwaitEvent(ref Buttons.FinishButtonTutClick);
            //Buttons.DisableFinish = true;
            Highlighter.DisableAll();
            await PrintMessage("Now that the program's finished, the player's turn now ends");
            var player = Player.Players[0];
            var enemy = Enemy.Enemies[0];
            Player.AutoSelect = true;
            while (true)
            {
                await PrintMessage("Now it's the enemy's turn. They will now have a chance to do their moves");
                Game.TutorialEnemyWait = false;
                Game.TutorialPlayerWait = true;
                await WaitWhile(() => Player.PlayersTurn == false);
                Game.TutorialEnemyWait = true;
                if (Player.Players.Count == 0)
                {
                    await EndTutorial();
                    return;
                }
                var Attackable = false;
                if (Vector2.Distance(player.transform.position,enemy.transform.position) > player.AttackRange)
                {
                    await PrintText("Now it's your turn. Advance towards the enemy!");
                    Buttons.DisableFinish = false;
                    Game.TutorialPlayerWait = false;
                    //await WaitWhile(() => Player.PlayersTurn == true && Vector2.Distance(player.transform.position, enemy.transform.position) > player.AttackRange);
                    bool InRange = await AwaitPlayerDistance(player, enemy);
                    if (InRange)
                    {
                        Arrows.EnableArrows(false, false);
                        Buttons.DisableCancel = true;
                        Buttons.DisableFinish = true;
                        await PrintMessage("You are now close enough to the enemy to attack it");
                        Attackable = true;
                    }
                }
                else
                {
                    Arrows.EnableArrows(false, false);
                    Buttons.DisableCancel = true;
                    Buttons.DisableFinish = true;
                    await PrintMessage("Now it's your turn and you are now close enough to the enemy to attack it");
                    Attackable = true;
                }
                if (Attackable)
                {
                    Highlighter.HighlightUI(GameObject.FindGameObjectWithTag("AttackButton"));
                    await PrintText("Click on the \"Attack\" Button to initiate an attack");
                    Buttons.DisableAttack = false;
                    Target.TargetEnabled = false;
                    await AwaitEvent(ref Buttons.AttackButtonTutClick);
                    Highlighter.HighlightSprite(Enemy.Enemies[0].gameObject);
                    await PrintText("Now Click on the target with the enemy over it to attack it, the player will deal damage to it");
                    Target.TargetEnabled = true;
                    Game.TutorialEnemyWait = true;
                    await AwaitEvent(ref Target.TutorialTargetEvent);
                    Target.TargetEnabled = false;
                    await WaitWhile(() => Player.PlayersTurn);
                    await PrintMessage("Excellent, the enemy has now been damaged");
                    Highlighter.HighlightSprite(Enemy.Enemies[0].Trails[0].gameObject);
                    await PrintMessage("Notice that the amount of trails the enemy has has gone down");
                    await PrintMessage("Once the enemy has no more trails, it takes just one more hit and then it's destroyed");
                    await PrintMessage("Repeat this process until the enemy has been destroyed");
                    break;

                }
                //Game.TutorialPlayerWait = false;
            }
            Game.TutorialEnemyWait = false;
            Game.TutorialPlayerWait = false;
            Buttons.DisableAttack = false;
            Game.WaitWin = true;
            Buttons.DisableCancel = false;
            Buttons.DisableFinish = false;
            Target.TargetEnabled = true;
            await WaitWhile(() => Enemy.Enemies.Count != 0 && Player.Players.Count != 0);
            if (Player.Players.Count == 0)
            {
                await EndTutorial();
                return;
            }
            else
            {
                await PrintMessage("Congrats, you have conquered the node!");
                await PrintMessage("This concludes the tutorial, thanks for your time, and good luck");
                Game.WaitWin = false;
                await EndTutorial();
                return;
            }
            //Activate finish button
            //Highlighter.HighlightUI(Game.ToolContainer.transform.GetChild(0).gameObject);
            //await PrintMessage("")
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        //await PrintMessage("")
    }

    private static async Task EndTutorial()
    {
        Pane TutorialPane = Pane.GetPane("Tutorial Screen");
        await Move(BasePosition);
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
