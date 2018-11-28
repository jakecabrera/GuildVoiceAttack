﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace GuildVoiceAttack
{
    class Program
    {
        public static dynamic prox = null;
        // private static String authSec = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt").Replace("\n", "");
        public static IFirebaseConfig config = FirebaseConf.buildFirebase();

        private static IFirebaseClient client;

        private static async Task firebaseInit()
        {
            client = new FireSharp.FirebaseClient(config);
            if (client == null) return;
            prox.WriteToLog("Connection Established", "orange");

            await client.OnAsync("Guild/BotCommands/",
                added: (s, args, d) =>
                {
                    Console.WriteLine("Added: " + args);
                },
                changed: (s, args, d) =>
                {
                    if (args.Path.Equals("/FinishMission") && args.Data.Equals("True")) finishMission();
                    if (prox == null) return;
                    prox.WriteToLog(args.OldData + " Changed to " + args.Data + " on path " + args.Path, "purple");
                },
                removed: (s, args, d) =>
                {
                    Console.WriteLine("Removed: " + args);
                });

            //var data = new Data
            //{
            //    name = "Jake",
            //    age = "22",
            //    sex = "M"
            //};
            //SetResponse response = await client.SetAsync("Information/" + data.name, data);

            //Data result = response.ResultAs<Data>();

            //Console.WriteLine("Data inserted " + result.age);
            //bar();
        }

        private static async void checkExistingCommands()
        {
            FirebaseResponse response = await client.GetAsync("Guild/BotCommands/FinishMission");
            bool needToFinishMission = response.ResultAs<bool>();

            if (needToFinishMission) finishMission();
        }

        private static async void finishMission()
        {
            if (prox == null) return;
            prox.WriteToLog("Finishing mission...", "orange");
            prox.ExecuteCommand("Finish mission");

            //SetResponse response = await client.SetAsync("Guild/BotCommands/FinishMission", false);
        }

        //NOTE THAT THIS IS A SAMPLE CLASS AND IS NOT FOR ANY MEANINGFUL USE.
        //You will want to gut this completely if you plan on using it... lol


        public static string VA_DisplayName()
        {
            return "Discord bot - v0.5";  //this is what you will want displayed in dropdowns as well as in the log file to indicate the name of your plugin
        }

        public static string VA_DisplayInfo()
        {
            return "My VoiceAttack Plugin\r\n\r\nThis is just a sample.\r\n\r\n2016 VoiceAttack";  //this is just extended info that you might want to give to the user.  note that you should format this up properly.
        }

        public static Guid VA_Id()
        {
            return new Guid("{C16BEBA9-2003-4b7f-BFA2-55B7C607925B}");  //this id must be generated by YOU... it must be unique so VoiceAttack can identify and use the plugin
        }

        static Boolean _stopVariableToMonitor = false;

        //this function is called from VoiceAttack when the 'stop all commands' button is pressed or a, 'stop all commands' action is called.  this will help you know if processing needs to stop if you have long-running code
        public static void VA_StopCommand()
        {
            _stopVariableToMonitor = true;
        }


        //note that in this version of the plugin interface, there is only a single dynamic parameter.  All the functionality of the previous parameters can be found in vaProxy
        public static async void VA_Init1(dynamic vaProxy)
        {
            //this is where you can set up whatever session information that you want.  this will only be called once on voiceattack load, and it is called asynchronously.
            //the SessionState property is a local copy of the state held on to by VoiceAttack.  In this case, the state will be a dictionary with zero items.  You can add as many items to hold on to as you want.
            //note that in this version, you can get and set the VoiceAttack variables directly.


            vaProxy.SessionState.Add("new state value", 369);  //set whatever private state information you want to maintain (note this is a dictionary of (string, object)
            vaProxy.SessionState.Add("second new state value", "hello");

            vaProxy.SetSmallInt("initializedCondition1", 1);  //set some meaningless example short integers (used to be called, 'conditions')
            vaProxy.SetSmallInt("initializedCondition2", 2);

            vaProxy.SetText("initializedText1", "This is 1");  //set some meaningless example text values
            vaProxy.SetText("initializedText2", "This is 2");

            vaProxy.SetInt("initializedInt1", 55);  //set some meaningless example integer values
            vaProxy.SetInt("initializedInt2", 77);

            vaProxy.SetDecimal("initializedDecimal1", 44.2m);  //set some meaningless example decimal values
            vaProxy.SetDecimal("initializedDecimal2", -99.9m);

            vaProxy.SetBoolean("initializedBoolean1", true);  //set some meaningless example boolean values
            vaProxy.SetBoolean("initializedBoolean2", false);

            prox = vaProxy;

            await firebaseInit();
            SetResponse response = await client.SetAsync("Guild/BotCommands/Online", true);
            checkExistingCommands();
        }

        public static async void VA_Exit1(dynamic vaProxy)
        {
            //this function gets called when VoiceAttack is closing (normally).  You would put your cleanup code in here, but be aware that your code must be robust enough to not absolutely depend on this function being called
            if (vaProxy.SessionState.ContainsKey("myStateValue"))  //the sessionstate property is a dictionary of (string, object)
            {
                SetResponse response = await client.SetAsync("Guild/BotCommands/Online", false);
                //do some kind of file cleanup or whatever at this point
            }
        }

        public static async void VA_Invoke1(dynamic vaProxy)
        {
            vaProxy.WriteToLog("Invoked", "blue");
            //This function is where you will do all of your work.  When VoiceAttack encounters an, 'Execute External Plugin Function' action, the plugin indicated will be called.
            //in previous versions, you were presented with a long list of parameters you could use.  The parameters have been consolidated in to one dynamic, 'vaProxy' parameter.

            //vaProxy.Context - a string that can be anything you want it to be.  this is passed in from the command action.  this was added to allow you to just pass a value into the plugin in a simple fashion (without having to set conditions/text values beforehand).  Convert the string to whatever type you need to.

            //vaProxy.SessionState - all values from the state maintained by VoiceAttack for this plugin.  the state allows you to maintain kind of a, 'session' within VoiceAttack.  this value is not persisted to disk and will be erased on restart. other plugins do not have access to this state (private to the plugin)

            //the SessionState dictionary is the complete state.  you can manipulate it however you want, the whole thing will be copied back and replace what VoiceAttack is holding on to


            //the following get and set the various types of variables in VoiceAttack.  note that any of these are nullable (can be null and can be set to null).  in previous versions of this interface, these were represented by a series of dictionaries

            //vaProxy.SetSmallInt and vaProxy.GetSmallInt - use to access short integer values (used to be called, 'conditions')
            //vaProxy.SetText and vaProxy.GetText - access text variable values
            //vaProxy.SetInt and vaProxy.GetInt - access integer variable values
            //vaProxy.SetDecimal and vaProxy.GetDecimal - access decimal variable values
            //vaProxy.SetBoolean and vaProxy.GetBoolean - access boolean variable values
            //vaProxy.SetDate and vaProxy.GetDate - access date/time variable values

            //to indicate to VoiceAttack that you would like a variable removed, simply set it to null.  all variables set here can be used withing VoiceAttack.
            //note that the variables are global (for now) and can be accessed by anyone, so be mindful of that while naming


            //if the, 'Execute External Plugin Function' command action has the, 'wait for return' flag set, VoiceAttack will wait until this function completes so that you may check condition values and
            //have VoiceAttack react accordingly.  otherwise, VoiceAttack fires and forgets and doesn't hang out for extra processing.


            //below is just some sample code showing how to work with vaProxy.  There's more in the VoiceAttack help documentation that is installed with VoiceAttack (VoiceAttackHelp.pdf).

            if (vaProxy.GetText("myCSharpTestValue") != null) //was the text value passed set?
            {

            }

            else //value has not been set.  set it here
            {

            }

            short? testShort = vaProxy.GetSmallInt("someValueIWantToClear");  //note that we are using short? (nullable short) in case the value is null
            if (testShort.HasValue)
            {
                vaProxy.SetSmallInt("someValueIWantToClear", null);  //setting the value to null tells VoiceAttack that you want the variable removed
            }

            bool finishedMission = vaProxy.GetBoolean("finishedMission");
            if (finishedMission)
            {
                if (client != null)
                {
                    vaProxy.WriteToLog("Finished the mission successfully.", "green");
                    SetResponse response = await client.SetAsync("Guild/BotCommands/FinishMission", false);
                } 
            }

            //here we check the context to see if we should perform an action (with some additional examples of what can be done with vaProxy
            if (vaProxy.Context == "fire weapons")
            {
                if (vaProxy.CommandExists("secret weapon"))  //check if a command exists
                {
                    if (vaProxy.ParseTokens("{ACTIVEWINDOWTITLE}") == "My Awesome Game")  //check the active window title using the parsetokens function
                    {
                        vaProxy.ExecuteCommand("secret weapon");  //this tells VoiceAttack to execute the, 'secret weapon' command by name (if the command exists
                    }
                    else
                    {
                        vaProxy.WriteToLog("Your game was not active and on top.", "purple");
                    }
                }
                else
                {
                    vaProxy.WriteToLog("the secret weapon command does not exist.  you deleted it, didn't you?", "orange");
                }
            }

            //here we are adding some stuff to state
            object objValue = null;
            String strStateValue = null;
            if (vaProxy.SessionState.TryGetValue("myStateValue", out objValue))  //we check to see if there is a value in state for 'myStateValue'
            {
                strStateValue = (String)objValue; //if we find something, we are going to cast it as a string and use it somewhere in here...
            }
            else
                strStateValue = "initialized";  //nothing was found in state... just set the string to, 'initialized' and keep moving...

            //hope that helps some!
        }
    }
}
