using UnityEngine;
using System.Collections;

public class AppearanceScript : MonoBehaviour {

    public Sprite[] hairStyles;

    public GameObject staffMember;
    public SpriteRenderer head;
    public SpriteRenderer hands;
    public SpriteRenderer body;
    public SpriteRenderer hair;

    Color[] shirtColours = new Color[6];
    Color[] hairColours = new Color[3];
    Color[] skinTones = new Color[3];
    
    public static string name = "";
    public static Sprite hairStyle;
    public static Color[] colours = new Color[3];


    void Start()
    {
        body = staffMember.GetComponent<SpriteRenderer>();

        SpriteRenderer[] srs = staffMember.GetComponentsInChildren<SpriteRenderer>();

        head = srs[1];
        hands = srs[2];
        hair = srs[3];

        shirtColours[0] = new Color(0.631f, 0, 0);
        shirtColours[1] = new Color(0.141f, 0.216f, 0.486f);
        shirtColours[2] = new Color(0.094f, 0.722f, 0);
        shirtColours[3] = new Color(1, 0.878f, 0);
        shirtColours[4] = new Color(0.514f, 0, 0.329f);
        shirtColours[5] = new Color(1, 1, 1);

        hairColours[0] = new Color(0.427f, 0.251f, 0.133f);
        hairColours[1] = new Color(1, 1, 0.580f);
        hairColours[2] = new Color(0, 0, 0);

        skinTones[0] = new Color(1, 0.886f, 0.808f);
        skinTones[1] = new Color(0.855f, 0.549f, 0.337f);
        skinTones[2] = new Color(0.361f, 0.216f, 0.118f);


        colours[0] = shirtColours[0];
        colours[1] = hairColours[0];
        colours[2] = skinTones[0];

    }

    public void HairStyleChanged(int index)
    {
        
        if (index != 3)
        {
            hair.sprite = hairStyles[index];
        }
        else
        {
            hair.sprite = null;
        }
    }

    public void HairColourChanged(int index)
    {
        colours[1] = hairColours[index];
        hair.color = hairColours[index];
    }

    public void ShirtStyleChanged(int index)
    {
        colours[0] = shirtColours[index];
        body.color = shirtColours[index];
    }

    public void SkinToneChanged(int index)
    {
        colours[2] = skinTones[index];
        head.color = skinTones[index];
        hands.color = skinTones[index];
    }

}
