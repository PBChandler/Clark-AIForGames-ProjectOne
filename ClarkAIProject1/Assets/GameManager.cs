using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<SpawnableCube> cubes;
    public static bool playerAlreadyHoldingCube;
    public Transform playerHoldPoint;
    public TextMeshProUGUI text_CubeCount, text_ChainCount, text_messageHerald;
    public int chain;
    public int CubeCount
    {
        get
        {
            return trueCubeCount;
        }
        set
        {
            trueCubeCount = value;
            text_CubeCount.text = "Button Presses: <br>" + trueCubeCount + "/20 ";
        }
    }
    private int trueCubeCount;
    public void Start()
    {
        instance = this;//dont need to worry about doubling up, can be lazy
    }

    public void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    foreach(SpawnableCube cube in cubes)
        //    {
        //       // if(cube.myCubeType == SpawnableCube.CubeType.RED)
        //       // {
        //       //     cube.GetComponent<Rigidbody>().AddExplosionForce(5330f, cube.transform.position, 20f);
        //       // }
        //    }
        //}
    }

    public static void iCheckStacks()
    {
        instance.CheckStacks();
    }
    public void CheckStacks()
    {
        foreach(SpawnableCube c in cubes)
        {
            if(c.chain.Count < 3)
            {
                c.Stackmessage.SetActive(false);
            }
            if(c.chain.Count < 20)
            {
                c.fullstackmessage.SetActive(false);
            }
            if (c.chain.Count > chain)
            {
                chain = c.chain.Count;
                text_ChainCount.text = "Biggest Chain: " + chain;
            }
            if(chain >= 3)
            {
                text_messageHerald.text = "Awesome Cube Co. Has reported a minor uptick in sales after it was proven 3 cubes could be stacked ontop of eachother.";
            }
            if(chain >= 5)
            {
                text_messageHerald.text = "Awesome Cube Co. Has reported strange noises beginning as they stacked 5 cubes atop eachother. Appears to be basic drumloop.";
            }
            if(chain >= 10)
            {
                text_messageHerald.text = "Awesome Cube Co. Has reported even more strange noises now, some kind of low rumble...";
            }
            if(chain >= 15)
            {
                text_messageHerald.text = "Awesome Cube Co. Was probably jumpscared by the chord coming in";
            }
            if(chain >= 20)
            {
                text_messageHerald.text = "Awesome Cube Co. Has reported a MAJOR uptick in sales after it was proven 20 cubes could be stacked, also weird scary strings are playing";
            }
        }
    }

}
