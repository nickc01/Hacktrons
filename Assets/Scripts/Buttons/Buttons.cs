using System;
using UnityEngine;

public static class Buttons
{
    public static Action StartButtonTutClick;
    public static Action FinishButtonTutClick;
    public static Action AttackButtonTutClick;
    public static bool DisableCancel = false;
    public static bool DisableAttack = false;
    public static bool DisableFinish = false;
    public static void PlayButton()
    {
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        GlobalRoutine.Start(Pane.SwitchTo("Main Menu", "Select Level"));
    }
    public static void BackMainMenuButton()
    {
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        GlobalRoutine.Start(Pane.SwitchBackTo("Select Level", "Main Menu"));
    }

    public static void StartGameButton()
    {
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
                GlobalRoutine.Start(Game.StartGame());
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
            GlobalRoutine.Start(Player.ActivePlayer.InitiateAttack());
        }
    }

    //Called when the player wants to finish the turn
    public static void FinishTurnButton()
    {
        if (FinishButtonTutClick != null)
        {
            FinishButtonTutClick?.Invoke();
            FinishButtonTutClick = null;
        }
        if (DisableFinish == false)
        {
            Player.ActivePlayer?.FinishTurn();
        }
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
        GlobalRoutine.Start(Game.ResetToSelectionScreen());
    }

    public static void QuitButton()
    {
        Game.PrimaryAudio.PlayOneShot(Sounds.ButtonSound);
        Application.Quit();
    }
}
