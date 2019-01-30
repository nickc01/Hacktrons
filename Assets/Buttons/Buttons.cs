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
        public static async void PlayButton()
        {
            //await CanvasController.MovePanes("Main Menu", "Select Level");
            await Pane.SwitchTo("Main Menu", "Select Level");
        }
        public static async void BackMainMenuButton()
        {
            //await CanvasController.MovePanes("Select Level", "Main Menu");
            await Pane.SwitchBackTo("Select Level", "Main Menu");
        }

        public static void StartGameButton()
        {
            //GameObject.FindGameObjectWithTag("Canvas").SetActive(false);
            //ScreenCapture.CaptureScreenshot("LevelScreenShot",2);
            if (Player.Players.Count > 0)
            {
                GameManager.StartGame();
            }
        }
        //Called when the player wishes to attack
        public static void AttackButton()
        {
            if (Player.ActivePlayer != null)
            {
                Player.ActivePlayer.InitiateAttack();
            }
        }

        //Called when the player wants to finish the turn
        public static void FinishTurnButton()
        {
            Player.ActivePlayer?.FinishTurn();
        }

        //Called when the player wants to cancel the attack
        public static void CancelAttackButton()
        {
            if (Player.ActivePlayer != null)
            {
                Player.ActivePlayer.CancelRequest = true;
            }
        }

        public static void BackButton()
        {
            GameManager.ResetToSelectionScreen();
        }

        public static void QuitButton()
        {
            Application.Quit();
        }
    }
//}
