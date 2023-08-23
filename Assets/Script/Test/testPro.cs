using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class testPro : MonoBehaviour
{
    // Start is called before the first frame update
    public int countDown = 0;

    public Transform par;

    public SkeletonGraphic skin;


    void Start()
    {
        /*GameObject sp = Instantiate(skin.gameObject);
        sp.name="lady";
        sp.transform.SetParent(par);
        var skele=sp.GetComponent<SkeletonGraphic>();
        skele.startingAnimation="action";
        skele.SetPlayAnimation();*/

        // StartCoroutine(startRun());

    }

    public IEnumerator startRun()
    {

        Debug.Log("執行開始");

        StartCoroutine(testRun());
        yield return StartCoroutine(testRun2());

    }



    public IEnumerator testRun()
    {

        while (countDown < 5)
        {
            countDown++;
            Debug.Log("現在的數字==>" + countDown);
            yield return new WaitForSeconds(0.3f);
        }


    }


    public IEnumerator testRun2()
    {

        Debug.Log("結束");
        yield return null;
    }

    public void OnGUI()
    {


        // Debug.Log("測試==>" + Event.current.mousePosition);
    }
}
