using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAnimator : MonoBehaviour
{
    Transform _myTransform;
    [SerializeField] private float _floatTo;
    // Start is called before the first frame update
    void Start()
    {
        float loopTime = Random.Range(1f, 2f);
        bool ccwRotation = Random.Range(0, 2) == 1;
        float rotationDegrees = 90;

        if (ccwRotation)
            rotationDegrees *= -1;

        _myTransform = GetComponent<Transform>();
        _myTransform.DOMoveY(_floatTo + transform.position.y, loopTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _myTransform.DOLocalRotate(new Vector3(0, rotationDegrees, 0), loopTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    public void Destroy()
    {
        DOTween.Kill(_myTransform);

        _myTransform.DOScale(0, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            //Globals.XPDropsPool.Remove(ID);
            Destroy(gameObject);
        }
        );
    }
    private void OnDestroy()
    {
        DOTween.Kill(_myTransform);
    }
}
