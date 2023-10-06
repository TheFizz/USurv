using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Material mat;
    public GameObject camPivot;

    public GameObject top, mid, bot;
    public GameObject trail;
    public Light Light;

    public TextAsset DeathLines;
    private GameObject deathUI;

    private List<Vector3> _cutPoints = new List<Vector3>()
    {
        new Vector3(-0.79967f,-99.9893f,-0.5f),
        new Vector3(0.7998f, -99.7678f, -0.5f),
        new Vector3(-0.33582f, -100.4502f, -0.5f)
    };

    private List<Vector3> _partsMove = new List<Vector3>()
    {
        new Vector3(0,0.415f,0),
        new Vector3(-0.268f, -0.218f,0),
        new Vector3(0.276f, -0.317f,0)
    };
    public void Play()
    {
        StartCoroutine(RecordFrame());
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        mat.mainTexture = texture;
        MoveCam();
    }

    void MoveCam()
    {
        deathUI = Instantiate(Game.Instance.DeathUiPrefab, Game.GameUI.transform.parent);
        var DeathText = deathUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        camPivot = Game.MainCamera.transform.parent.gameObject;
        var lines = DeathLines.text.Split('\n');
        var line = lines[Random.Range(0, lines.Length)];
        //line = lines[8];
        Game.GameUI.SetActive(false);
        DeathText.text = line;
        camPivot.GetComponent<CameraFollow>().enabled = false;
        camPivot.transform.position = new Vector3(0, -100, 0);
        camPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        Game.MainCamera.orthographicSize = 0.45f; // Pure magic. 
        //Light.intensity = 0;
        Animate();
    }
    void Animate()
    {
        Sequence trailSeq = DOTween.Sequence();
        trailSeq.SetUpdate(true);
        trailSeq.AppendInterval(.5f);
        trailSeq.Append(trail.transform.DOMove(_cutPoints[1], 0.05f));
        trailSeq.AppendInterval(.1f);
        trailSeq.Append(trail.transform.DOMove(_cutPoints[2], 0.05f));
        trailSeq.AppendInterval(.2f);
        trailSeq.AppendCallback(StopGame);
        trailSeq.AppendInterval(.5f);
        trailSeq.Join(top.transform.DOLocalMove(_partsMove[0], 0.5f).SetEase(Ease.OutBounce).SetUpdate(true));
        trailSeq.Join(mid.transform.DOLocalMove(_partsMove[1], 0.5f).SetEase(Ease.OutBounce).SetUpdate(true));
        trailSeq.Join(bot.transform.DOLocalMove(_partsMove[2], 0.5f).SetEase(Ease.OutBounce).SetUpdate(true));
        trailSeq.AppendCallback(() =>
                {
                    trailSeq.Kill(true);
                    Destroy(trail);
                    deathUI.SetActive(true);
                    End();
                }
                );
        trailSeq.Play();

    }

    private void End()
    {

        var buttons = new List<Button>(deathUI.GetComponentsInChildren<Button>());
        buttons.Find(b => b.name == "RetryBTN").onClick.AddListener(() => SceneSwitcher.Instance.RestartGame());
        buttons.Find(b => b.name == "QuitBTN").onClick.AddListener(() => Game.Room.QuitGame());

    }

    void StopGame()
    {
        Time.timeScale = 0;
        Game.InputHandler.SetInputEnabled(false);
    }
}