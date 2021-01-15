using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerCollider : MonoBehaviour
{
    [SerializeField] Banner banner;

    private bool isInRange = false;
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            isInRange = true;

            CharacterBase player = collider.GetComponent<CharacterBase>();
            if (player == null) return;
            if (player.Info.id.Equals(PlayerManager.Instance.MyId))
            {
                Debug.Log("Player has entered the ad range. request ad data...");
                banner.RequestAdData();
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            isInRange = false;

            CharacterBase player = collider.GetComponent<CharacterBase>();
            if (player == null) return;
            if (player.Info.id.Equals(PlayerManager.Instance.MyId))
            {
                Debug.Log("Player has exited the ad range.");
                banner.SetFillState(false);
            }
        }
    }

    private void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!(banner ? banner.AdRedirectLink : string.Empty).Equals(string.Empty))
                {
                    Application.OpenURL(banner.AdRedirectLink);
                }
            }
        }
    }
}
