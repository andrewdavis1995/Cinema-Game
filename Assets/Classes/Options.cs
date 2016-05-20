using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    [Serializable]
    public class Options
    {
        int musicLevel;
        int fxLevel;
        bool autosave;

        public Options()
        {
            musicLevel = 4;
            fxLevel = 3;
            autosave = false; 
        }

        public void UpdateDetails(int m, int f, bool a)
        {
            if (musicLevel != m)
            {
                musicLevel = m;

                // turn the music down/up
                GameObject.Find("Music").GetComponent<AudioSource>().volume = ((float)musicLevel / 8f);
            }

            if (fxLevel != f)
            {
                fxLevel = f;

                // update the volume of any customers who have already been created
                GameObject[] customerSounds = GameObject.FindGameObjectsWithTag("Customer");
                for (int i = 0; i < customerSounds.Length; i++)
                {
                    customerSounds[i].GetComponent<AudioSource>().volume = ((float)fxLevel / 6f);
                }

                ObjectPool.current.audioVolume = fxLevel;

            }


            autosave = a;
        }

        public void Load(Options op)
        {
            UpdateDetails(op.GetMusicLevel(), op.GetFXLevel(), op.GetAutosave());
        }

        public int GetMusicLevel() { return musicLevel; }
        public int GetFXLevel() { return fxLevel; }
        public bool GetAutosave() { return autosave; }

    }
}
