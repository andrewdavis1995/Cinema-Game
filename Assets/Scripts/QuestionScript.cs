using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Classes;
using System;

public class QuestionScript : MonoBehaviour
{
    public Transform question;     // the prefab of the projector icon (clickable)
    public bool runGeneration;      // whether or not the generation / creation loop should run
    StaffMember sm;     // the staff member associated with the question
    public bool isButton = false;
    int dayNum;


    int GetQuestionFrequency(int days)
    {
        if (days == 0)
        {
            return 2000;
        }
        else if (days < 3)
        {
            return 4500;
        }
        else if (days < 5)
        {
            return 9000;
        }

        return 30000;
    }

    /// <summary>
    /// Run the loop
    /// </summary>
    void FixedUpdate()
    {
        if (!isButton) {
            // if the loop should run...
            if (runGeneration)
            {
                // if there isn't already a question for the that staff member
                if (sm.GetClicksRemaining() < 1)
                {
                    // get how many days they have worked for 
                    // (the longer they have worked, the less likely they are to have questions)
                    int daysWorked = dayNum - sm.GetStartDay();

                    // get an upper bound based on the number of days worked
                    int upperRange = GetQuestionFrequency(daysWorked);

                    // generate a random number from 0 to upper bound
                    int randomValue = UnityEngine.Random.Range(0, upperRange);

                    // if that number is 0, break the screen
                    if (randomValue == 0)
                    {
                        sm.QuestionRaised();
                        CreateNew();
                    }
                }
                else
                {
                    sm.UpdateCount();
                }
            }
        }
    }

    /// <summary>
    /// Initialise
    /// </summary>
    /// <param name="staff">Which staff member is associated with the Question</param>
    public void Begin(StaffMember staff)
    {
        sm = staff;
        runGeneration = true;
        isButton = false;
    }

    /// <summary>
    /// End the running - destroy the question icons
    /// </summary>
    public void End()
    {
        // destroy all question icons
        // Do, or do not, there is no...
        try {
            runGeneration = false;
            GameObject[] icons = GameObject.FindGameObjectsWithTag("Question");
            foreach (GameObject go in icons)
            {
                Destroy(go);
            }
        }
        catch (Exception) { }
    }
    
    /// <summary>
    /// Create a new Question icon
    /// </summary>
    /// <param name="index">The screen to create the icon for</param>
    void CreateNew()
    {
        #region Create a projector icon

        // get the position of the associated staff member
        Vector3 position = sm.GetVector();
        position.y += 1.75f;
        position.z = -1;

        GameObject ques = Instantiate(question.gameObject, position, Quaternion.identity) as GameObject;
        QuestionScript qs = ques.GetComponent<QuestionScript>();
        QuestionScript qs2 = ques.transform.GetComponent<QuestionScript>();
        qs.isButton = true;
        qs.sm = sm;
        
        #endregion
        
    }

    /// <summary>
    /// When the mouse is released
    /// </summary>
    void OnMouseUp()
    {

        // checks that it is the icon that is clicked - not the staff member
        if (isButton)
        {
            // return the icon to its normal size
            transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }
    }

    /// <summary>
    /// When the mouse is clicked down
    /// </summary>
    void OnMouseDown()
    {
        if (isButton)
        {
            // make the icon bigger
            transform.localScale = new Vector3(1f, 1f, 1);
            
            // get the current clicks remaining for the associated staff member
            int prevClicks = sm.GetClicksRemaining();

            // update the clicks
            int remaining = sm.QuestionClicked();

            // if that was the last click... FIXED!!!
            if (prevClicks == 1)
            {
                sm.ResetClicks();
                // destroy the icon
                Destroy(gameObject);
            }
        }

    }
    
}