using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public class FBScript : MonoBehaviour {
    
    public Text txtUsername;
    public Image picProfilePic;

    // Use this for initialization
    void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("LOGGED IN ALREADY");
        }
        else
        {
            Debug.Log("NOT YET LOGGED IN");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FBLogin()
    {

        List<string> permissions = new List<string>();
        permissions.Add("public_profile");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("ERROR!!");
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("SUCCESS");

                FB.API("/me?fields=first_name,last_name", HttpMethod.GET, DisplayUsername);
                FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
                FB.GetAppLink(GetAppLink);
            }
            else
            {
                Debug.Log("FAIL");
            }
        }
    }

    string appLink;

    void GetAppLink(IAppLinkResult result)
    {
        appLink = result.Url;
    }

    void DisplayUsername(IResult result)
    {
        txtUsername.text = "Welcome back " + result.ResultDictionary["first_name"].ToString() + " " + result.ResultDictionary["last_name"].ToString() + "\n... you worthless pile of garbage";
    }

    void DisplayProfilePic(IGraphResult result)
    {
        picProfilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        Controller.profilePicture = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
    }


    public void InviteFriends()
    {
        FB.Mobile.AppInvite(
            new System.Uri(appLink));
    }


}

