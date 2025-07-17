using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shadow : MonoBehaviour
{
    private Vector3 startPos;
    public float moveRadius = 0.5f; 
    public float moveSpeed = 1f;    

    void Start()
    {
        startPos = transform.position;
        // ≈сли вдруг Light уже присутствует, удал€ем его (на вс€кий случай)
        if (GetComponent<Light>() != null)
            Destroy(GetComponent<Light>());
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            Vector3 offset = new Vector3(Random.Range(-moveRadius, moveRadius), Random.Range(-moveRadius, moveRadius), 0);
            Vector3 targetPos = startPos + offset;
            float journey = 0f;
            Vector3 initialPos = transform.position;
            while (journey < 1f)
            {
                journey += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(initialPos, targetPos, journey);
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    public void LightUp()
    {
        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.color = Color.cyan;
            light.enabled = true; 
        }
        else
        {
            Debug.LogWarning(gameObject.name + " не имеет Light компонента!");
        }
    }

    public void TurnOff()
    {
        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.color = Color.white; 
        }
    }

    public void Mistake()
    {
        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.color = Color.red; 
        }
    }

    public void Correct()
    {
        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.color = Color.green; 
        }
    }
}
