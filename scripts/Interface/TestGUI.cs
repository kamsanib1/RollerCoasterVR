using UnityEngine;
using System.Collections;

public class TestGUI : MonoBehaviour {

    TrackBuilder tb;
    bool _trainAnimFlag = false;
    void OnGUI()
    {
        Rect rect = new Rect(new Vector2(Screen.width - 120, 20), new Vector2(100, Screen.height - 40));
        GUILayout.BeginArea(rect);
        if (GUILayout.Button("train")) { train(); }
        if (GUILayout.Button("animal")) { animal(); }
        if (GUILayout.Button("run train")) {
            Debug.Log("train anim");
            if (_trainAnimFlag) {
                trainAnim();
            }
        }
        GUILayout.EndArea();
    }
    void train() {
        string code = "track 50 up 60 0 0;track 70 down 120 0 0;track 20 up 60 0 0;track 50 forward 0 0 0;";
        GameObject startPoint = new GameObject("track start point");
        startPoint.transform.position = new Vector3(0, 2, 0);
        tb = new TrackBuilder(startPoint);
        tb.generate(code);
        _trainAnimFlag = true;
    }
    void animal() {
        string code = "while(1<3){walk 10 left 90;}";
        //Data.currentCode = code;
        GameObject animal = GameObject.Instantiate(Data.files[0].file);
        AnimalAnimation _aa =  animal.AddComponent<AnimalAnimation>();
        AnimalStats __as = animal.AddComponent<AnimalStats>();
        __as.setType(0);
    }
    void trainAnim() {
        GameObject train = GameObject.Instantiate(Data.data.train_normal);
        train.transform.position = new Vector3(0, 2, 0);
        TrainAnimation __ta = train.AddComponent<TrainAnimation>();
        __ta.setTrackBuilder(tb);
    }
}
