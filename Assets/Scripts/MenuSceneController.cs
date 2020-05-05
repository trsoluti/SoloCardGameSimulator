using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCandyland() => SceneManager.LoadScene("Candyland");
    public void PlayDoofMRI() => SceneManager.LoadScene("DoofMRI");
    public void PlayElementalist() => SceneManager.LoadScene("Elementalist");
    public void PlayGoodKnights() => SceneManager.LoadScene("GoodKnights");
    public void PlayOneLongBattle() => SceneManager.LoadScene("OneLongBattle");
    public void PlayOrderUp() => SceneManager.LoadScene("OrderUp");
    public void PlayTheOriginalSix() => SceneManager.LoadScene("OriginalSix");
    public void PlaySaveTheVillage() => SceneManager.LoadScene("SaveTheVillage");
    public void PlaySpaceRansom() => SceneManager.LoadScene("SpaceRansom");
    public void PlayUnforseenCircumstances() => SceneManager.LoadScene("UnforseenConsequences");
    
}
