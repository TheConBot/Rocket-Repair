using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RocketControl : MonoBehaviour
{
    //Public Vars
    [Header("Object References")]
    public AudioSource bitSmall_audio;
    public AudioSource bitLarge_audio;
    public AudioSource thruster_audio;
    public AudioFollow audioFollow;
    public GameObject rocket;
    public GameObject background;
    public GameObject backgroundDepth;
    public ParticleSystem rocketTrail;
    public Slider gauge;
    [Space]
    [Header("Serialized Variables")]
    public int bitsCollected;
    public float rocketForce;
    public float perfectMultiplier;
    public float failureMultiplier;
    public float thrusterTime;
    public float gaugeSpeed = 1;
    public float screenWrapRespawnPos = 95;
    public float directionalInfluenceStr = 0.05f;
    //Hidden Public Vars
    [HideInInspector]
    public float maxHeightAchieved;
    [HideInInspector]
    public bool launchRocket_initial;
    [HideInInspector]
    public int totalBitsCollected;
    [HideInInspector]
    public float totalHeightAchieved;
    //Private Vars
    private BitSpawner bitSpawn;
    private Material bgMat;
    private Material bgDepthMat;
    private Rigidbody2D rb;
    private Vector3 rocket_spawn;
    private bool launchRocket_thrust;
    private bool resetting_level;
    private bool rocketFalling;
    private float maxHeightAchievedOffset;

    private void Start()
    {
        bgMat = background.GetComponent<Renderer>().sharedMaterial;
        bgDepthMat = backgroundDepth.GetComponent<Renderer>().sharedMaterial;
        bitSpawn = Camera.main.GetComponent<BitSpawner>();
        rocket_spawn = rocket.transform.position;
        maxHeightAchievedOffset = rocket.transform.position.y;
        rb = rocket.GetComponent<Rigidbody2D>();
        StartCoroutine(MoveGauge());
    }

    private void Update()
    {
        if(Mathf.Abs(transform.position.x) >= 110 && !resetting_level)
        {
            gameObject.transform.position = new Vector3(-((transform.position.x/Mathf.Abs(transform.position.x)) * screenWrapRespawnPos), transform.position.y, transform.position.z);
            Camera.main.gameObject.GetComponent<CameraFollow>().setCameraPosition();
            audioFollow.UpdatePosition();
        }
        if (Input.GetButtonDown("Fire1") && !launchRocket_initial && !UIManager.S.shop_panel.activeSelf)
        {
            launchRocket_initial = true;
            StartCoroutine(UIManager.S.RocketLaunch());
            float multiplier = 1;
            if (isBetween(gauge.value, 0, 0.45f) || isBetween(gauge.value, 0.55f, 1))
            {
                multiplier = failureMultiplier;
            }
            else if (isBetween(gauge.value, 0.49f, 0.51f))
            {
                multiplier = perfectMultiplier;
            }
            StartCoroutine(LaunchRocket(multiplier));
        }

        if((rocket.transform.position.y - maxHeightAchievedOffset) > maxHeightAchieved)
        {
            SetMaxHeight();
        }

        if(rb.velocity.magnitude != 0)
        {
            bgMat.mainTextureOffset = new Vector2(bgMat.mainTextureOffset.x + (-Time.deltaTime * (rb.velocity.x * 0.005f)), bgMat.mainTextureOffset.y + (-Time.deltaTime * (rb.velocity.y * 0.005f)));
            bgDepthMat.mainTextureOffset = new Vector2(bgDepthMat.mainTextureOffset.x + (-Time.deltaTime * (rb.velocity.x * 0.001f)), bgDepthMat.mainTextureOffset.y + (-Time.deltaTime * (rb.velocity.y * 0.001f)));
        }

        //var mouse = Input.mousePosition;
        //var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        //var offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
        //var angle = (Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg) - 90;
        var angle = transform.eulerAngles.z + (-175 * Time.deltaTime * Input.GetAxis("Horizontal")); 
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {
        //Directional Influence
        rb.AddForce(new Vector2(directionalInfluenceStr * transform.up.x, 0));
    }

    private void SetMaxHeight()
    {
        maxHeightAchieved = (float)Math.Round(rocket.transform.position.y - maxHeightAchievedOffset, 2);
    }

    private IEnumerator LaunchRocket(float multiplier)
    {
        rb.isKinematic = false;
        rocketTrail.Play();
        thruster_audio.Play();
        launchRocket_thrust = true;
        StartCoroutine(ThrusterTime());
        while (launchRocket_thrust)
        {
            rb.AddRelativeForce(new Vector2(0, rocketForce * multiplier), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
        thruster_audio.Stop();
        rocketTrail.Stop();
    }

    private IEnumerator ThrusterTime()
    {
        yield return new WaitForSeconds(thrusterTime);
        launchRocket_thrust = false;
    }

    private IEnumerator MoveGauge()
    {
        gauge.value = UnityEngine.Random.value;
        int x = UnityEngine.Random.Range(0, 2);
        bool goLeft = x == 0 ? true : false;
        while (!launchRocket_initial)
        {
            if (goLeft) gauge.value -= Time.deltaTime * gaugeSpeed;
            else gauge.value += Time.deltaTime * gaugeSpeed;

            if (gauge.value == 0) goLeft = false;
            else if (gauge.value == 1) goLeft = true;

            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Ground":
                if (!resetting_level || !launchRocket_initial)
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0;
                    rocketTrail.Stop();
                    launchRocket_thrust = false;
                    StopCoroutine("ThrusterTime");
                    StartCoroutine(ResetLevel());
                    resetting_level = true;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Bit_small":
                other.gameObject.SetActive(false);
                bitsCollected = Mathf.Clamp(bitsCollected += 1, 0, 9999);
                totalBitsCollected += 1;
                bitSmall_audio.Play();
                break;
            case "Bit_large":
                other.gameObject.SetActive(false);
                bitsCollected = Mathf.Clamp(bitsCollected += 5, 0, 9999);
                totalBitsCollected += 5;
                bitLarge_audio.Play();
                break;
            case "Wormhole":
                StartCoroutine(Wormhole(other.transform.position));
                break;
        }
        bitsCollected = Mathf.Clamp(bitsCollected, 0, 9999);
    }

    private IEnumerator Wormhole(Vector2 wormholePos)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        Vector2 newPos = new Vector2(wormholePos.x, wormholePos.y + 8);
        while (Vector2.Distance(transform.position, newPos) != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, newPos, Time.deltaTime * 6);
            yield return null;
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(UIManager.S.RoundEnd());
        yield return new WaitForSeconds(1);
        totalHeightAchieved += maxHeightAchieved;
        UIManager.S.DisplayWinScreen();
        yield return null;
    }

    private bool isBetween(float value, float x, float y)
    {
        if (value > x && value < y)
        {
            return true;
        }
        return false;
    }

    public IEnumerator ResetLevel()
    {
        totalHeightAchieved += maxHeightAchieved;
        yield return new WaitForSeconds(1.5f);
        maxHeightAchieved = Mathf.RoundToInt(maxHeightAchieved);
        int bonus = 0;
        while(maxHeightAchieved > 0)
        {
            maxHeightAchieved = Mathf.Clamp(maxHeightAchieved -= 10, 0, Mathf.Infinity);
            bitSmall_audio.Play();
            bitsCollected = Mathf.Clamp(bitsCollected += 1, 0, 9999);
            totalBitsCollected += 1;
            if(bonus >= 10)
            {
                bonus = 0;
                bitsCollected = Mathf.Clamp(bitsCollected += 5, 0, 9999);
                totalBitsCollected += 5;
                bitLarge_audio.Play();
            }
            else
            {
                bonus++;
            }
            yield return null;
        }
        maxHeightAchieved = 0;
        yield return new WaitForSeconds(1f);
        StartCoroutine(UIManager.S.RoundEnd());
        yield return new WaitForSeconds(1f);
        bitSpawn.DespawnBits();
        bitSpawn.SpawnBits();
        rocket.transform.position = rocket_spawn;
        rocket.transform.rotation = Quaternion.identity;
        UIManager.S.roundStart_panel.alpha = 1;
        UIManager.S.roundStart_panel.interactable = true;
        launchRocket_initial = false;
        StartCoroutine(MoveGauge());
        StartCoroutine(UIManager.S.RoundStart());
        resetting_level = false;
        yield return null;
    }
}
