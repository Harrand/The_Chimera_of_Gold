﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

  /**
   * The functionalities to leave the game
   * @Author Lawrence Howes-Yarlett
   */
public class QuitButtons : MonoBehaviour
{
    
    //Variables     
    public Canvas quitMenu;
    public Button start;
    public Button cont;
    public Button settings;
    public Button quit;
    public Canvas quitToMenuMenu;
    
    /**
     * Sets Quit to disabled when game starts
     * @author Lawrence Howes-Yarlett
     */
    public void Start()
    {
        if(quitMenu != null)
            quitMenu.enabled = false;
        if(quitToMenuMenu != null)
            quitToMenuMenu.enabled = false;
        if (start != null)
        {
            start.enabled = true;
        }
        if (cont != null)
        {
            cont.enabled = true;
        }
        if (settings != null)
        {
            settings.enabled = true;
        }
        if (quit != null)
        {
            quit.enabled = true;
        }
    }


    /**
     * Enables menu when quit is pressed 
     * @author Lawrence Howes-Yarlett
     */
    public void QuitPress()
    {
        quitMenu.enabled = true;
    }

    public void QuitToMenu()
    {
        quitToMenuMenu.enabled = true;
        if (start != null)
        {
            start.enabled = false;
        }
        if (cont != null)
        {
            cont.enabled = false;
        }
        if (settings != null)
        {
            settings.enabled = false;
        }
        if (quit != null)
        {
            quit.enabled = false;
        }
    }

    /**
     * YayPress() Allows you to quit the game
     * @author Lawrence Howes-Yarlett
     */
    public void YayPress()
    {
        Application.Quit();
    }

   /**
	* NayPress() Allows you to return to the main menu
	* @author Lawrnce Howes-Yarlett
	*/
    public void NayPress()
    {
        quitMenu.enabled = false;
        if (start != null)
        {
            start.enabled = true;
        }
        if (cont != null)
        {
            cont.enabled = true;
        }
        if (settings != null)
        {
            settings.enabled = true;
        }
        if (quit != null)
        {
            quit.enabled = true;
        }
    }

    /**
     * YayMenu() Allows you to quit the game to the main menu
     * @author Lawrence Howes-Yarlett
     */
    public void YayMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
	/**
	 * NayMenu() Allows you to return to the game
	 * @author Lawrence Howes-Yarlett
	 */
    public void NayMenu()
    {
        quitToMenuMenu.enabled = false;
        if (start != null)
        {
            start.enabled = true;
        }
        if (cont != null)
        {
            cont.enabled = true;
        }
        if (settings != null)
        {
            settings.enabled = true;
        }
        if (quit != null)
        {
            quit.enabled = true;
        }
    }
}
