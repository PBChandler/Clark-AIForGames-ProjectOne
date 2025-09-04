using UnityEngine;

public class Bard : MonoBehaviour
{
    public AudioSource[] trackParts;
    public CubeHome spawner;
    int lastProgress = 0;
    public float overallVolume;
    public void Start()
    {
        foreach(AudioSource a in trackParts)
        {
            a.volume = 0;
        }
        spawner.cubeCountUpdated += updateProgress;
    }

    public void updateProgress(int i)
    {
        if(i > lastProgress)
        {
            lastProgress = i;
            switch (lastProgress)
            {
                case 5:
                    trackParts[0].volume = overallVolume;
                    break;
                case 10:
                    trackParts[1].volume = overallVolume;
                    break;
                case 15:
                    trackParts[2].volume = overallVolume;
                    break;
                case 20:
                    trackParts[3].volume = overallVolume;
                    break;

                default:
                    break;
            }
        }
    }

    public void Update()
    {
        
    }
}
