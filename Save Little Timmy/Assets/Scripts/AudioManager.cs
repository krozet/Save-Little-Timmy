using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Insert audio instance variables here
    public FMOD.Studio.EventInstance gameMusic;
    public FMOD.Studio.EventInstance CrazyJoe_Fire_Wince;

    public List<FMOD.Studio.EventInstance> audioInstances;
    private Transform crazyJoe;
     
    // Audio Paths
    [System.NonSerialized] public const string GAME_MUSIC_PATH = "event:/Music/Dub1";
    [System.NonSerialized] public const string CRAZYJOE_FIRE_WINCE_PATH = "event:/Crazy Joe Sound Effects/Crazy Joe.Fire.Wince";

    // Audio Indexes
    [System.NonSerialized] public const int CRAZYJOE_FIRE_WINCE_INDEX = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioInstances = new List<FMOD.Studio.EventInstance>();

        // Insert new instances
        audioInstances.Insert(CRAZYJOE_FIRE_WINCE_INDEX, CrazyJoe_Fire_Wince);

        crazyJoe = GameObject.Find("Crazy Joe").transform;
        gameMusic = FMODUnity.RuntimeManager.CreateInstance(GAME_MUSIC_PATH);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(gameMusic, crazyJoe, crazyJoe.gameObject.GetComponent<Rigidbody>());
        gameMusic.start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AMPlayOneShotAttached(int index, string path, GameObject attachedObject) {
        // check if eventinstance is null
        audioInstances[index] = FMODUnity.RuntimeManager.CreateInstance(path);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(audioInstances[index], attachedObject.transform, attachedObject.GetComponent<Rigidbody>());
        audioInstances[index].start();
        audioInstances[index].release();
    }
}
