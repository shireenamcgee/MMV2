using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// menu
//
// this script is attached to the MenuManager GameObject in the menu scene
// and controls the menu behaviour

public class menu : MonoBehaviour
{

    public InputField participantId;

    // Use this for initialization
    void Start()
    {
        if (!PlayerPrefs.HasKey("sameColor"))
        {
            PlayerPrefs.SetInt("sameColor", 0);
        }
        if (!PlayerPrefs.HasKey("colorCount"))
        {
            PlayerPrefs.SetInt("colorCount", 6);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // the escape key is equivalent to android's back button
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    // Here begins the UI Eventhandling

    #region UI Eventhandling

    //
    // public void playClicked()
    //
    // This method gets called if the player clicks the play button
    // The megaMind level will be loaded
    //

    public void playClicked()
    {
        
        SetNewParticipantId().ToString();

        megaMind.GameProgress.Write(new megaMind.EventRecord() { EventName = "PLAY", });

        Application.LoadLevel("megaMind");
    }
    private const string PARTICIPANT_ID_KEY = "ParticipantId";
    private string ParticipantIdFolder;
    /// <summary>
    /// gets the participant id from the participantid.txt file
    /// Increments the Participant ID
    /// writes the new Participant id to the menu screen
    /// writes the new patticcipant id to the participntid.txt file
    /// increment
    /// </summary>
    /// <returns></returns>
    private int SetNewParticipantId()
    {
        string ParticipantIdFileName="";
        megaMind.GameProgress.ParticipantId = -1;
        try
        {
            
            DateTime RunDate = DateTime.Today;
            ParticipantIdFileName = System.IO.Path.Combine(megaMind.GameProgress.baseDirectory ,  string.Format("LastUsedParticipantId_{0}.DAT",RunDate.ToString("yyyyMMdd")));

            string sLine=null;
            var fi = new  System.IO.FileInfo(ParticipantIdFileName);
            if (fi.Exists)
            {
                using (var fPar = fi.OpenText())
                {
                    sLine = fPar.ReadLine();
                }

                if (sLine == null || sLine.Trim() == string.Empty)
                {
                    megaMind.GameProgress.ParticipantId = 1;
                }
                else
                {
                    //parse name:value,name2:value2
                    string[] asLine = sLine.Split(',');
                    foreach (string sParameter in asLine)
                    {
                        string[] asParameter = sParameter.Split(':');
                        if (asParameter[0] == PARTICIPANT_ID_KEY)
                        {
                            int i;
                            if (int.TryParse(asParameter[1], out i))
                                megaMind.GameProgress.ParticipantId = i;
                        }
                    }
                }
            }
            else
            {
                megaMind.GameProgress.ParticipantId = 1;
            }

            if(megaMind.GameProgress.ParticipantId < 1)
            {
                //error?
                //dont want to start from 0 and risk overwrites
                throw (new ArgumentException("Invalid Calculation of Participant Id.  Value should be at least 1."));
            }

            return megaMind.GameProgress.ParticipantId;
        }
        catch (Exception ex)
        {
            // if we get an exception, set the participant id to something based on the hour of day so that we do  get overlap
            // assuming max play rate is no greater than 5 games in 2 minutes;
            //create a unique daily id of that grain
            megaMind.GameProgress.ParticipantId  = DateTime.Now.Hour * 30 + DateTime.Now.Minute / 2;
            megaMind.GameProgress.Write(new megaMind.EventRecord(){ EventName="WARNING:ParticipantId Not sequential", DataKey="DETAIL" , DataValue ="There is a file tha tracks participant Id and gets incremented for each player.  something whent wrong so the Participant Id was set to (Minute Of Day /2) to ensure uniqueness" });
            //something is brokej but we dont want to crash the game just because we cannot get the id
            //so create one based on the time of day and and offset that will ensure no overlap occurs.  
            //there are 3 digits in the id so start with 500 to isolate the Exception Id's in the second bolock of 499 ids

        }
        finally
        {
            participantId.text = (++megaMind.GameProgress.ParticipantId).ToString().PadLeft(3, '0');
            //write out the new Participant Id to file
            using (var fi = System.IO.File.CreateText(ParticipantIdFileName))
            {
                fi.WriteLine("{0}:{1}", PARTICIPANT_ID_KEY, participantId.text);
                fi.Flush();
                fi.Close();
            }
        }
        return megaMind.GameProgress.ParticipantId;

    }

    public void TextChanged(string Text)
    {

    }

    //
    // public void exitClicked()
    //
    // This method gets called if the player clicks the exit button
    // The the application terminates
    //

    public void exitClicked()
    {

        megaMind.GameProgress.Write(new megaMind.EventRecord() { EventName = "Exit" });
        Application.Quit();
    }



    //
    // public void rulesClicked()
    //
    // This method gets called if the player clicks the rules button
    // The the application loads the rules level
    //

    public void rulesClicked()
    {

        megaMind.GameProgress.Write(new megaMind.EventRecord() { EventName = "Rules" });
        Application.LoadLevel("rules");
    }



    //
    // public void settingsClicked()
    //
    // This method gets called if the player clicks the settings button
    // The the application loads the settings level
    //


    public void settingsClicked()
    {
        megaMind.GameProgress.Write(new megaMind.EventRecord() { EventName = "Settings" });
        Application.LoadLevel("settings");
    }

    #endregion UI Eventhandling





}
