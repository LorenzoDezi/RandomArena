using FPSDemo.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeStageBehaviour : MonoBehaviour {

    [SerializeField]
    int stageIndex = 0;

    [SerializeField]
    private Animation doorAnimation;

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
            PickUpManager.manager.ResetSpawnedPickUp();
            doorAnimation.Play("DoorClose");
            Destroy(this.gameObject, 1f);
        }
    }


}
