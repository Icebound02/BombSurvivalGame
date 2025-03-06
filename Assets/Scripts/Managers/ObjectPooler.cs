using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler singleton;

    [SerializeField] private GameObject kickEffectPrefab = default;
    private List<GameObject> kickEffects = new List<GameObject>();

    [SerializeField] private GameObject stickyAnimPrefab = default;
    private List<GameObject> stickyAnims = new List<GameObject>();

    [SerializeField] private GameObject explosionPrefab = default;
    private List<GameObject> explosions = new List<GameObject>();

    private void Awake()
    {
        singleton = this;
    }

    public GameObject SpawnKickEffect(Vector3 position)
    {
        // Retrieve from pool
        for(int i = 0; i < kickEffects.Count; ++i)
        {
            if(Retrieve(kickEffects[i].gameObject, position, Vector3.one, Quaternion.identity))
                return kickEffects[i];
        }

        // Create new
        GameObject kickEffect = Instantiate(kickEffectPrefab, position, Quaternion.identity, null);//.GetComponent<GameObject>();
        kickEffects.Add(kickEffect);
        return kickEffect;
    }
    public GameObject SpawnStickyAnim(Transform parent, float angle)
    {
        // Retrieve from pool
        for(int i = 0; i < stickyAnims.Count; ++i)
        {
            if(Retrieve(stickyAnims[i].gameObject, Vector3.zero, Vector3.one, Quaternion.identity))
                return stickyAnims[i];
        }

        // Create new
        GameObject stickyAnim = Instantiate(stickyAnimPrefab, Vector3.zero, Quaternion.identity, null);//.GetComponent<GameObject>();
        stickyAnim.transform.SetParent(parent);
        stickyAnim.transform.localPosition = Vector3.zero;
        stickyAnim.transform.eulerAngles = new Vector3(0f, 0f, angle);
        stickyAnims.Add(stickyAnim);
        return stickyAnim;
    }
    public GameObject SpawnExplosion(Vector3 position, Vector3 scale)
    {
        // Retrieve from pool
        for(int i = 0; i < explosions.Count; ++i)
        {
            if(Retrieve(explosions[i].gameObject, position, scale, Quaternion.identity))
                return explosions[i];
        }

        // Create new
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity, null);//.GetComponent<GameObject>();
        explosion.transform.localScale = scale;
        explosions.Add(explosion);
        return explosion;
    }

    public bool Retrieve(GameObject obj, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        if(obj.activeSelf)
            return false;
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;
        return true;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(null);
        obj.transform.localPosition = Vector3.zero;
    }
}
