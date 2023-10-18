using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimations : MonoBehaviour
{
    public Animator GateAnimator;
    public GameObject PortalPrefab;
    public bool isPortalSpawned = false;

    private Animator portalAnim;
    public float activationDelay = 0.5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var info = GateAnimator.GetCurrentAnimatorClipInfo(0);
        var name = info[0].clip.name;
        if (name == "Torii_Armature|Torii_Base_No_Portal" && !isPortalSpawned)
            SpawnPortal();
        if (portalAnim == null)
            return;

        info = portalAnim.GetCurrentAnimatorClipInfo(0);
        name = info[0].clip.name;
        Debug.Log(name);
        if (isPortalSpawned && name == "Torii_Armature|Torii_Base_Portal")
            StartCoroutine(PlayerActivationCR(activationDelay));
        if (isPortalSpawned && name == "Torii_Armature|Portal_Closing")
        {
            PlayerSystems.Instance.SetPlayerLocked(false);
            isPortalSpawned = false;
        }
    }

    private void SpawnPortal()
    {
        Debug.Log("Spawned portal");
        isPortalSpawned = true;
        var portal = Instantiate(PortalPrefab, transform.position, Quaternion.identity);
        portalAnim = portal.GetComponentInChildren<Animator>();
    }
    IEnumerator PlayerActivationCR(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerSystems.Instance.SetPlayerActive(true);
        DespawnPortal();
    }

    private void DespawnPortal()
    {
        portalAnim.SetTrigger("PortalClose");
    }
}
