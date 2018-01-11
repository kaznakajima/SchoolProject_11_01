using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static Map_Unit attacker;
    public static Map_Unit defender;
    public static GameObject mikoshi_Action;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);

        defender.Damage(attacker,defender);
        attacker.IsMoved = true;

        yield return new WaitForSeconds(1.0f);

        if(defender.Life <= 0)
        {
            defender.DestroyAnimate();
            if(defender.unitType == Map_Unit.UnitType.Mikoshi)
            {
                Debug.Log(attacker.team + "の勝利");
            }
        }

        mikoshi_Action.SetActive(false);
        attacker.gameObject.SetActive(true);
        Destroy(gameObject);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
