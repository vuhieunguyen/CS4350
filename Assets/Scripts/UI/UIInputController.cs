﻿using System;
using UnityEngine;

namespace UI
{
    public class UIInputController : MonoBehaviour
    {
        public PauseMenu pauseMenu;
        private void Update()
        {
            if (Input.GetButtonDown("CraftMenu"))
            {
                CraftMenuManager.Instance.DisplayCraftMenu();
            }

            if (Input.GetButtonDown("Craft") && CraftMenuManager.Instance.IsCraftMenuDisplayed())
            {
                CraftMenuManager.Instance.Craft();
            }
            
            if (Input.GetButtonDown("PauseGame"))
            {
                // todo make pausemenu a singleton?
                if (PauseMenu.isPaused)
                {
                    pauseMenu.ResumeGame();
                }
                else
                {
                    pauseMenu.PauseGame();
                }
            }
        }
    }
}