using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Glowing Balls")]
    public GameObject glowingBallPrefab;
    public float ballLifetime = 20f;
    public float rechargeTime = 30f;

    [Header("UI")]
    public List<Image> chargeIcons;   

    void Start()
    {
        foreach (var icon in chargeIcons)
            icon.fillAmount = 1f;
    }

    void Update()
    {
        HandleMovement();
        HandleDrop();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    private void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int idx = chargeIcons.FindIndex(icon => icon.fillAmount >= 1f);
            if (idx >= 0)
            {
                GameObject ball = Instantiate(glowingBallPrefab, transform.position, Quaternion.identity);
                Destroy(ball, ballLifetime);

                chargeIcons[idx].fillAmount = 0f;
                StartCoroutine(RechargeIcon(idx));
            }
        }
    }

    private IEnumerator RechargeIcon(int index)
    {
        float elapsed = 0f;
        Image icon = chargeIcons[index];

        while (elapsed < rechargeTime)
        {
            elapsed += Time.deltaTime;
            icon.fillAmount = Mathf.Clamp01(elapsed / rechargeTime);
            yield return null;
        }

        icon.fillAmount = 1f;
    }
}
