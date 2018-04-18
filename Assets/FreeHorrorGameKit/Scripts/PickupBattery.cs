using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBattery : MonoBehaviour
{
    [Header("Battery System Settings")]
    public GameObject Player;
    public float chargeValue;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(true);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You get this battery: " + collider.gameObject.name);

                // disable UI
                collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(false);

                // add battery value                
                AddBattery(collider.gameObject, chargeValue);

                // disable game object
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            // disable UI
            collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(false);
        }
    }

    void AddBattery(GameObject player, float value)
    {
        Debug.LogFormat("Battery charge value: {0}", value);
        if(player.GetComponent<PlayerBehaviour>().battery < player.GetComponent<PlayerBehaviour>().batteryMax)
            player.GetComponent<PlayerBehaviour>().battery += value;
    }

}
