using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Classes;

public class AppearanceScript : MonoBehaviour {

    public Sprite[] hairStyles;     // the images to use for hair
    
    public GameObject staffMember;      // the game object for the staff member

    public GameObject popup;        // popup box
    Text popupMessage;      // the text part of the popup
    public InputField txtName;        // the name textbox

    // the various components / body parts of the staff member
    static SpriteRenderer head;
    static SpriteRenderer hands;
    static SpriteRenderer body;
    static SpriteRenderer hair;

    // the lists of colours to use for each component
    Color[] shirtColours = new Color[6];
    Color[] hairColours = new Color[4];
    Color[] skinTones = new Color[3];

    static AppearanceScript current;

    // details to pass back to other scene
    public static Sprite hairStyle;     // the sprite for hair
    public static Color[] colours = new Color[3];   // the colours chosen for each component
    static bool isNew = false;   // whether or not the staff member is new

    int newCount = 2;       // if a new game, count how many are required

    public Controller mainController;      // the instance of Controller to use

    void Start()
    {
        current = this;

        // get the Sprite Renderer for each component
        body = staffMember.GetComponent<SpriteRenderer>();
        SpriteRenderer[] srs = staffMember.GetComponentsInChildren<SpriteRenderer>();
        head = srs[1];
        hands = srs[2];
        hair = srs[3];

        Text[] txts = popup.GetComponentsInChildren<Text>();
        popupMessage = txts[1];

        #region initialise colours
        // shirt colours
        shirtColours[0] = new Color(0.631f, 0, 0);
        shirtColours[1] = new Color(0.141f, 0.216f, 0.486f);
        shirtColours[2] = new Color(0.094f, 0.722f, 0);
        shirtColours[3] = new Color(1, 0.878f, 0);
        shirtColours[4] = new Color(0.514f, 0, 0.329f);
        shirtColours[5] = new Color(1, 1, 1);

        // hair colours
        hairColours[0] = new Color(0.427f, 0.251f, 0.133f);
        hairColours[1] = new Color(1, 1, 0.580f);
        hairColours[2] = new Color(0, 0, 0);
        hairColours[3] = new Color(1, 0.443f, 0.094f);

        // skin tones
        skinTones[0] = new Color(1, 0.886f, 0.808f);
        skinTones[1] = new Color(0.855f, 0.549f, 0.337f);
        skinTones[2] = new Color(0.361f, 0.216f, 0.118f);

        // set the default values
        colours[0] = shirtColours[0];
        colours[1] = hairColours[0];
        colours[2] = skinTones[0];
        #endregion
    }

    /// <summary>
    /// set the initial values
    /// </summary>
    /// <param name="isN"></param>
    public static void Initialise(bool isN, Color[] col, int needed)
    {
        isNew = isN;
        if (col != null)
        {
            colours = col;
        }

        AppearanceScript.current.Reset();
    }

    /// <summary>
    /// When the user changes the hair style
    /// </summary>
    /// <param name="index">The index of the new hair style</param>
    public void HairStyleChanged(int index)
    {
        // bald check   
        if (index != 4)
        {
            hair.sprite = hairStyles[index];
        }
        else
        {
            hair.sprite = null;
        }
    }

    /// <summary>
    /// When the user changes the hair colour
    /// </summary>
    /// <param name="index">The index of the new hair colour</param>
    public void HairColourChanged(int index)
    {
        hair.color = hairColours[index];
    }

    /// <summary>
    /// When the user changes the shirt colour
    /// </summary>
    /// <param name="index">The index of the new shirt colour</param>
    public void ShirtStyleChanged(int index)
    {
        body.color = shirtColours[index];
    }

    /// <summary>
    /// When the user changes the skin tone
    /// </summary>
    /// <param name="index">The index of the new skin tone</param>
    public void SkinToneChanged(int index)
    {
        head.color = skinTones[index];
        hands.color = skinTones[index];
    }

    /// <summary>
    /// The user chooses that they are finished
    /// </summary>
    public void Finished()
    {
        colours[0] = body.color;
        colours[1] = hair.color;
        colours[2] = head.color;
        hairStyle = hair.sprite;

        string name = txtName.text;

        // check the length of the name
        if (name.Length < 3)
        {
            popupMessage.text = "Name must be at least 3 characters long";
            popup.SetActive(true);
        }
        else
        {
            // perform call back to delegate on main Controller

            int id = mainController.staffMembers.Count;
            
            mainController.AddStaffMember(name);

            newCount--;

            if (newCount > 0)
            {
                popupMessage.text = "Great! " + name + " looks happy to be here! Now let's hire someone else to help them out!";
                popup.SetActive(true);
                Reset();
            }
            else
            {
                // hide modeller
                mainController.staffModel.SetActive(false);
                // close menu 
                mainController.staffAppearanceMenu.SetActive(false);
                // show all staff
                mainController.ReShowStaffAndBuildings();

                mainController.statusCode = 0;
            }

        }
    }

    /// <summary>
    /// Hide the popup message
    /// </summary>
    public void ClosePopup()
    {
        popup.SetActive(false);
    }

    /// <summary>
    /// Reset the values and images to the original states
    /// </summary>
    void Reset()
    {
        txtName.text = "";

        body.color = shirtColours[0];
        head.color = skinTones[0];
        hair.color = hairColours[0];
        hair.sprite = hairStyles[0];
        hands.color = skinTones[0];

        colours[0] = skinTones[0];
        colours[1] = hairColours[0];
        colours[2] = shirtColours[0];

    }

}
