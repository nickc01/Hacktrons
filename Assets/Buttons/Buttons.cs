using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//namespace Game
//{
public static class Buttons
{

    //public static bool StartButtonAFT = false;
    public static Action StartButtonTutClick;
    public static Action FinishButtonTutClick;
    public static Action AttackButtonTutClick;
    public static bool DisableCancel = false;
    public static bool DisableAttack = false;
    public static bool DisableFinish = false;
    public static async void PlayButton()
    {
        //await CanvasController.MovePanes("Main Menu", "Select Level");
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        await Pane.SwitchTo("Main Menu", "Select Level");
    }
    public static async void BackMainMenuButton()
    {
        //await CanvasController.MovePanes("Select Level", "Main Menu");
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        await Pane.SwitchBackTo("Select Level", "Main Menu");
    }

    public static void StartGameButton()
    {
        //GameObject.FindGameObjectWithTag("Canvas").SetActive(false);
        //ScreenCapture.CaptureScreenshot("LevelScreenShot",2);
        if (!TutorialRoutine.TutorialActive || (TutorialRoutine.TutorialActive && StartButtonTutClick != null))
        {
            if (Player.Players.Count > 0)
            {
                if (StartButtonTutClick != null)
                {
                    StartButtonTutClick?.Invoke();
                    StartButtonTutClick = null;
                }
                Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
                Game.StartGame();
            }
        }
    }
    //Called when the player wishes to attack
    public static void AttackButton()
    {
        if (Player.ActivePlayer != null && DisableAttack == false)
        {
            if (AttackButtonTutClick != null)
            {
                AttackButtonTutClick?.Invoke();
                AttackButtonTutClick = null;
            }
            Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
            Player.ActivePlayer.InitiateAttack();
        }
    }

    //Called when the player wants to finish the turn
    public static void FinishTurnButton()
    {
        //if (!TutorialRoutine.TutorialActive || (TutorialRoutine.TutorialActive && FinishButtonTutClick != null))
        //{
        if (FinishButtonTutClick != null)
        {
            FinishButtonTutClick?.Invoke();
            FinishButtonTutClick = null;
        }
        if (DisableFinish == false)
        {
            //Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
            Player.ActivePlayer?.FinishTurn();
        }
        //}
    }

    //Called when the player wants to cancel the attack
    public static void CancelAttackButton()
    {
        if (Player.ActivePlayer != null && DisableCancel == false)
        {
            Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
            Player.ActivePlayer.CancelRequest = true;
        }
    }

    public static void BackButton()
    {
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        Game.ResetToSelectionScreen();
    }

    public static void QuitButton()
    {
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        Application.Quit();
    }
}
//}
