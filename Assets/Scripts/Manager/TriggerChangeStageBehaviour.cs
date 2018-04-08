using FPSDemo.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeStageBehaviour : MonoBehaviour {

    [SerializeField]
    int stageIndex = 0;

    private Collider collider;

	void Start () {
        this.collider = this.GetComponent<Collider>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LevelManager.Manager.ActivateTeleportsWithStageIndex(stageIndex);
            SoundtrackManager.Manager.ChangeAmbientMusic();
            Destroy(this.gameObject, 1f);
        }
    }


}
