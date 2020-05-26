using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 LogMaster Class is used for a quick way to log items.

    First way is to simply call QuickLog and give it a string and a value.
    Second way is to make an instance of LogMaster, then Append messages and finally call PrintLongLog.
    
    You will have to adjust how many lines you can view at once in the editor console
    (click the three dots in the console windown on the right, select Log Entry,
    and click on the number displayed in the log 'Total number of Lines: x').
     */
public class LogMaster
{
    string longLog = "";
    string separator = "-------------------------\n";
    // includes top and bottom separators, as well as 'Total number of Lines: x'
    int numberOfLines = 3;

    public LogMaster() {

    }

    public void Append<T>(string log, T value) {
        longLog += log + ": " + value.ToString() + "\n";
        numberOfLines++;
    }

    public void Append(string log) {
        longLog += log + "\n";
        numberOfLines++;
    }

    public void PrintLongLog() {
        Debug.Log(separator + 
            "Total number of Lines: " + numberOfLines + "\n" +
            longLog +
            separator);
    }

    public void Clear() {
        longLog = "";
        numberOfLines = 3;
    }

    public static void QuickLog<T>(string log, T value) {
        Debug.Log(log + ": " + value);
    }
}


