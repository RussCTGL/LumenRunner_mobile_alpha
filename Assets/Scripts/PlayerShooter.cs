using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera mainCamera;
    public float shootForce = 50f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 5;
    public float reloadTime = 2.5f;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("Mobile Settings")]
    public bool enableMobileControls = true;
    public bool ignoreUI = true;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public GameObject reloadingPrompt;

    // Boost management
    private float baseFireRate;
    private float baseReloadTime;
    private Coroutine boostCoroutine;

    void Start()
    {
        currentAmmo = maxAmmo;

        baseFireRate = fireRate;
        baseReloadTime = reloadTime;

        UpdateAmmoUI();
        if (reloadingPrompt != null) reloadingPrompt.SetActive(false);
    }

    void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time < nextFireTime) return;

        // Editor / Desktop mouse
        if (Mouse.current != null && Mouse.current.leftButton != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Avoid clicking through UI (mouse)
            if (ignoreUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mp = Mouse.current.position.ReadValue();
            Shoot(mp);
            return;
        }

        // Mobile touch
        if (enableMobileControls && Touchscreen.current != null && Touchscreen.current.primaryTouch != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                // NOTE: For touch UI blocking with the new input system, add the Input System UI Input Module to your EventSystem.
                if (ignoreUI && EventSystem.current != null)
                {
                    // If you use the Input System UI Input Module, EventSystem.current.IsPointerOverGameObject() will generally work.
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;
                }

                Vector2 pos = touch.position.ReadValue();
                Shoot(pos);
            }
        }

        // Keyboard manual reload (optional, Editor)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }
    }

    void Shoot(Vector2 screenPos)
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        UpdateAmmoUI();

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100f);

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Standard Unity API - works across platforms
        Rigidbody brb = bullet.GetComponent<Rigidbody>();
        if (brb != null)
            brb.linearVelocity = direction * shootForce;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (reloadingPrompt != null) reloadingPrompt.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        if (reloadingPrompt != null) reloadingPrompt.SetActive(false);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    // Methods used by AmmoBoostCollectible
    public void RefillAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateAmmoUI();
    }

    public void ApplyBoost(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            ResetBoost();
        }

        boostCoroutine = StartCoroutine(BoostRoutine(fireRateMultiplier, reloadTimeMultiplier, duration));
    }

    private IEnumerator BoostRoutine(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        fireRate = Mathf.Max(0.01f, baseFireRate * fireRateMultiplier);
        reloadTime = Mathf.Max(0.05f, baseReloadTime * reloadTimeMultiplier);

        yield return new WaitForSeconds(duration);

        ResetBoost();
        boostCoroutine = null;
    }

    private void ResetBoost()
    {
        fireRate = baseFireRate;
        reloadTime = baseReloadTime;
    }
}