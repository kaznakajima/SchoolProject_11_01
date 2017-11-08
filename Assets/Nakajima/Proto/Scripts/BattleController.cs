using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static Map_Unit attacker;
    public static Map_Unit defender;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);

        defender.Damage(attacker,defender);

        yield return new WaitForSeconds(1.0f);

        if(defender.Life <= 0)
        {
            defender.DestroyAnimate();
        }

        Destroy(gameObject);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
