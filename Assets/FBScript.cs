using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.MiniJSON;
using Assets.Classes;
using System.Net;
using System.IO;

public class FBScript : MonoBehaviour {
    
    //public Text txtUsername;
    //public Image picProfilePic;
    public string firstname;
    public string surname;
    public string id = "";
    
    public static FBScript current;

    public Button newGame;
    public Button loadGame;
    public Button rulesButton;

    public GameObject facebookPanel;
    public GameObject loggedInPanel;

    public Text txtLoggedInAs;

    public List<FacebookFriend> friendList = new List<FacebookFriend>();


    // Use this for initialization
    void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
        current = this;
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
        FB.LogOut();
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

        FB.LogOut();
        newGame.enabled = false;
        loadGame.enabled = false;
        rulesButton.enabled = false;
        gameObject.SetActive(false);

        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");

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
                FB.API("/me/friends?fields=first_name,last_name", HttpMethod.GET, DisplayFriends);
                //FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
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

    // http://answers.unity3d.com/questions/959943/deserialize-facebook-friends-result.html
    void DisplayFriends(IResult result)
    {
        friendList.Clear();

        current = this;

        var dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
        var theList = new List<object>();
        theList = (List<object>)(dict["data"]);

        int friendCount = theList.Count;
        
        for (int i = 0; i < friendCount; i++)
        {
            string friendFBID = GetDataValueForKey((Dictionary<string, object>)(theList[i]), "id");
            string first_name = GetDataValueForKey((Dictionary<string, object>)(theList[i]), "first_name");
            string last_name = GetDataValueForKey((Dictionary<string, object>)(theList[i]), "last_name");

            
            Debug.Log(friendFBID + " --> " + first_name + " " + last_name);

            FacebookFriend fwend = new FacebookFriend();
            fwend.name = first_name + " " + last_name;
            fwend.id = friendFBID;

            current.friendList.Add(fwend);
        }

    }

    // http://answers.unity3d.com/questions/959943/deserialize-facebook-friends-result.html
    private string GetDataValueForKey(Dictionary<string, object> dict, string key)
    {
        object objectForKey;
        if (dict.TryGetValue(key, out objectForKey))
        {
            return (string)objectForKey;
        }
        else {
            return "";
        }
    }

    void DisplayUsername(IResult result)
    {
        firstname = result.ResultDictionary["first_name"].ToString();
        surname = result.ResultDictionary["last_name"].ToString();
        id = result.ResultDictionary["id"].ToString();

        //txtUsername.text = "Welcome back " + result.ResultDictionary["first_name"].ToString() + " " + result.ResultDictionary["last_name"].ToString();

        if (firstname != null && !firstname.Equals(""))
        {
            txtLoggedInAs.text = "Logged In as: " + firstname + " " + surname;
            loggedInPanel.SetActive(true);
            facebookPanel.SetActive(false);
        }


        newGame.enabled = true;
        loadGame.enabled = true;
        rulesButton.enabled = true;
        gameObject.SetActive(true);

        // save who was logged in to a local file - to 'remember' them for next time
        Toggle remMe = GameObject.Find("rememberMe").GetComponent<Toggle>();
        
        remMe.gameObject.SetActive(false);

        ButtonScript.owner = firstname + " " + surname;

    }

    //void DisplayProfilePic(IGraphResult result)
    //{
    //    picProfilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
    //    Controller.profilePicture = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
    //}
    
    public void InviteFriends()
    {
        FB.Mobile.AppInvite(
            new System.Uri(appLink));
    }

    public void Logout()
    {
        firstname = "";
        id = "";
        facebookPanel.SetActive(true);
        loggedInPanel.SetActive(false);
    }   

}

