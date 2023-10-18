using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimations : MonoBehaviour
{
    public Animator GateAnimator;
    public GameObject PortalPrefab;
    private GameObject portalObj;
    public bool isPortalSpawned = false;
    private bool SeqFinished = false;
    private bool crStarted = false;
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
        if (name == "Gate_Open" && !isPortalSpawned && !SeqFinished)
            SpawnPortal();
        if (portalAnim == null)
            return;

        info = portalAnim.GetCurrentAnimatorClipInfo(0);
        name = info[0].clip.name;
        Debug.Log(name);
        if (isPortalSpawned && name == "Portal_Open")
            StartCoroutine(PlayerActivationCR(activationDelay));
        if (isPortalSpawned && name == "Portal_Closed")
        {
            PlayerSystems.Instance.SetPlayerLocked(false);
            isPortalSpawned = false;
            SeqFinished = true;
            Destroy(portalObj);
        }
    }

    private void SpawnPortal()
    {
        Debug.Log("Spawned portal");
        isPortalSpawned = true;
        portalObj = Instantiate(PortalPrefab, transform.position, Quaternion.identity);
        portalAnim = portalObj.GetComponentInChildren<Animator>();
    }
    IEnumerator PlayerActivationCR(float delay)
    {
        if (!crStarted)
        {
            yield return new WaitForSeconds(delay);
            PlayerSystems.Instance.SetPlayerActive(true);
            DespawnPortal();
        }
        crStarted = true;
    }

    private void DespawnPortal()
    {
        portalAnim.SetTrigger("PortalClose");
    }
}
