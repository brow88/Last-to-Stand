using TMPro;
using UnityEngine;

public class StartCountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;

        Hide();
    }

    private void GameManager_OnGameStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsStartTimerActive())
        {
            Show();
        }
        else
        {
            Hide();
        }     
    }

    private void Update()
    {
        countDownText.text = Mathf.Ceil(GameManager.Instance.GetStartTimer()).ToString();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
