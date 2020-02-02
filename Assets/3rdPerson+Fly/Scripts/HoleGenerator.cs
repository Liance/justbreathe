using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject [] holes;
    private float coolDown = 5f;
    private List<GameObject> activeHoles;
    [SerializeField]
    private GameEnvironmentManager environmentManager;
    // Start is called before the first frame update
    void Start()
    {
        activeHoles = new List<GameObject>();
        StartCoroutine(WaitAndMakeHoly());
    }

    IEnumerator WaitAndMakeHoly()
    {
        var randHole = GetRandomHole();
        environmentManager.SetOxygenLevelText();
        if (randHole)
        {
            randHole.SetActive(true);
            randHole.GetComponent<Hole>().IsReleasingOxygen = true;
            activeHoles.Add(randHole);
            randHole.GetComponent<Hole>().SetHoleParticlesActive(true);
            GameEnvironmentManager.NumberOfDomeHoles= GameEnvironmentManager.NumberOfDomeHoles + 1;
            Debug.Log(GameEnvironmentManager.NumberOfDomeHoles);
            environmentManager.SetOxygenLevelText();
        }
        else
        {
            Debug.Log("Null hole");
        }
        coolDown = Random.Range(5, 10);
        yield return new WaitForSeconds(coolDown);
        StartCoroutine(WaitAndMakeHoly());
    }

    private GameObject GetRandomHole()
    {
        if(holes == null)
        {
            return null;
        }
        int randomHole = Random.Range(0, holes.Length);
        return (holes[randomHole].activeInHierarchy)? null: holes[randomHole];
    }

    public void SetInactive(GameObject hole)
    {
        activeHoles.Remove(hole);
        hole.SetActive(false);
        GameEnvironmentManager.NumberOfDomeHoles = Mathf.Clamp(GameEnvironmentManager.NumberOfDomeHoles - 1, 0, 40);
        environmentManager.SetOxygenLevelText();
    }
}
