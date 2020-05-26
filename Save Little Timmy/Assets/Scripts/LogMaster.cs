using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMaster
{
    string longLog = "";
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
        Debug.Log("-------------------------\n" + 
            "Total number of Lines: " + numberOfLines + "\n" +
            longLog +
            "-------------------------\n");
    }

    public void Clear() {
        longLog = "";
        numberOfLines = 3;
    }

    public static void QuickLog<T>(string log, T value) {
        Debug.Log(log + ": " + value);
    }
}


