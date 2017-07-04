using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform playerPosition;
    public float CamToLadderTime;

    private Vector3 offset;
    private bool showingLadder = false;

	void Start () {
        offset = transform.position - playerPosition.position;
	}
	
	void LateUpdate () {
        if (!GameController.instance.movingCam) {
            transform.position = playerPosition.position + offset;
        } else {
            //transform.position = GameObject.FindGameObjectWithTag("Ladder").transform.position + offset;
        }
	}

    public void MoveCamToLadder () {
        GameController.instance.movingCam = true;
        StartCoroutine(MoveCam());
    }

    IEnumerator MoveCam() {
        Vector3 targetPos = GameObject.FindGameObjectWithTag("Ladder").transform.position + offset;
        float remainingDistance = (transform.position - targetPos).magnitude;
        while (remainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPos, (CamToLadderTime * Time.deltaTime));
            transform.position = newPosition;
            remainingDistance = (transform.position - targetPos).magnitude;
            yield return null;
        }

        targetPos = GameObject.FindGameObjectWithTag("Player").transform.position + offset;
        remainingDistance = (transform.position - targetPos).magnitude;
        while (remainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPos, (CamToLadderTime * Time.deltaTime));
            transform.position = newPosition;
            remainingDistance = (transform.position - targetPos).magnitude;
            yield return null;
        }
        //   transform.position = GameObject.FindGameObjectWithTag("Ladder").transform.position + offset;
        // yield return new WaitForSeconds(CamToLadderTime);
        GameController.instance.movingCam = false;
    }

}
