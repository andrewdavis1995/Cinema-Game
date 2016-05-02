using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Classes;

public class AppearanceScript : MonoBehaviour {

    public Sprite[] hairStyles;     // the images to use for hair
    public Sprite[] extraImages;     // the images to use for extra components
    
    int staffMemberIndex = -1;      // the index of the staff member being edited
    
    public GameObject staffMember;      // the game object for the staff member

    public GameObject popup;        // popup box
    Text popupMessage;      // the text part of the popup
    public InputField txtName;        // the name textbox

    // the various components / body parts of the staff member
    static SpriteRenderer head;
    static SpriteRenderer hands;
    static SpriteRenderer body;
    static SpriteRenderer hair;
    static SpriteRenderer extras;

    // the lists of colours to use for each component
    Color[] shirtColours = new Color[6];
    Color[] hairColours = new Color[5];
    Color[] skinTones = new Color[3];

    static AppearanceScript current;

    // details to pass back to other scene
    public static Sprite hairStyle;     // the sprite for hair
    public static Sprite extraOption;     // the sprite for hair
    public static Color[] colours = new Color[3];   // the colours chosen for each component
    static bool isNew = false;   // whether or not the staff member is new

    int newCount = 1;       // if a new game, count how many are required

    int hairID = 0;         // hair sprite chosen
    int extrasID = 3;       // extras sprite chosen


    public Controller mainController;      // the instance of Controller to use

    void Start()
    {
        current = this;

        // get the Sprite Renderer for each component
        SpriteRenderer[] srs = staffMember.GetComponentsInChildren<SpriteRenderer>();
        body = srs[0];
        head = srs[1];
        hands = srs[2];
        hair = srs[3];
        extras = srs[4];

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
        hairColours[1] = new Color(0.690f, 0.690f, 0.392f);
        hairColours[2] = new Color(0, 0, 0);
        hairColours[3] = new Color(1, 0.443f, 0.094f);
        hairColours[4] = new Color(0.471f, 0.471f, 0.471f);

        // skin tones
        skinTones[0] = new Color(1, 0.804f, 0.671f);
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
    public static void Initialise(bool isN, Color[] col, int needed, Color defaultShirt, int staffIndex, Sprite hairStyle, string n, Sprite extra)
    {
        AppearanceScript.current.Reset();

        current.newCount = needed;

        current.txtName.text = n;

        current.staffMemberIndex = staffIndex;

        isNew = isN;
        if (col != null)
        {
            colours = col;
            current.LoadColours(col);
            hair.sprite = hairStyle;
            extras.sprite = extra;
        }
        
        colours[2] = defaultShirt;
        body.color = defaultShirt;

    }

    public void LoadColours(Color[] col)
    {
        colours[2] = col[2];
        colours[1] = col[1];

        hair.color = col[1];
        hands.color = col[2];
        head.color = col[2];
        extras.color = col[1];
    }

    /// <summary>
    /// When the user changes the hair style
    /// </summary>
    /// <param name="index">The index of the new hair style</param>
    public void HairStyleChanged(int index)
    {
        hairID = index;

        // bald check   
        if (index != 6)
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

        if (extras.sprite == extraImages[0] || extras.sprite == extraImages[2])
        {
            extras.color = hairColours[index];
        }
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
    /// When the user changes the option selected
    /// </summary>
    public void ExtrasChanged(int index)
    {
        extrasID = index;

        if (index != 3)
        {
            extras.sprite = extraImages[index];

            if (index != 1)
            {
                extras.color = hair.color;
            }

        }
        else
        {
            extras.sprite = null;
        }
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
        extraOption = extras.sprite;

        string name = txtName.text.Trim();
        txtName.text = name;

        // check the length of the name
        if (name.Length < 3)
        {
            popupMessage.text = "Name must be at least 3 characters long";
            popup.SetActive(true);
        }
        else
        {
            // perform call back to delegate on main Controller
            if (isNew)
            {
                int id = mainController.staffMembers.Count;

                mainController.AddStaffMember(name, hairID, extrasID);
            }
            else
            {
                mainController.StaffEditComplete(staffMemberIndex, name, hairID, extrasID);
            }

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

        staffMemberIndex = -1;

        head.color = skinTones[0];
        hair.color = hairColours[0];
        hair.sprite = hairStyles[0];
        hands.color = skinTones[0];
        extras.sprite = null;

        colours[0] = skinTones[0];
        colours[1] = hairColours[0];

        hairID = 0;
        extrasID = 3;
    }

}
